using System;
using System.Collections.Generic;
using System.Linq;

namespace LumineREx.Utils.Gacha
{
    
    /// <summary>
    /// Main gacha system with generic item type support
    /// </summary>
    public class GachaSystem<T>
    {
        private BannerConfig<T> _currentBanner;
        private PitySystem _pitySystem;
        private Random _random;
        
        public GachaSystem(BannerConfig<T> banner, int seed = -1)
        {
            _currentBanner = banner;
            _pitySystem = new PitySystem();
            _random = seed == -1 ? new Random() : new Random(seed);
        }
        
        /// <summary>
        /// Single pull
        /// </summary>
        public PullResult<T> Pull()
        {
            // Increment all pity counters
            foreach (var config in _currentBanner.RarityConfigs)
            {
                _pitySystem.IncrementPity(config.Rarity);
            }
            
            // Determine rarity
            Rarity pulledRarity = DetermineRarity();
            
            // Get item from that rarity
            GachaItem<T> item = GetItemFromRarity(pulledRarity);
            
            // Create result
            PullResult<T> result = new PullResult<T>
            {
                Item = item,
                PityCounter = _pitySystem.GetPityCounter(pulledRarity),
                WasPityActivated = _pitySystem.GetPityCounter(pulledRarity) >= GetConfigForRarity(pulledRarity).SoftPityStart,
                WasGuaranteed = _pitySystem.IsGuaranteed(pulledRarity)
            };
            
            // Reset pity for any rarity (and lower pities)
            ResetLowerRarityPities(pulledRarity);
            
            return result;
        }
        
        /// <summary>
        /// Multi pull (e.g. 10x)
        /// </summary>
        public List<PullResult<T>> MultiPull(int count = 10)
        {
            List<PullResult<T>> results = new List<PullResult<T>>();
            for (int i = 0; i < count; i++)
            {
                results.Add(Pull());
            }
            return results;
        }
        
        private Rarity DetermineRarity()
        {
            // Sort rarity configs from highest to lowest
            var sortedConfigs = _currentBanner.RarityConfigs
                .OrderByDescending(c => (int)c.Rarity)
                .ToList();
            
            foreach (var config in sortedConfigs)
            {
                float probability = _pitySystem.CalculateProbability(config);
                float roll = (float)(_random.NextDouble() * 100);
                
                if (roll < probability)
                {
                    return config.Rarity;
                }
            }
            
            // Fallback rarity to lowest
            return sortedConfigs.Last().Rarity;
        }
        
        private GachaItem<T> GetItemFromRarity(Rarity rarity)
        {
            // Filter items by rarity
            var itemsOfRarity = _currentBanner.ItemPool
                .Where(i => i.Rarity == rarity)
                .ToList();
            
            if (itemsOfRarity.Count == 0)
                return null;
            
            // 50/50 system for featured items
            if (_currentBanner.Use5050System && rarity == Rarity.Legendary)
            {
                bool isGuaranteed = _pitySystem.IsGuaranteed(rarity);
                var featuredItems = itemsOfRarity.Where(i => i.IsFeatured).ToList();
                var standardItems = itemsOfRarity.Where(i => !i.IsFeatured).ToList();
                
                if (featuredItems.Count > 0)
                {
                    if (isGuaranteed)
                    {
                        // Guaranteed featured
                        _pitySystem.SetGuaranteed(rarity, false);
                        return featuredItems[_random.Next(featuredItems.Count)];
                    }
                    else
                    {
                        // 50/50
                        float roll = (float)(_random.NextDouble() * 100);
                        if (roll < _currentBanner.FeaturedRate)
                        {
                            _pitySystem.SetGuaranteed(rarity, false);
                            return featuredItems[_random.Next(featuredItems.Count)];
                        }
                        else
                        {
                            _pitySystem.SetGuaranteed(rarity, true);
                            return standardItems.Count > 0 ? 
                                standardItems[_random.Next(standardItems.Count)] : 
                                featuredItems[_random.Next(featuredItems.Count)];
                        }
                    }
                }
            }
            
            // Random pick from items to that rarity 
            return itemsOfRarity[_random.Next(itemsOfRarity.Count)];
        }
        
        private RarityConfig GetConfigForRarity(Rarity rarity)
        {
            return _currentBanner.RarityConfigs.FirstOrDefault(c => c.Rarity == rarity);
        }
        
        private void ResetLowerRarityPities(Rarity pulledRarity)
        {
            foreach (var config in _currentBanner.RarityConfigs)
            {
                if ((int)config.Rarity <= (int)pulledRarity)
                {
                    _pitySystem.ResetPity(config.Rarity);
                }
            }
        }
        
        // ============= UTILITY METHODS =============
        
        public int GetCurrentPity(Rarity rarity)
        {
            return _pitySystem.GetPityCounter(rarity);
        }
        
        public void SetPity(Rarity rarity, int count)
        {
            // For testing or loading saved data
            for (int i = 0; i < count; i++)
            {
                _pitySystem.IncrementPity(rarity);
            }
        }
        
        public Dictionary<Rarity, float> GetCurrentProbabilities()
        {
            Dictionary<Rarity, float> probs = new Dictionary<Rarity, float>();
            foreach (var config in _currentBanner.RarityConfigs)
            {
                probs[config.Rarity] = _pitySystem.CalculateProbability(config);
            }
            return probs;
        }
    }
}