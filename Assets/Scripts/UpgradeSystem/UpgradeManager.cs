using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private Transform cardContainer;
    [SerializeField] private GameObject upgradeCardPrefab;

    [Header("Gacha Settings")]
    [SerializeField] private int choicesCount = 3;
    [SerializeField] private List<UpgradeData> allUpgrades;

    public void Initialize()
    {
        Debug.Log("Upgrademanager Initialize");
        if (upgradePanel != null)
        {
            upgradePanel.SetActive(false);
        }
    }

    public void TriggerLevelUp()
    {
        Time.timeScale = 0f;
        upgradePanel.SetActive(true);
        RollUpgrades();
    }

    public void RollUpgrades()
    {
        foreach (Transform child in cardContainer)
        {
            Destroy(child.gameObject);
        }

        List<UpgradeData> pool = new List<UpgradeData>(allUpgrades);
        int actualChoices = Mathf.Min(choicesCount, pool.Count);

        for (int i = 0; i < actualChoices; i++)
        {
            int randomIndex = Random.Range(0, pool.Count);
            UpgradeData chosenUpgrade = pool[randomIndex];
            pool.RemoveAt(randomIndex);

            GameObject cardObj = Instantiate(upgradeCardPrefab, cardContainer, false);
            
            UpgradeCardUI cardUI = cardObj.GetComponent<UpgradeCardUI>();
            if (cardUI != null)
            {
                cardUI.SetupCard(chosenUpgrade);
            }
        }
    }

    public void ApplyUpgrade(UpgradeData data)
    {
        data.ApplyEffect();
        
        upgradePanel.SetActive(false);
        Time.timeScale = 1f; 
    }
}