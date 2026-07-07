using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;

public class CardView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image cardImage;
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text description;
    [SerializeField] private TMP_Text cost; 
    [SerializeField] private TMP_Text damage; 
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private LayerMask dropZoneLayer;
    [SerializeField] private bool isInteractable = true;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private BuffEffectType EffectType;

    public Card CardData => card;
    private Card card;
    private Camera mainCam;
    private Vector3 homePosition;
    private Quaternion homeRotation;

    private Canvas cardCanvas;
    private SpriteRenderer[] allSprites;
    private int baseCanvasOrder;

    public System.Action<CardView> OnCardUsed;  
    public System.Action<CardView> OnCardDiscarded; 
    public System.Action<CardView> OnCardDefeated; 
    public System.Action<CardView> OnCardSummoned;
    public System.Action<int> OnCardDrawTriggered;

    private void Awake()
    {
        mainCam = Camera.main;
        cardCanvas = GetComponentInChildren<Canvas>();
        allSprites = GetComponentsInChildren<SpriteRenderer>();
    }

    public void Setup(Card card)
    {
        this.card = card;

        if (card.Type == CardType.Summon || card.Type == CardType.Enemy)
        {
            card.ResetHealth();
        }
        
        if (card.Sprite == null)
        {
            cardImage.color = new Color(1, 1, 1, 0); 
        }
        else
        {
            cardImage.color = new Color(1, 1, 1, 1);
            cardImage.sprite = card.Sprite;
        }
        // ------------------------

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
        if (zoneType == DropZoneType.DiscardArea) return true; 

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
                if (zone.EnemyCardView != null && zone.EnemyCardView.CardData.IsAlive)
                {
                    zone.EnemyCardView.ReceiveDamage(card.Damage);
                    effectApplied = true;
                }
                break;
            case CardType.Debuff:
                if (zone.EnemyCardView != null && zone.EnemyCardView.CardData.IsAlive)
                {
                    if (card.Effect == "AttackDown")
                    {
                        zone.EnemyCardView.ReceiveAttackDebuff(card.EffectAmount);
                        zone.EnemyCardView.ReceiveDamage(card.Damage);
                        effectApplied = true; 
                    }
                    else
                    {
                        
                    }
                }
                break;

            case CardType.Buff:
            if (card.EffectType == BuffEffectType.HealthBoost)
                {
                    if (playerStats != null && !playerStats.isHealthFull())
                    {
                        playerStats.Heal(card.EffectAmount);
                        effectApplied = true;
                    }
                    else
                    {
                        effectApplied = false;
                    }
                }

                else if (card.EffectType == BuffEffectType.Shield)
                {
                    playerStats?.AddShield(card.EffectAmount);
                    effectApplied = true;
                }

                else if (card.EffectType == BuffEffectType.DrawExtraCard)
                {
                    OnCardDrawTriggered?.Invoke(card.EffectAmount);
                    effectApplied = true;
                }

                break;
            case CardType.Summon:
                effectApplied = true;
                break;
        }

        if (effectApplied)
        {
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

        transform.DOMove(targetPos, 0.3f).SetEase(Ease.InOutQuad).SetLink(gameObject);
        transform.DOLocalRotateQuaternion(discardZone.transform.rotation, 0.3f).SetLink(gameObject);
    }

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

        if (!card.IsAlive)
        {
            OnCardDefeated?.Invoke(this);
        }
        else
        {
            if (cardImage != null)
            {
                cardImage.DOColor(Color.red, 0.15f).OnComplete(() => 
                {
                    cardImage.DOColor(Color.white, 0.15f); 
                });
            }
            
            transform.DOShakePosition(0.25f, strength: new Vector3(0.3f, 0.3f, 0),vibrato: 15).SetLink(gameObject);
        }
    }

    public void ReceiveAttackDebuff(int percentage)
    {
        int ReductionAmount = Mathf.RoundToInt(card.Damage * percentage / 100f);
        card.Damage -= ReductionAmount;

        if (card.Damage < 2)
        {
            card.Damage = 2;
        }

        if (damage != null)
        {
            damage.text = card.Damage.ToString();
        }

        if (card.Damage != null)
        {
            cardImage.DOColor(new Color(0.6f, 0.2f, 0.8f), 0.3f).OnComplete(() =>
            {
                cardImage.DOColor(Color.white, 0.3f);
            });
        }

        transform.DOPunchScale(new Vector3(-0.1f, -0.1f, 0), 0.3f, 5).SetLink(gameObject);
    }

    private void ReturnToHand()
    {
        transform.DOMove(homePosition, 0.25f).SetLink(gameObject).OnComplete(() =>
        {
            foreach(var sr in allSprites) sr.sortingOrder = 0;
            if (cardCanvas != null) cardCanvas.sortingOrder = baseCanvasOrder;
        });
        transform.DOLocalRotateQuaternion(homeRotation, 0.25f).SetLink(gameObject);
        transform.DOScale(new Vector3(2f, 2.5f, 1f), 0.25f).SetLink(gameObject);
    }
}