using System.Collections.Generic;

namespace LumineREx.Utils.Gacha
{
    [System.Serializable]
    public class BannerConfig<T>
    {
        public string BannerId;
        public string BannerName;
        public List<RarityConfig> RarityConfigs;
        public List<GachaItem<T>> ItemPool;  // Generic item pool
        public bool Use5050System;
        public float FeaturedRate;
    }
}