namespace _01_Scripts.Runtime.Core.Character
{
    public class CharacterExp
    {
        private int _level = 1;
        private int _currentExp = 0;
        private int _requiredExp = 0;
        
        public int CurrentLevel => _currentExp;
        public int CurrentExp => _currentExp;
        public int RequiredExp => _requiredExp;

        public void AddExp(int amount)
        {
            
        }

        public void IncreaseLevel()
        {
            _currentExp -= _requiredExp;
            _level++;
        }
    }
}