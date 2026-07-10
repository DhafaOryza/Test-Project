using System;
using System.Collections;
using System.Collections.Generic;
using LumineREx.Utils.Singleton;
using UnityEngine;

namespace _01_Scripts.Runtime.Core.ActionSystem
{
    /// <summary>
    /// Central system responsible for executing <see cref="GameAction"/> objects through a
    /// three-phase pipeline: Pre → Perform → Post.
    /// <para>
    /// Each phase collects and executes a list of reactions (child <see cref="GameAction"/>s)
    /// in sorted or insertion order. Subscribers can hook into Pre/Post phases via
    /// <see cref="SubscribeReaction{T}"/>, while the Perform phase is driven by a registered
    /// performer delegate via <see cref="AttachPerformer{T}"/>.
    /// </para>
    /// <para>
    /// This class is a singleton. Access it via <c>ActionSystem.Instance</c>.
    /// </para>
    /// </summary>
    public class ActionSystem : MonoBehaviour
    {
        /// <summary>
        /// The active reaction list being populated and consumed during the current phase of
        /// <see cref="Flow"/>. Set to each action's Pre/Perform/Post reaction list in turn.
        /// </summary>
        private List<GameAction> _reactions = null;

        /// <summary>
        /// Gets a value indicating whether a <see cref="GameAction"/> is currently being
        /// performed. Only one action can run at a time; calls to <see cref="Perform"/> while
        /// this is <c>true</c> are silently ignored.
        /// </summary>
        public bool IsPerforming { get; private set; } = false;

        /// <summary>
        /// Stores callbacks subscribed to the <b>Pre</b> phase, keyed by the
        /// <see cref="GameAction"/> subtype they observe.
        /// </summary>
        private static readonly Dictionary<Type, List<Action<GameAction>>> PreSubs = new();

        /// <summary>
        /// Stores callbacks subscribed to the <b>Post</b> phase, keyed by the
        /// <see cref="GameAction"/> subtype they observe.
        /// </summary>
        private static readonly Dictionary<Type, List<Action<GameAction>>> PostSubs = new();

        /// <summary>
        /// Stores the coroutine performer delegates registered for each <see cref="GameAction"/>
        /// subtype. Only one performer per type is supported; registering again overwrites the
        /// previous one.
        /// </summary>
        private static readonly Dictionary<Type, Func<GameAction, IEnumerator>> Performer = new();

        /// <summary>
        /// Gets or sets a value indicating whether reaction lists are sorted before execution.
        /// <para>
        /// When <c>true</c> (default), reactions with a <see cref="GameAction.SortingCode"/> are
        /// executed first (in ascending code order), followed by unsorted reactions in their
        /// original insertion order.
        /// </para>
        /// <para>
        /// Set to <c>false</c> to disable sorting and preserve pure insertion order (backward
        /// compatible behaviour).
        /// </para>
        /// </summary>
        public bool UseSorting { get; private set; } = true;
        
        private Coroutine _activeFlowCoroutine;

        // If true, only actions matching _lockedActionType are allowed to be processed
        private bool _isLocked = false;
        private Type _lockedActionType = null;

        // -------------------------------------------------------------------------
        // Public API
        // -------------------------------------------------------------------------

        /// <summary>
        /// Starts executing the given <paramref name="action"/> through the full
        /// Pre → Perform → Post pipeline.
        /// </summary>
        /// <remarks>
        /// If an action is already in progress (<see cref="IsPerforming"/> is <c>true</c>),
        /// this call is a no-op. The optional <paramref name="onPerformFinished"/> callback is
        /// invoked on the same frame the pipeline completes.
        /// </remarks>
        /// <param name="action">The root <see cref="GameAction"/> to execute.</param>
        /// <param name="onPerformFinished">
        /// Optional callback invoked after all three phases complete. Runs on the Unity main thread.
        /// </param>
        public void Perform(GameAction action, Action onPerformFinished = null)
        {
            if (_isLocked && action.GetType() != _lockedActionType)
                return; 

            if (IsPerforming) return;

            IsPerforming = true;
            _activeFlowCoroutine = StartCoroutine(Flow(action, () =>
            {
                IsPerforming = false;
                _activeFlowCoroutine = null;
                onPerformFinished?.Invoke();
            }));
        }

        /// <summary>
        /// Appends a reaction to the current active reaction list without assigning a sorting
        /// code. Reactions added this way are executed after all sorted reactions (in insertion
        /// order) when <see cref="UseSorting"/> is <c>true</c>.
        /// </summary>
        /// <remarks>
        /// This method exists for backward compatibility. Prefer
        /// <see cref="AddSortedReaction"/> for new code.
        /// </remarks>
        /// <param name="gameAction">The <see cref="GameAction"/> to append as a reaction.</param>
        public void AddReaction(GameAction gameAction)
        {
            _reactions?.Add(gameAction);
        }

        /// <summary>
        /// Appends a reaction to the current active reaction list and assigns a sorting code if
        /// one has not already been set.
        /// </summary>
        /// <remarks>
        /// When <see cref="UseSorting"/> is <c>true</c>, reactions with a lower sorting code
        /// value execute before those with a higher value. Reactions without a code always
        /// execute last.
        /// </remarks>
        /// <param name="gameAction">The <see cref="GameAction"/> to append as a reaction.</param>
        /// <param name="priority">
        /// Execution priority for this reaction. Higher values cause earlier execution.
        /// Defaults to <c>0</c>.
        /// </param>
        /// <param name="sourceUID">
        /// Optional unique identifier of the originating object, used as a secondary sort key
        /// when two reactions share the same <paramref name="priority"/>. Pass <c>null</c> to
        /// omit.
        /// </param>
        public void AddSortedReaction(GameAction gameAction, int priority = 0, string sourceUID = null)
        {
            if (_reactions == null) return;

            if (!gameAction.SortingCode.HasValue)
            {
                gameAction.SortingCode = SortingCode.Generate(priority, sourceUID);
            }

            _reactions.Add(gameAction);
        }
        
        /// <summary>
        /// "Emergency stop": forcibly halts the currently running flow (including all nested
        /// reactions and any performer running inside it), discards whatever reactions are
        /// still queued, then runs this action on its own. If lockToThisType is true, any
        /// other action type will be rejected by Perform() until Unlock() is called.
        /// </summary>
        public void ForcePerform(GameAction action, bool lockToThisType = true)
        {
            if (_activeFlowCoroutine != null)
            {
                StopCoroutine(_activeFlowCoroutine);
                _activeFlowCoroutine = null;
            }

            _reactions?.Clear();
            _reactions = null;
            IsPerforming = false;

            if (lockToThisType)
            {
                _isLocked = true;
                _lockedActionType = action.GetType();
            }

            IsPerforming = true;
            _activeFlowCoroutine = StartCoroutine(Flow(action, () =>
            {
                IsPerforming = false;
                _activeFlowCoroutine = null;
            }));
        }

        // -------------------------------------------------------------------------
        // Static Registration API
        // -------------------------------------------------------------------------

        /// <summary>
        /// Registers a coroutine-based performer for the given <typeparamref name="T"/> action
        /// type. The performer is invoked during the <b>Perform</b> phase of
        /// <see cref="Flow"/>.
        /// </summary>
        /// <remarks>
        /// Only one performer per type is supported. Calling this method again for the same type
        /// silently replaces the previous performer.
        /// </remarks>
        /// <typeparam name="T">
        /// The concrete <see cref="GameAction"/> subtype this performer handles.
        /// </typeparam>
        /// <param name="performer">
        /// A delegate that receives the typed action and returns an <see cref="IEnumerator"/>
        /// coroutine. The coroutine is fully awaited before the Post phase begins.
        /// </param>
        public static void AttachPerformer<T>(Func<T, IEnumerator> performer) where T : GameAction
        {
            Type type = typeof(T);
            IEnumerator WrapperPerformer(GameAction action) => performer((T)action);
            Performer[type] = WrapperPerformer;
        }

        /// <summary>
        /// Removes the registered performer for the given <typeparamref name="T"/> action type.
        /// If no performer is registered, this is a no-op.
        /// </summary>
        /// <typeparam name="T">
        /// The concrete <see cref="GameAction"/> subtype whose performer should be removed.
        /// </typeparam>
        public static void DetachPerformer<T>() where T : GameAction
        {
            Type type = typeof(T);
            Performer.Remove(type);
        }

        /// <summary>
        /// Subscribes a callback to the Pre or Post reaction phase for the given
        /// <typeparamref name="T"/> action type.
        /// </summary>
        /// <remarks>
        /// The callback is invoked synchronously at the start of the corresponding phase, before
        /// any reactions in that phase's list are executed. Multiple callbacks can be registered
        /// for the same type and timing; they are invoked in registration order.
        /// </remarks>
        /// <typeparam name="T">
        /// The concrete <see cref="GameAction"/> subtype to observe.
        /// </typeparam>
        /// <param name="reaction">
        /// The callback to invoke when an action of type <typeparamref name="T"/> reaches the
        /// specified phase.
        /// </param>
        /// <param name="timing">
        /// <see cref="ReactionTiming.PRE"/> to hook the Pre phase;
        /// <see cref="ReactionTiming.POST"/> to hook the Post phase.
        /// </param>
        public static void SubscribeReaction<T>(Action<T> reaction, ReactionTiming timing) where T : GameAction
        {
            Dictionary<Type, List<Action<GameAction>>> subs = timing == ReactionTiming.PRE ? PreSubs : PostSubs;
            void WrapperPerformer(GameAction action) => reaction((T)action);

            if (!subs.ContainsKey(typeof(T)))
            {
                subs.Add(typeof(T), new());
            }

            subs[typeof(T)].Add(WrapperPerformer);
        }

        /// <summary>
        /// Unsubscribes a previously registered callback from the Pre or Post reaction phase.
        /// </summary>
        /// <remarks>
        /// Due to the lambda wrapper created during <see cref="SubscribeReaction{T}"/>, removal
        /// by delegate reference does not work as expected. Consider storing the wrapper reference
        /// manually if precise removal is required.
        /// </remarks>
        /// <typeparam name="T">
        /// The concrete <see cref="GameAction"/> subtype the callback was registered for.
        /// </typeparam>
        /// <param name="reaction">The callback to remove.</param>
        /// <param name="timing">
        /// The phase (<see cref="ReactionTiming.PRE"/> or <see cref="ReactionTiming.POST"/>) the
        /// callback was registered under.
        /// </param>
        public static void UnsubscribeReaction<T>(Action<T> reaction, ReactionTiming timing) where T : GameAction
        {
            Dictionary<Type, List<Action<GameAction>>> subs = timing == ReactionTiming.PRE ? PreSubs : PostSubs;
            if (subs.ContainsKey(typeof(T)))
            {
                void WrapperPerformer(GameAction action) => reaction((T)action);
                subs[typeof(T)].Remove(WrapperPerformer);
            }
        }

        /// <summary>
        /// Clears all state: active reaction list, all Pre/Post subscriptions, and all
        /// registered performers.
        /// </summary>
        /// <remarks>
        /// Call this when tearing down a scene or resetting the game state to avoid stale
        /// delegates from a previous session.
        /// </remarks>
        public void ClearAll()
        {
            _reactions = null;
            PreSubs.Clear();
            PostSubs.Clear();
            Performer.Clear();
        }
        
        /// <summary>Releases the lock, e.g. after a restart or returning to the main menu.</summary>
        public void Unlock()
        {
            _isLocked = false;
            _lockedActionType = null;
        }

        // -------------------------------------------------------------------------
        // Private Pipeline
        // -------------------------------------------------------------------------

        /// <summary>
        /// Core coroutine that drives the three-phase pipeline for a single
        /// <see cref="GameAction"/>.
        /// </summary>
        /// <param name="action">The action to process.</param>
        /// <param name="onFlowFinished">
        /// Optional callback invoked after the Post phase completes.
        /// </param>
        private IEnumerator Flow(GameAction action, Action onFlowFinished = null)
        {
            // ========== PRE PHASE ==========
            _reactions = action.PreReactions;
            PerformerSubscriber(action, PreSubs);
            yield return PerformSortedReactions();

            // ========== PERFORM PHASE ==========
            _reactions = action.Performreactions;
            yield return PerformPerformer(action);
            yield return PerformSortedReactions();

            // ========== POST PHASE ==========
            _reactions = action.PostReactions;
            PerformerSubscriber(action, PostSubs);
            yield return PerformSortedReactions();

            onFlowFinished?.Invoke();
        }

        /// <summary>
        /// Takes a snapshot of the current <see cref="_reactions"/> list, optionally sorts it by
        /// <see cref="GameAction.SortingCode"/>, then executes each reaction recursively via
        /// <see cref="Flow"/>.
        /// </summary>
        /// <remarks>
        /// A snapshot is used to prevent issues caused by reactions that add further reactions to
        /// the list mid-iteration.
        /// <para>
        /// Sort order when <see cref="UseSorting"/> is <c>true</c>:
        /// <list type="number">
        ///   <item>Reactions with a <see cref="GameAction.SortingCode"/>, ascending.</item>
        ///   <item>Reactions without a code, in their original insertion order.</item>
        /// </list>
        /// </para>
        /// </remarks>
        private IEnumerator PerformSortedReactions()
        {
            if (_reactions == null || _reactions.Count == 0)
                yield break;

            // 1. Create snapshot (prevent concurrent modification)
            var snapshot = new List<GameAction>(_reactions);

            // 2. Sort snapshot if sorting enabled
            if (UseSorting)
            {
                snapshot.Sort((a, b) =>
                {
                    bool aHasSort = a.SortingCode.HasValue;
                    bool bHasSort = b.SortingCode.HasValue;

                    if (aHasSort && bHasSort)
                        return a.SortingCode.Value.CompareTo(b.SortingCode.Value);

                    if (aHasSort) return -1;
                    if (bHasSort) return 1;

                    return 0; // Both unsorted — preserve insertion order
                });
            }

            // 3. Execute sorted snapshot
            foreach (var reaction in snapshot)
            {
                yield return Flow(reaction);
            }
        }

        /// <summary>
        /// Looks up and invokes the registered performer coroutine for the given
        /// <paramref name="action"/>'s concrete type. If no performer is registered, the method
        /// yields nothing and returns immediately.
        /// </summary>
        /// <param name="action">The action whose performer should be invoked.</param>
        private IEnumerator PerformPerformer(GameAction action)
        {
            Type type = action.GetType();
            if (Performer.ContainsKey(type))
            {
                yield return Performer[type](action);
            }
        }

        /// <summary>
        /// Invokes all callbacks registered in <paramref name="subs"/> for the concrete type of
        /// <paramref name="action"/>. Used to fire Pre and Post phase subscribers synchronously
        /// at the start of each phase.
        /// </summary>
        /// <param name="action">
        /// The action being processed; its runtime type is used as the lookup key.
        /// </param>
        /// <param name="subs">
        /// The subscription dictionary to look up — either <see cref="PreSubs"/> or
        /// <see cref="PostSubs"/>.
        /// </param>
        private void PerformerSubscriber(GameAction action, Dictionary<Type, List<Action<GameAction>>> subs)
        {
            Type type = action.GetType();
            if (subs.ContainsKey(type))
            {
                foreach (var sub in subs[type])
                {
                    sub(action);
                }
            }
        }
    }
}