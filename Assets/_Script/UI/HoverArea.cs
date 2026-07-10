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
    
    [Header("References")]
    [SerializeField] private EnemyManager enemyManager; 
    [SerializeField] private GameManager gameManager; 

    private void OnMouseEnter()
    {
        if (enemyManager == null) return;

        string message = "";
        switch (zoneType)
        {
            case HoverZoneType.EnemyDeck:
                message = $"Sisa Deck Musuh: {enemyManager.CardsLeft}";
                break;
            case HoverZoneType.EnemyGraveyard:
                message = $"Musuh Dikalahkan: {enemyManager.DefeatedCount}";
                break;
            case HoverZoneType.PlayerDeck:
                if (gameManager != null) message = $"Sisa Deck: {GameManager.Instance.deckManager.DrawPileCount}";
                break;
            case HoverZoneType.PlayerGraveyard:
                if (gameManager != null) message = $"Tumpukan Discard: {GameManager.Instance.deckManager.DiscardPileCount}";
                break;
        }

        enemyManager.ShowTooltip(message);
    }

    private void OnMouseExit()
    {
        if (enemyManager == null) return;
        enemyManager.HideTooltip();
    }
}