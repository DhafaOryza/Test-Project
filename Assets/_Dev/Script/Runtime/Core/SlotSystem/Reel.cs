using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

namespace _Dev.Script.Runtime.Core.SlotSystem
{
    public class Reel : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private List<SymbolView> symbols;
        [SerializeField] private List<Sprite> sprites;

        [Header("Layout")]
        [SerializeField] private int visibleCount = 3;
        [SerializeField] private int bufferTop = 1;
        [SerializeField] private int bufferBottom = 1;
        [SerializeField] private float spacing = 1.5f;

        [Header("Spin Settings")]
        [SerializeField] private float startSpeed = 10f;
        [SerializeField] private float stopSpeed = 2f;
        [SerializeField] private float alignSpeed = 6f;
        [SerializeField] private int extraSpinSteps = 15;
        
        private ReelState _state = ReelState.Idle;

        private List<SymbolView> _slots;
        private Queue<int> _pendingValues;
        private int _initialQueueLength;

        private float _offset;
        private float _speed;

        private int SlotCount => bufferTop + visibleCount + bufferBottom;
        
        private float CenterIndex => (SlotCount - 1) * 0.5f;

        public event Action Finished;
        
        public bool IsSpinning => _state != ReelState.Idle;

        private void Awake()
        {
            _slots = new List<SymbolView>(symbols);

            foreach (var s in _slots)
                s.SetSymbol(sprites[RandomValue()]);

            ApplyPositions();
        }

        private void OnValidate()
        {
            if (symbols != null && symbols.Count != SlotCount)
            {
                Debug.LogWarning(
                    $"[{name}] symbols count ({symbols.Count}) must = " +
                    $"bufferTop + visibleCount + bufferBottom ({SlotCount})");
            }
        }

        private void Update()
        {
            if (_state == ReelState.Idle)
                return;

            switch (_state)
            {
                case ReelState.Spinning:
                    _speed = startSpeed;
                    break;

                case ReelState.Stopping:
                    float remainingRatio = _pendingValues.Count / (float)_initialQueueLength;
                    _speed = Mathf.Lerp(stopSpeed, startSpeed, remainingRatio);
                    break;
            }

            if (_state == ReelState.Spinning || _state == ReelState.Stopping)
            {
                _offset += _speed * Time.deltaTime;

                while (_offset >= spacing)
                {
                    _offset -= spacing;
                    Step();
                    
                    if (_state == ReelState.Aligning)
                        break;
                }
            }

            if (_state == ReelState.Aligning)
            {
                _offset = Mathf.MoveTowards(_offset, 0f, alignSpeed * Time.deltaTime);

                if (_offset <= 0.001f)
                {
                    _offset = 0f;
                    _state = ReelState.Idle;

                    transform.DOPunchPosition(Vector3.down * .1f, .15f);
                }
            }

            ApplyPositions();
        }

        public void Spin()
        {
            _pendingValues = null;
            _offset = 0f;
            _speed = startSpeed;
            _state = ReelState.Spinning;
        }
        
        public void Stop(int[] result)
        {
            if (result.Length != visibleCount)
            {
                Debug.LogError($"[{name}] result.Length ({result.Length}) must == visibleCount ({visibleCount})");
                return;
            }

            _pendingValues = new Queue<int>();
            
            for (int i = 0; i < extraSpinSteps; i++)
                _pendingValues.Enqueue(RandomValue());
            
            for (int i = visibleCount - 1; i >= 0; i--)
                _pendingValues.Enqueue(result[i]);

            for (int i = 0; i < bufferTop; i++)
                _pendingValues.Enqueue(RandomValue());

            _initialQueueLength = _pendingValues.Count;
            _state = ReelState.Stopping;
        }

        private void Step()
        {
            var symbol = _slots[SlotCount - 1];
            _slots.RemoveAt(SlotCount - 1);
            _slots.Insert(0, symbol);

            symbol.SetSymbol(sprites[NextValue()]);
        }

        private int NextValue()
        {
            if (_state == ReelState.Stopping && _pendingValues.Count > 0)
            {
                int value = _pendingValues.Dequeue();

                if (_pendingValues.Count == 0)
                    _state = ReelState.Aligning;

                return value;
            }

            return RandomValue();
        }

        private int RandomValue() => Random.Range(0, sprites.Count);

        private void ApplyPositions()
        {
            for (int i = 0; i < _slots.Count; i++)
            {
                var t = _slots[i].transform;
                var pos = t.localPosition;
                pos.y = (CenterIndex - i) * spacing - _offset;
                t.localPosition = pos;
            }
            
            Finished?.Invoke();
        }
    }
}