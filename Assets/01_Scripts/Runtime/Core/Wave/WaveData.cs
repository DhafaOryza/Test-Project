using System.Collections.Generic;

namespace _01_Scripts.Runtime.Core.Wave
{
    [System.Serializable]
    public class WaveData
    {
        public List<SpawnEntry> Enemies;
        
        public float PreparationDuration = 15f;
        public float SpawnInterval = 0.2f;
    }
}