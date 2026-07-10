using _01_Scripts.Runtime.Core.Character;
using _01_Scripts.Runtime.PoolingSystem;
using UnityEngine;

namespace _01_Scripts.Runtime.Core.Wave
{
    [System.Serializable]
    public class SpawnEntry
    {
        public CharacterDefSO Character;
        public PoolIdSO poolIdSo;
        public int Amount;
    }
}