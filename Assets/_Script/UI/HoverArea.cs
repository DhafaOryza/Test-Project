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
    [SerializeField] private EnemyManager enemyManager; // Wajib diisi untuk Tooltip UI
    [SerializeField] private GameManager gameManager;   // Wajib diisi khusus untuk zona Player

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
                if (gameManager != null) message = $"Sisa Deck: {gameManager.DrawPileCount}";
                break;
            case HoverZoneType.PlayerGraveyard:
                if (gameManager != null) message = $"Tumpukan Discard: {gameManager.DiscardPileCount}";
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