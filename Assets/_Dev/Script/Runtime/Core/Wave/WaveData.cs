using System.Collections.Generic;

namespace _Dev.Script.Runtime.Core.Wave
{
    [System.Serializable]
    public class WaveData
    {
        public List<SpawnEntry> Enemies;
        
        public float PreparationDuration = 15f;
        public float SpawnInterval = 0.2f;
    }
}