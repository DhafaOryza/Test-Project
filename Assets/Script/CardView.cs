using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CardView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private SpriteRenderer cardImage;
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text cost;
    [SerializeField] private LayerMask dropZoneLayer;

    private Card card;
    private Camera mainCam;
    private Vector3 homePosition;
    private Quaternion homeRotation;

    public System.Action<CardView> OnCardUsed; // dipanggil HandManager buat hapus dari list & kurangi kesempatan

    private void Awake()
    {
        mainCam = Camera.main;
    }

    public void Setup(Card card)
    {
        this.card = card;
        cardImage.sprite = card.Sprite;
        title.text = card.Title;
        cost.text = card.Cost.ToString();
    }

    // Dipanggil HandManager setiap kali posisi tangan di-update, supaya kartu tau harus "pulang" kemana
    public void SetHomeTransform(Vector3 position, Quaternion rotation)
    {
        homePosition = position;
        homeRotation = rotation;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.DOKill();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 worldPoint = mainCam.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, mainCam.WorldToScreenPoint(transform.position).z));
        transform.position = worldPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
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

        Debug.Log($"{card.Title} digunakan ({card.Type})");
        OnCardUsed?.Invoke(this); // HandManager yang handle hapus dari hand + kurangi kesempatan
        Destroy(gameObject);
    }

    public void ReceiveDamage(int amount)
    {
        card.TakeDamage(amount);
        // TODO: update UI health kalau card ini Enemy/Summon
        if (!card.IsAlive)
            Destroy(gameObject);
    }

    private void ReturnToHand()
    {
        transform.DOMove(homePosition, 0.25f);
        transform.DORotateQuaternion(homeRotation, 0.25f);
    }
}