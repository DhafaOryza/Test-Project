using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CardView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private SpriteRenderer cardImage;
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text Description;
    [SerializeField] private TMP_Text healthText; // opsional, cuma dipakai kalau Type = Enemy/Summon
    [SerializeField] private TMP_Text cost; // opsional, cuma dipakai kalau Type = Attack/Buff/Debuff
    [SerializeField] private TMP_Text damage; // opsional, cuma dipakai kalau Type = Attack/Debuff/Summon
    [SerializeField] private LayerMask dropZoneLayer;
    [SerializeField] private bool isInteractable = true;

    public Card CardData => card;
    private Card card;
    private Camera mainCam;
    private Vector3 homePosition;
    private Quaternion homeRotation;

    public System.Action<CardView> OnCardUsed;  // dipanggil HandManager buat hapus dari list & kurangi kesempatan
    public System.Action<CardView> OnCardDefeated; // dipanggil kalau Health kartu ini habis (buat Enemy/Summon)

    private void Awake()
    {
        mainCam = Camera.main;
    }

    public void Setup(Card card)
    {
        this.card = card;
        cardImage.sprite = card.Sprite;
        title.text = card.Title;
        Description.text = card.Description;
        damage.text = card.Damage.ToString();

        if (cost != null)
        {
            cost.text = card.Cost.ToString();
        }

        bool showsHealth = card.Type == CardType.Enemy || card.Type == CardType.Summon;
        if (healthText != null)
        {
            healthText.gameObject.SetActive(showsHealth);
            if (showsHealth) healthText.text = $"{card.CurrentHealth}/{card.MaxHealth}";
        }
    }

    public void SetInteractable(bool value) => isInteractable = value;

    // Dipanggil HandManager setiap kali posisi tangan di-update, supaya kartu tau harus "pulang" kemana
    public void SetHomeTransform(Vector3 position, Quaternion rotation)
    {
        homePosition = position;
        homeRotation = rotation;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isInteractable) return;
        transform.DOKill();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isInteractable) return;
        Vector3 worldPoint = mainCam.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, mainCam.WorldToScreenPoint(transform.position).z));
        transform.position = worldPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isInteractable) return;
        DropZone zone = FindDropZoneUnderPointer();

        if (zone == null)
        {
            ReturnToHand();
            return;
        }

        bool valid = IsValidDropForCardType(zone.ZoneType);
        if (!valid)
        {
            ReturnToHand();
            return;
        }

        ResolveCardEffect(zone);
    }


    private DropZone FindDropZoneUnderPointer()
    {
        Vector2 worldPoint = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Debug.DrawRay(mainCam.ScreenToWorldPoint(Input.mousePosition), Vector3.forward * 10f, Color.red, 1f);
        Collider2D hit = Physics2D.OverlapPoint(worldPoint, dropZoneLayer);
        return hit != null ? hit.GetComponent<DropZone>() : null;
    }

    private bool IsValidDropForCardType(DropZoneType zoneType)
    {
        return card.Type switch
        {
            CardType.Attack or CardType.Debuff or CardType.Summon => zoneType == DropZoneType.EnemyArea,
            CardType.Buff => zoneType == DropZoneType.PlayEffectArea,
            _ => false
        };
    }

    private void ResolveCardEffect(DropZone zone)
    {
        switch (card.Type)
        {
            case CardType.Attack:
            case CardType.Debuff:
                if (zone.EnemyCardView != null)
                    zone.EnemyCardView.ReceiveDamage(card.Damage);
                break;
            case CardType.Buff:
                // TODO: panggil sistem buff ke player/tim
                break;
            case CardType.Summon:
                // TODO: spawn ally card di board
                break;
        }

    
        OnCardUsed?.Invoke(this); // HandManager yang handle hapus dari hand + kurangi kesempatan
        Destroy(gameObject);
    }

    public void ReceiveDamage(int amount)
    {
        card.TakeDamage(amount);

        if (healthText != null)
            healthText.text = $"{card.CurrentHealth}/{card.MaxHealth}";

        Debug.Log($"{card.Title} menerima {amount} damage, sisa HP: {card.CurrentHealth}");

        if (!card.IsAlive)
        {
            OnCardDefeated?.Invoke(this);
            Destroy(gameObject);
        }
    }

    private void ReturnToHand()
    {
        transform.DOMove(homePosition, 0.25f);
        transform.DOLocalRotateQuaternion(homeRotation, 0.25f);
    }
}