using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [SerializeField] private HandManager handManager;
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] private ChanceUI chanceUI;
    [SerializeField] private float delayBeforeEnemyTurn = 1f;

    private void OnEnable()
    {
        handManager.OnplaysChanged += chanceUI.SetChanceUI;
        handManager.OnPlayExhausted += HandleEnemyTurn;
    }

    private void OnDisable()
    {
        handManager.OnplaysChanged -= chanceUI.SetChanceUI;
        handManager.OnPlayExhausted -= HandleEnemyTurn;
    }

    private void HandleEnemyTurn()
    {
        Invoke(nameof(ExecuteEnemyTurn), delayBeforeEnemyTurn);
    }

    private void ExecuteEnemyTurn()
    {
        enemyManager.EnemyAttackPlayer();
        handManager.ResetRoundPlays();

    }
}