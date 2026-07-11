using _01_Scripts.Runtime.Core.Character;
using _01_Scripts.Runtime.Enum;
using UnityEngine;

namespace _01_Scripts.Runtime.Interface
{
    public interface IStatsInfo
    {
        Transform Transform { get; }
        CharacterStats GetCharacterStats();
    }
}