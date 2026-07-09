using UnityEngine;

namespace _01_Scripts.Runtime.Core.SlotSystem
{
    public class SymbolView : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer _spriteRenderer;

        public void SetSymbol(Sprite sprite)
        {
            _spriteRenderer.sprite = sprite;
        }
    }
}