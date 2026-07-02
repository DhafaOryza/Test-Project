using UnityEngine;

namespace _Dev.Script.Runtime.Core.SlotSystem
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