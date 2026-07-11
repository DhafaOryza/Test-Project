using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

public class ChoiceCardSlot : MonoBehaviour, IPointerClickHandler , IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI Components")]
    [SerializeField] private Image cardImage;  
    [SerializeField] private TMP_Text title;              
    [SerializeField] private TMP_Text description;        
    [SerializeField] private TMP_Text damage;             
    [SerializeField] private TMP_Text healthText;         
    [SerializeField] private TMP_Text cost;               

    private CardData currentData;
    private System.Action<CardData> onChosenCallback;

    public void Setup(CardData data, System.Action<CardData> onChosen)
    {
        currentData = data;
        onChosenCallback = onChosen;

        gameObject.SetActive(data != null);
        if (data == null) return;

        // UI Image juga menggunakan .sprite, jadi kodenya sama persis!
        cardImage.sprite = data.Sprite;
        title.text = data.Title;
        description.text = data.Description;

        if (damage != null) damage.text = data.Damage.ToString();
        if (cost != null) cost.text = data.Cost.ToString();

        // Logika untuk menampilkan Health jika tipe kartu Summon atau Enemy
        bool showsHealth = data.Type == CardType.Enemy || data.Type == CardType.Summon;
        if (healthText != null)
        {
            healthText.gameObject.SetActive(showsHealth);
            if (showsHealth) healthText.text = data.Health.ToString(); // Menggunakan base Health dari CardData, ini aman!
        }

        transform.localScale = Vector3.one;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentData == null) return;

        transform.localScale = Vector3.one;
        onChosenCallback?.Invoke(currentData);
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        if (currentData != null)
        {
            transform.DOScale(Vector3.one  * 1.1f, 0.2f).SetEase(Ease.OutBack).SetLink(gameObject);
        }
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
       if (currentData != null)
        {
            transform.DOScale(Vector3.one , 0.2f).SetEase(Ease.OutQuad).SetLink(gameObject);
        }
    }
}