namespace LumineREx.Utils.Gacha
{
    /// <summary>
    /// Configuration for every Rarity
    /// </summary>
    [System.Serializable]
    public class RarityConfig
    {
        public Rarity Rarity;
        public float BaseRate;           // Base probability (0-100)
        public int HardPity;             // Guaranteed after X pulls
        public int SoftPityStart;        // Soft pity start from pull-X
        public float SoftPityIncrement;  // Increment per pull di soft pity (0-100)
    }
} 