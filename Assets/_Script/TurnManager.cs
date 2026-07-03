using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [SerializeField] private HandManager handManager;
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] private ChanceUI chanceUI;
    [SerializeField] private float delayBeforeEnemyTurn = 1f;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private int cardsPerTurn = 3;

    private void Start()
    {
        DrawCardForNewTurn();
    }
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
        DrawCardForNewTurn();
    }

    private void DrawCardForNewTurn()
    {
        for (int i = 0; i < cardsPerTurn; i++)
        {
            gameManager.DrawCard();
        }
    }

    public void EndTurnButtonPressed()
    {
        handManager.ForceEndTurn();
    }
}