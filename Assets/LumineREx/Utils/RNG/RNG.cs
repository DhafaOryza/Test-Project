using System;
using System.Collections.Generic;

namespace LumineREx.Utils.RNG
{
    public class RNG
    {
        private Random _random;

        private int _pity;

        public RNG ()
        {
            SetRandomSeed();
        }

        private void SetRandomSeed()
        {
            _random = new Random();  
        }

        public int Roll(int min, int max)
        {
            return _random.Next(min, max);
        }
        
        public bool RollChance (int chance)
        {
            double roll = _random.NextDouble();
            
            return roll < chance;
        }

        public T GetWeight<T>(List<WeightedItem<T>> items)
        {
            int totalWeight = 0;
            
            //Get All weight in Total
            foreach (var item in items) 
            {
                totalWeight += item.Weight;
            }
            
            //Roll random number between zero to total weight
            int roll  = _random.Next(0, totalWeight);
            
            //Store chance
            int current = 0;

            foreach (var item in items) 
            {
                current += item.Weight;

                if (roll < current)
                {
                    return item.Item;
                }
            }
            
            return default(T);  
        }
    }
}