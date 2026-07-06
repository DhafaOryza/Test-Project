using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeCardUI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Button cardButton;

    private UpgradeData assignedUpgrade;

    public void SetupCard(UpgradeData data)
    {
        assignedUpgrade = data;
        
        nameText.text = data.upgradeName;
        descriptionText.text = data.description;
        if (data.icon != null) iconImage.sprite = data.icon;

        cardButton.onClick.RemoveAllListeners();
        cardButton.onClick.AddListener(SelectThisUpgrade);
    }

    private void SelectThisUpgrade()
    {
        UpgradeManager.Instance.ApplyUpgrade(assignedUpgrade);
    }
}