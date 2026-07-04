using UnityEngine;

public class HoverArea : MonoBehaviour
{
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] private bool isGraveyard;

    public void OnMouseEnter()
    {
        if (enemyManager == null)
        return;

        if (isGraveyard)
        {
            enemyManager.ShowTooltip($"Musuh Dikalahkan: {enemyManager.DefeatedCount}");
        }
        else
        {
            enemyManager.ShowTooltip($"Sisa Deck: {enemyManager.CardsLeft}");
        }



       
    }

    public void OnMouseExit()
    {
        if (enemyManager == null)
        return;

        enemyManager.HideTooltip();
    }
}
