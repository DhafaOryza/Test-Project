using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;

public class CardView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    //[SerializeField] private SpriteRenderer cardImage;
    [SerializeField] private Image cardImage;
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text description;
    [SerializeField] private TMP_Text cost; 
    [SerializeField] private TMP_Text damage; 
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private LayerMask dropZoneLayer;
    [SerializeField] private bool isInteractable = true;
    [SerializeField] private PlayerStats playerStats;

    public Card CardData => card;
    private Card card;
    private Camera mainCam;
    private Vector3 homePosition;
    private Quaternion homeRotation;

    /*private UnityEngine.Rendering.SortingGroup sortingGroup;
    private int originalSortingOrder;*/

    private Canvas cardCanvas;
    private SpriteRenderer[] allSprites;
    private int baseCanvasOrder;

    public System.Action<CardView> OnCardUsed;  // dipanggil HandManager buat hapus dari list & kurangi kesempatan
    public System.Action<CardView> OnCardDiscarded; // dipanggil HandManager buat hapus dari list TANPA kurangi kesempatan
    public System.Action<CardView> OnCardDefeated; // dipanggil kalau Health kartu ini habis (buat Enemy/Summon)
    public System.Action<CardView> OnCardSummoned;

    private void Awake()
    {
        mainCam = Camera.main;
        cardCanvas = GetComponentInChildren<Canvas>();
        allSprites = GetComponentsInChildren<SpriteRenderer>();

        //sortingGroup = GetComponent<UnityEngine.Rendering.SortingGroup>();
    }

    public void Setup(Card card)
    {
        this.card = card;

        if (card.Type == CardType.Summon || card.Type == CardType.Enemy)
        {
            card.ResetHealth();
        }
        
        cardImage.sprite = card.Sprite;
        title.text = card.Title;
        description.text = card.Description;
        damage.text = card.Damage.ToString();
        if (cost != null) cost.text = card.Cost.ToString();

        bool showsHealth = card.Type == CardType.Enemy || card.Type == CardType.Summon;
        if (healthText != null)
        {
            healthText.gameObject.SetActive(showsHealth);
            if (showsHealth) healthText.text = $"{card.CurrentHealth}";
        }
    }

    public void SetInteractable(bool value) => isInteractable = value;
    public void SetPlayerStats(PlayerStats stats) => playerStats = stats;

    public void SetHomeTransform(Vector3 position, Quaternion rotation)
    {
        homePosition = position;
        homeRotation = rotation;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isInteractable) return;
        transform.DOKill();

        foreach (var sprite in allSprites)
        {
            sprite.sortingOrder = 999;
        }

        if (cardCanvas != null)
        {
            baseCanvasOrder = cardCanvas.sortingOrder; 
            cardCanvas.overrideSorting = true;
            cardCanvas.sortingOrder = 1000;
        }

        /*if (sortingGroup != null)
        {
            originalSortingOrder = sortingGroup.sortingOrder;
            sortingGroup.sortingOrder =  999;
        }*/


        transform.DORotate(Vector3.zero, 0.15f);
        transform.DOScale(new Vector3(2.1f, 2.6f, 1.1f), 0.15f);
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
        Collider2D hit = Physics2D.OverlapPoint(worldPoint, dropZoneLayer);
        return hit != null ? hit.GetComponent<DropZone>() : null;
    }

    private bool IsValidDropForCardType(DropZoneType zoneType)
    {
        if (zoneType == DropZoneType.DiscardArea) return true; // semua tipe kartu boleh dibuang

        return card.Type switch
        {
            CardType.Attack or CardType.Debuff => zoneType == DropZoneType.EnemyArea,
            CardType.Buff or CardType.Summon => zoneType == DropZoneType.PlayEffectArea,
            _ => false
        };
    }

   private void ResolveCardEffect(DropZone zone)
    {
        if (zone.ZoneType == DropZoneType.DiscardArea)
        {
            ExecuteDiscardVisual(zone);
            OnCardUsed?.Invoke(this);
            return;
        }

        bool effectApplied = false;

        switch (card.Type)
        {
            case CardType.Attack:
            case CardType.Debuff:
                if (zone.EnemyCardView != null && zone.EnemyCardView.CardData.IsAlive)
                {
                    zone.EnemyCardView.ReceiveDamage(card.Damage);
                    effectApplied = true;
                }
                break;

            case CardType.Buff:
                playerStats?.Heal(card.EffectAmount);
                effectApplied = true;
                break;

            case CardType.Summon:
                effectApplied = true;
                break;
        }

        if (effectApplied)
        {
            Debug.Log($"{card.Title} digunakan ({card.Type})");
            
           
            if (card.Type == CardType.Summon)
            {
                ExecuteSummonVisual();
                OnCardSummoned?.Invoke(this);
            }
            else
            {
                DropZone discardZone = GetDiscardZone();
                if (discardZone != null) 
                {
                    ExecuteDiscardVisual(discardZone);
                }
            }
            
            OnCardUsed?.Invoke(this); 
        }
        else
        {
            ReturnToHand();
        }
    }

    private void ExecuteSummonVisual()
    {
        isInteractable = false; 
        
       
        homePosition = transform.position;
        homeRotation = transform.rotation;

        
        if (allSprites != null)
        {
            foreach(var sr in allSprites) sr.sortingOrder = 1;
        }
        if (cardCanvas != null)
        {
            cardCanvas.sortingOrder = baseCanvasOrder;
        }
        
        // (Opsional) Beri sedikit efek debu/hentakan saat Summon turun ke meja
        transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0f), 0.3f, 5);
    }

    private void ExecuteDiscardVisual(DropZone discardZone)
    {
        isInteractable = false;
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        discardZone.DiscardedVisuals.Add(this);

        foreach (var stackedCard in discardZone.DiscardedVisuals)
        {
            if (stackedCard != this && stackedCard != null)
            {
                Canvas stackedCanvas = stackedCard.GetComponentInChildren<Canvas>();
                if (stackedCanvas != null)
                {
                    stackedCanvas.enabled = false;
                }
            }
        }

        if (discardZone.DiscardedVisuals.Count > 2)
        {
            CardView oldestCard = discardZone.DiscardedVisuals[0];
            if (oldestCard != null)
            {
                Destroy(oldestCard.gameObject);
            }
            discardZone.DiscardedVisuals.RemoveAt(0);
        }

        int stackIndex = discardZone.DiscardedVisuals.Count - 1;
        Vector3 offset = new Vector3(0.05f * stackIndex, 0.05f * stackIndex, -0.1f * stackIndex);
        Vector3 targetPos = discardZone.transform.position + offset;


        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
        foreach(var sr in sprites) sr.sortingOrder = stackIndex;

        Canvas canvas = GetComponentInChildren<Canvas>();
        if (canvas != null)
        {
            canvas.overrideSorting = true;
            canvas.sortingOrder = stackIndex + 1; 
        }
        /*
        UnityEngine.Rendering.SortingGroup sg = GetComponent<UnityEngine.Rendering.SortingGroup>();
        if (sg != null)
        {
            sg.sortingOrder = stackIndex;
        }*/

        transform.DOMove(targetPos, 0.3f).SetEase(Ease.InOutQuad).SetLink(gameObject);
        transform.DOLocalRotateQuaternion(discardZone.transform.rotation, 0.3f).SetLink(gameObject);
        //transform.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.3f).SetLink(gameObject);
    }

    // Fungsi pembantu untuk mencari DiscardArea
    private DropZone GetDiscardZone()
    {
        DropZone[] zones = FindObjectsOfType<DropZone>();
        foreach (var z in zones)
        {
            if (z.ZoneType == DropZoneType.DiscardArea) return z;
        }
        return null;
    }

    public void ReceiveDamage(int amount)
    {
        card.TakeDamage(amount);

        if (healthText != null)
            healthText.text = $"{card.CurrentHealth}";

        //Debug.Log($"{card.Title} menerima {amount} damage, sisa HP: {card.CurrentHealth}");

        if (!card.IsAlive)
        {
            OnCardDefeated?.Invoke(this);
            //Destroy(gameObject);
        }
        else
        {
            if (allSprites != null && allSprites.Length > 0)
            {
                allSprites[0].DOColor(Color.red, 0.15f).OnComplete(() => 
                {
                    allSprites[0].DOColor(Color.white, 0.15f); // Kembalikan ke warna asli
                });
            

            transform.DOShakePosition(0.25f, strength: new Vector3(0.3f, 0.3f, 0),vibrato: 15).SetLink(gameObject);
            }
        }
    }

    private void ReturnToHand()
    {
        transform.DOMove(homePosition, 0.25f).SetLink(gameObject).OnComplete(() =>
        {
            foreach(var sr in allSprites) sr.sortingOrder = 0;
            if (cardCanvas != null) cardCanvas.sortingOrder = baseCanvasOrder;
            
            /*if (sortingGroup != null)
            {
                sortingGroup.sortingOrder = originalSortingOrder;
            }*/
        });
        transform.DOLocalRotateQuaternion(homeRotation, 0.25f).SetLink(gameObject);
        transform.DOScale(new Vector3(2f, 2.5f, 1f), 0.25f).SetLink(gameObject);
    }
}