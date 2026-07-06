using System.Collections.Generic;
using _Dev.Script.Runtime.Core.Spawner;
using LumineREx.Utils.Singleton;
using UnityEngine;

namespace _Dev.Script.Runtime.Core.Character.Ally
{
    public class AlliesManager : Singleton<AlliesManager>
    {
        [SerializeField]
        private AllySpawner _allySpawner;
        
        private readonly List<Character> _allies = new List<Character>();
        
        public void AddCharacter(Character character)
        {
            _allies.Add(character);
        }
    }
}