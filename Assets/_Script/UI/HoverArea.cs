using UnityEngine;

public enum HoverZoneType { 
    EnemyDeck, 
    EnemyGraveyard, 
    PlayerDeck, 
    PlayerGraveyard 
}

public class HoverArea : MonoBehaviour
{
    [SerializeField] private HoverZoneType zoneType;
     

    private void OnMouseEnter()
    {
        if (GameManager.Instance.enemyManager == null) return;

        string message = "";
        switch (zoneType)
        {
            case HoverZoneType.EnemyDeck:
                message = $"Sisa Deck Musuh: {GameManager.Instance.enemyManager.CardsLeft}";
                break;
            case HoverZoneType.EnemyGraveyard:
                message = $"Musuh Dikalahkan: {GameManager.Instance.enemyManager.DefeatedCount}";
                break;
            case HoverZoneType.PlayerDeck:
                if (GameManager.Instance.deckManager != null) message = $"Sisa Deck: {GameManager.Instance.deckManager.DrawPileCount}";
                break;
            case HoverZoneType.PlayerGraveyard:
                if (GameManager.Instance.deckManager != null) message = $"Tumpukan Discard: {GameManager.Instance.deckManager.DiscardPileCount}";
                break;
        }

        GameManager.Instance.enemyManager.ShowTooltip(message);
    }

    private void OnMouseExit()
    {
        if (GameManager.Instance.enemyManager == null) return;
        GameManager.Instance.enemyManager.HideTooltip();
    }
}