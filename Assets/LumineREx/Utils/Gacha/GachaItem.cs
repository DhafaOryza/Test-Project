namespace LumineREx.Utils.Gacha
{
    [System.Serializable]
    public abstract class GachaItem<T>
    {
        public string ItemId;
        public string ItemName;
        public T Item;
        public Rarity Rarity;
        public bool IsFeatured; // For 50/50 system
        public int Amount;
        public int Price;
        
        
        public GachaItem(string id, string name, T item, Rarity rarity, bool featured, int amount, int price)
        {
            ItemId = id;
            ItemName = name;
            Item = item;
            Rarity = rarity;
            IsFeatured = featured;
            Amount = amount;
            Price = price;
        }
    }
}