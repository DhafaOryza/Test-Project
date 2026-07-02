using System.Collections.Generic;

namespace _Dev.Script.Runtime.Core.ActionSystem
{
    public abstract class GameAction
    {
        public List<GameAction> PreReactions = new List<GameAction>();
        public List<GameAction> Performreactions = new List<GameAction>();
        public List<GameAction> PostReactions = new List<GameAction>();
        
        public SortingCode? SortingCode { get; set; }
    }
}