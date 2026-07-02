using System;
using System.Collections.Generic;

namespace LumineREx.Utils.Gacha
{
public class PitySystem
    {
        private Dictionary<Rarity, int> pityCounters;
        private Dictionary<Rarity, bool> guaranteedNext;
        
        public PitySystem()
        {
            pityCounters = new Dictionary<Rarity, int>();
            guaranteedNext = new Dictionary<Rarity, bool>();
            
            foreach (Rarity rarity in Enum.GetValues(typeof(Rarity)))
            {
                pityCounters[rarity] = 0;
                guaranteedNext[rarity] = false;
            }
        }
        
        public int GetPityCounter(Rarity rarity)
        {
            return pityCounters.ContainsKey(rarity) ? pityCounters[rarity] : 0;
        }
        
        public void IncrementPity(Rarity rarity)
        {
            if (pityCounters.ContainsKey(rarity))
                pityCounters[rarity]++;
            else
                pityCounters[rarity] = 1;
        }
        
        public void ResetPity(Rarity rarity)
        {
            pityCounters[rarity] = 0;
        }
        
        public bool IsGuaranteed(Rarity rarity)
        {
            return guaranteedNext.ContainsKey(rarity) && guaranteedNext[rarity];
        }
        
        public void SetGuaranteed(Rarity rarity, bool value)
        {
            guaranteedNext[rarity] = value;
        }
        
        /// <summary>
        /// Calculate probability with soft pity
        /// </summary>
        public float CalculateProbability(RarityConfig config)
        {
            int currentPity = GetPityCounter(config.Rarity);
            
            // Hard pity - 100% chance
            if (currentPity >= config.HardPity - 1)
                return 100f;
            
            // Soft pity - increment probability
            if (currentPity >= config.SoftPityStart)
            {
                int softPityPulls = currentPity - config.SoftPityStart;
                float boostedRate = config.BaseRate + (softPityPulls * config.SoftPityIncrement);
                return Math.Min(boostedRate, 100f);
            }
            
            // Normal probability
            return config.BaseRate;
        }
    }
}