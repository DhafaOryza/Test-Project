using UnityEngine;

public class TurnManager : MonoBehaviour
{
    

    [SerializeField] private ChanceUI chanceUI;
    [SerializeField] private float delayBeforeTurnTranstition = 1f;
    [SerializeField] private int cardsPerTurn = 3;

    public void Initialize()
    {
        GameManager.Instance.handManager.OnPlaysChanged += chanceUI.SetChanceUI;
        GameManager.Instance.handManager.OnPlaysExhausted += HandleAllyTurn;
    }

    private void OnDisable()
    {
        GameManager.Instance.handManager.OnPlaysChanged -= chanceUI.SetChanceUI;
        GameManager.Instance.handManager.OnPlaysExhausted -= HandleAllyTurn;
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
        if (GameManager.Instance.allyManager == null || GameManager.Instance.allyManager.ActiveAllies.Count == 0)
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
        if (index >= GameManager.Instance.allyManager.ActiveAllies.Count)
        {
            Debug.Log("[TurnManager] Semua Ally selesai menyerang. Sekarang giliran Musuh!");
            ExecuteEnemyTurn();
            return;
        }

        CardView currentAlly = GameManager.Instance.allyManager.ActiveAllies[index];
        // Pilih target musuh pertama yang masih hidup
        Transform enemyTarget = GameManager.Instance.enemyManager.ActiveEnemies.Count > 0 ? GameManager.Instance.enemyManager.ActiveEnemies[0].transform : null;

        if (currentAlly != null && enemyTarget != null)
        {
            GameManager.Instance.allyManager.AllyAttackTarget(currentAlly, enemyTarget, () =>
            {
                PlayAllyAttack(index + 1);
            });
        }
        else
        {
            PlayAllyAttack(index + 1);
        }
    }

    private void ExecuteEnemyTurn()
    {
        Debug.Log("[TurnManager] Giliran Pasukan Musuh dimulai!");
        PlayEnemyAttack(0); // Mulai dari musuh index ke-0
    }

    // Fungsi antrean keroyokan musuh
    private void PlayEnemyAttack(int index)
    {
        // Jika sudah semua musuh menyerang, kembalikan giliran ke Player
        if (GameManager.Instance.enemyManager.ActiveEnemies == null || index >= GameManager.Instance.enemyManager.ActiveEnemies.Count)
        {
            //Debug.Log("[TurnManager] Semua Musuh selesai menyerang. Kembali ke giliran Player!");
            GameManager.Instance.handManager.ResetRoundPlays();
            DrawCardForNewTurn();
            return;
        }

        CardView currentEnemy = GameManager.Instance.enemyManager.ActiveEnemies[index];
        Transform choosenTarget = GameManager.Instance.playerStats.transform;

        // Logika Random Target
        if (GameManager.Instance.allyManager != null && GameManager.Instance.allyManager.ActiveAllies.Count > 0)
        {
            int RandomChoice = Random.Range(0, GameManager.Instance.allyManager.ActiveAllies.Count + 1);
            if (RandomChoice < GameManager.Instance.allyManager.ActiveAllies.Count)
            {
                choosenTarget = GameManager.Instance.allyManager.ActiveAllies[RandomChoice].transform;
                //Debug.Log($"[TurnManager] Musuh '{currentEnemy.CardData.Title}' menyerang Ally index ke-{RandomChoice}!");
            }
            else
            {
                //Debug.Log($"[TurnManager] Musuh '{currentEnemy.CardData.Title}' menyerang Player!");
            }
        }
        
        // Eksekusi serangan musuh ini, lalu panggil diri sendiri (index + 1)
        GameManager.Instance.enemyManager.EnemyAttackTarget(currentEnemy, choosenTarget, () =>
        {
            PlayEnemyAttack(index + 1);
        });
    }

    private void DrawCardForNewTurn()
    {
        for (int i = 0; i < cardsPerTurn; i++)
        {
            GameManager.Instance.deckManager.DrawCard();
        }
    }

    public void EndTurnButtonPressed()
    {
        GameManager.Instance.handManager.ForceEndTurn();
    }
}