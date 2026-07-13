using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;
using Unity.VisualScripting;

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
    [SerializeField] private BuffEffectType EffectType;

    public BaseCard CardData => card;
    private BaseCard card;
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

    public void Setup(BaseCard newCard)
    {
        this.card = newCard;
        
        if (card.Sprite == null)
        {
            cardImage.color = new Color(1, 1, 1, 0); 
        }
        else
        {
            cardImage.color = new Color(1, 1, 1, 1);
            cardImage.sprite = card.Sprite;
        }

        title.text = card.Title;
        description.text = card.Description;
        if (cost != null) cost.gameObject.SetActive(false);

        if (card is AttackCard atkCard)
        {
            damage.gameObject.SetActive(true);
            damage.text = atkCard.Damage.ToString();
        }
        else if (card is SummonCard summonCardforDmg)
        {
            damage.gameObject.SetActive(true);
            damage.text = summonCardforDmg.Damage.ToString();
        }
        else
        {
            damage.gameObject.SetActive(false);
        }

        if (card is SummonCard summonCard)
        {
            healthText.gameObject.SetActive(true);
            healthText.text = summonCard.currentHealth.ToString();
            summonCard.ResetHealth();
        }
        else
        {
            healthText.gameObject.SetActive(false);
        }
    }

    public void SetInteractable(bool value) => isInteractable = value;
    public void SetPlayerStats(PlayerStats stats) => GameManager.Instance.playerStats = stats;

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
            OnCardUsed?.Invoke(this);
            return;
        }

        bool effectApplied = card.ResolveEffect(zone);
        if (effectApplied)
        {
            if (card is SummonCard)
            {
                ExecuteSummonVisual();
                OnCardSummoned?.Invoke(this);
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
        if (card is SummonCard summonCard)
        {
            summonCard.TakeDamage(amount);
            if(healthText != null)
            {
                healthText.text = summonCard.currentHealth.ToString();
            }

            if (!summonCard.IsAlive)
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
    }

    public void ReceiveAttackDebuff(int percentage)
    {
        int currentDamage = 0;

        if (card is AttackCard atkCard)
        {
            int ReductionAmount = Mathf.RoundToInt(atkCard.Damage * percentage / 100f);
            atkCard.Damage -= ReductionAmount;
            if (atkCard.Damage < 2) atkCard.Damage = 2;
            currentDamage = atkCard.Damage;
        }
        else if (card is SummonCard summonCard)
        {
            int ReductionAmount = Mathf.RoundToInt(summonCard.Damage * percentage / 100f);
            summonCard.Damage -= ReductionAmount;
            if (summonCard.Damage < 2) summonCard.Damage = 2;
            currentDamage = summonCard.Damage;
        }
        else
        {
            return;
        }

        if (damage != null) damage.text = currentDamage.ToString();

        if (cardImage != null)
        {
            cardImage.DOColor(new Color(0.6f, 0.2f, 0.8f), 0.3f).OnComplete(() => cardImage.DOColor(Color.white, 0.3f));
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

    public void SetStackCount (int count)
    {
        if (cost != null)
        {
            cost.gameObject.SetActive(true);
            cost.text = $"{count}x";
        }
        else
        {
            cost.gameObject.SetActive(false);
        }
    }
}