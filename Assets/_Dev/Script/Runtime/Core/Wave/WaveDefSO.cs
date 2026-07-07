using System.Collections.Generic;
using UnityEngine;

namespace _Dev.Script.Runtime.Core.Wave
{
    [CreateAssetMenu(fileName = "WaveDefSO", menuName = "Game/WaveDefSO", order = 0)]
    public class WaveDefSO : ScriptableObject
    {
        [SerializeReference]
        private List<WaveData> waveData;
        
        public List<WaveData> WaveData => waveData;
    }
}