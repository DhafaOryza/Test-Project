using UnityEngine;

public class TurnManager : MonoBehaviour
{

    [Header("Managers")]
    [SerializeField] private AllyManager allyManager;
    [SerializeField] private HandManager handManager;
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] private GameManager gameManager;

    [SerializeField] private ChanceUI chanceUI;
    [SerializeField] private float delayBeforeTurnTranstition = 1f;
    [SerializeField] private int cardsPerTurn = 3;
    [SerializeField] private  PlayerStats playerStats;

    private void Start()
    {
        
    }

    private void OnEnable()
    {
        handManager.OnPlaysChanged += chanceUI.SetChanceUI;
        handManager.OnPlaysExhausted += HandleAllyTurn;
    }

    private void OnDisable()
    {
        handManager.OnPlaysChanged -= chanceUI.SetChanceUI;
        handManager.OnPlaysExhausted -= HandleAllyTurn;
    }

    public void BeginFirstTurn()
    {
        DrawCardForNewTurn();
    }

    private void HandleAllyTurn()
    {
        Invoke(nameof(ExecuteAllyTurn), delayBeforeTurnTranstition);
    }

    private void ExecuteAllyTurn()
    {
        if (allyManager == null || allyManager.ActiveAllies.Count == 0)
        {
            Debug.Log("[TurnManager] Tidak ada Ally di arena. Langsung ke giliran Musuh!");
            ExecuteEnemyTurn();
            return;
        }
        
        Debug.Log("[TurnManager] Giliran Pasukan Ally dimulai!");
        PlayAllyAttack(0);
    }

    private void PlayAllyAttack(int index)
    {
        if (index >= allyManager.ActiveAllies.Count)
        {
            Debug.Log("[TurnManager] Semua Ally selesai menyerang. Sekarang giliran Musuh!");
            ExecuteEnemyTurn();
            return;
        }

        CardView currentAlly = allyManager.ActiveAllies[index];
        Transform enemyTarget = enemyManager.ActiveEnemyTransform;

        if (currentAlly != null || enemyTarget != null)
        {
            allyManager.AllyAttackTarget(currentAlly, enemyTarget, () =>
            {
                PlayAllyAttack(index + 1);
            });
        }
        else
        {
            PlayAllyAttack(index + 1);
        }
    }

    private void HandleEnemyTurn()
    {
        Invoke(nameof(ExecuteEnemyTurn), delayBeforeTurnTranstition);
    }

    private void ExecuteEnemyTurn()
    {
        Transform choosenTarget = playerStats.transform;
        if (allyManager != null && allyManager.ActiveAllies.Count > 0)
        {
            int RandomChoice = Random.Range(0, allyManager.ActiveAllies.Count + 1);

            if (RandomChoice < allyManager.ActiveAllies.Count)
            {
                choosenTarget = allyManager.ActiveAllies[RandomChoice].transform;
                Debug.Log($"[TurnManager] Musuh memutuskan untuk menyerang Ally index ke-{RandomChoice}!");
            }
            else
            {
                Debug.Log("[TurnManager] Musuh memutuskan untuk tetap menyerang Player!");
            }
        }
        
        enemyManager.EnemyAttackTarget(choosenTarget, () =>
        {
            handManager.ResetRoundPlays();
            DrawCardForNewTurn();

        });
        
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