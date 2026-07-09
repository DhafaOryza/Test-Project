using System;
using System.Threading;

namespace _01_Scripts.Runtime.Core.ActionSystem
{
    /// <summary>
    /// Composite key for deterministic sorting from GameAction reactions.
    /// Comparison hierarchy: Priority → Timestamp → SourceUID → SequenceID
    /// </summary>
    public struct SortingCode : IComparable<SortingCode>, IEquatable<SortingCode>
    {
        /// <summary>
        /// Priority level (higher = execute first)
        /// </summary>
        public int Priority { get; set; }
        
        /// <summary>
        /// Creation timestamp dalam ticks (earlier = execute first)
        /// Auto-generated saat Generate() dipanggil
        /// </summary>
        public long Timestamp { get; set; }
        
        /// <summary>
        /// Unique identifier dari source object (alphabetical order)
        /// Contoh: "joker_123", "card_5H", "modifier_burn"
        /// </summary>
        public string SourceUID { get; set; }
        
        /// <summary>
        /// Global sequence number untuk tie-breaking (insertion order)
        /// Auto-incremented per Generate() call
        /// </summary>
        public int SequenceID { get; set; }
        
        // Global counter untuk SequenceID (thread-safe)
        private static int _globalSequence = 0;
        
        /// <summary>
        /// Factory method to generate SortingCode runtime
        /// </summary>
        /// <param name="priority">Priority level (default: 0)</param>
        /// <param name="sourceUID">Unique ID (default: GUID)</param>
        /// <returns>New SortingCode instance</returns>
        public static SortingCode Generate(int priority = 0, string sourceUID = null)
        {
            return new SortingCode
            {
                Priority = priority,
                Timestamp = DateTime.UtcNow.Ticks,
                SourceUID = sourceUID ?? Guid.NewGuid().ToString(),
                SequenceID = Interlocked.Increment(ref _globalSequence)
            };
        }
        
        /// <summary>
        /// Hierarchical comparison untuk sorting
        /// </summary>
        public int CompareTo(SortingCode other)
        {
            // 1. Priority (descending - higher priority first)
            if (Priority != other.Priority)
                return other.Priority.CompareTo(Priority);
            
            // 2. Timestamp (ascending - earlier first)
            if (Timestamp != other.Timestamp)
                return Timestamp.CompareTo(other.Timestamp);
            
            // 3. SourceUID (alphabetical - deterministic)
            int uidCompare = string.Compare(SourceUID, other.SourceUID, 
                                           StringComparison.Ordinal);
            if (uidCompare != 0)
                return uidCompare;
            
            // 4. SequenceID (ascending - insertion order)
            return SequenceID.CompareTo(other.SequenceID);
        }
        
        /// <summary>
        /// Equality comparison
        /// </summary>
        public bool Equals(SortingCode other)
        {
            return Priority == other.Priority &&
                   Timestamp == other.Timestamp &&
                   SourceUID == other.SourceUID &&
                   SequenceID == other.SequenceID;
        }
        
        public override bool Equals(object obj)
        {
            return obj is SortingCode other && Equals(other);
        }
        
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + Priority.GetHashCode();
                hash = hash * 31 + Timestamp.GetHashCode();
                hash = hash * 31 + (SourceUID?.GetHashCode() ?? 0);
                hash = hash * 31 + SequenceID.GetHashCode();
                return hash;
            }
        }
        
        public override string ToString()
        {
            return $"SortingCode[P:{Priority}, T:{Timestamp}, UID:{SourceUID}, Seq:{SequenceID}]";
        }
    }
}