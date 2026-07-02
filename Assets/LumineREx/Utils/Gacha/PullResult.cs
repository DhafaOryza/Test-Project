namespace LumineREx.Utils.Gacha
{
    /// <summary>
    /// Result for Pull
    /// </summary>
    public class PullResult<T>
    {
        public GachaItem<T> Item;
        public int PityCounter;
        public bool WasPityActivated;
        public bool WasGuaranteed;
    }
}