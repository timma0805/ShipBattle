using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Reflection;

[RequireComponent(typeof(LayoutElement))]
[RequireComponent(typeof(Button))]

public class UIBattleCard : UIDragObject, IPointerEnterHandler, IPointerExitHandler
    {
    [SerializeField]
    private Image bgImg;
    [SerializeField]
    private TMP_Text nameTxt;
    [SerializeField]
    private TMP_Text costTxt;
    [SerializeField]
    private TMP_Text typeTxt;
    [SerializeField]
    private TMP_Text detailTxt;
    [SerializeField]
    private TMP_Text occupationTxt;
    [SerializeField]
    private Sprite[] bgTypeSprite;
    [SerializeField]
    private Image arrowImg;
    [SerializeField]
    private Sprite[] arrowSprite;
    [SerializeField]
    private TMP_Text valueTxt;
    [SerializeField]
    private TMP_Text countdownTxt;
    [SerializeField]
    private Image startTile;
    [SerializeField]
    private Image[] tileList;

    private LayoutElement layoutElement;
    private Button button;

    public Card cardData { get; private set; }
    private UIBattleCardsPanel cardPanel;
    private RectTransform rectTransform;
    private Quaternion originalRotation;

    //Setting
    private const float shakeDuration = 0.5f;
    private const float shakeIntensity = 1.0f;
    private int startTileIndex  = -1;
    private int tileWidth;

    private Coroutine shakeCoroutine;

    void Awake()
    {
        // Get the RectTransform component of the UI image
        rectTransform = GetComponent<RectTransform>();
        // Save the original rotation for resetting after the shake
        originalRotation = rectTransform.rotation;

        layoutElement = GetComponent<LayoutElement>();
        button = GetComponent<Button>();
    }

    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener(onClickCard);     
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public void CreateCard(UIBattleCardsPanel _cardPanel, Card card)
    {
        cardPanel = _cardPanel;
        UpdateCardData(card);
    }

    public void UpdateCardData(Card card)
    {
        cardData = card;

        bgImg.sprite = bgTypeSprite[(int)card.CardData.Type];
        nameTxt.text = card.CardData.Name;
        costTxt.text = card.CardData.Cost.ToString();
        typeTxt.text = card.GetCardTypeName();
        detailTxt.text = card.GetCardDetailString();
        occupationTxt.text = card._characterData.Name;
        countdownTxt.text = card.CardData.Countdown.ToString();
        arrowImg.sprite = arrowSprite[(int)card.CardData.Type];

        if (!card.CardData.isCombined)
        {
            CardEffect cardEffect = card.CardData.EffectList[0];
            valueTxt.text = cardEffect.Value.ToString();

            if (cardEffect.Direction != FaceDirection.NA)
            {
                if (cardEffect.Direction == FaceDirection.Back)
                    arrowImg.transform.Rotate(Vector3.forward, 180);
                else if (cardEffect.Direction == FaceDirection.Up)
                    arrowImg.transform.Rotate(Vector3.forward, 90);
                else if (cardEffect.Direction == FaceDirection.Down)
                    arrowImg.transform.Rotate(Vector3.forward, 270);
            }
            else
            {
                arrowImg.sprite = arrowSprite[0];
            }

            for (int i = 0; i < tileList.Length; i++)
            {
                if (i == startTileIndex)
                    tileList[i].color = Color.black;
                else
                    tileList[i].color = Color.white;
            }

            if (cardEffect.PosList.Count == 0)
            {
                startTile.color = Color.red;
            }
            else
            {
                if (startTileIndex == -1)
                {
                    startTileIndex = Array.IndexOf(tileList, startTile);
                    tileWidth = startTile.transform.parent.GetComponent<GridLayoutGroup>().constraintCount;
                }

                for (int i = 0; i < cardEffect.PosList.Count; i++)
                {
                    Vector2 vector = cardEffect.PosList[i];
                    int index = startTileIndex;
                    index += (int)vector.x;
                    index += (int)vector.y * tileWidth;
                    if (index >= 0 && index < tileList.Length)
                        tileList[index].color = Color.red;
                    else
                        Debug.LogError($"Card {cardData.CardData.Name} Tile Out of Range");
                }
            }
        }
        else //TODO: Show combined card detail
        {

        }
    }

    public async Task ShowCard()
    {
        Debug.Log("Start ShowCard");

        canDragging = false;
        layoutElement.ignoreLayout = false;
        this.gameObject.SetActive(true);
        canDragging = true;
    }

    public void Invisible()
    {
        Debug.Log("Invisible");

        if(layoutElement == null)
            layoutElement = GetComponent<LayoutElement>();
        
        layoutElement.ignoreLayout = true;

        this.gameObject.SetActive(false);
    }

    private  void onClickCard()
    {
        Debug.Log("ClickCard: " + cardData.CardData.ID);
    }

    protected override void OnPointerDown()
    {
        if (!canDragging)
            return;

        if (cardPanel != null)
        {
            cardPanel.ShowEffectRange(cardData);
            layoutElement.ignoreLayout = true;
        }
    }

    protected override void OnPointerUp()
    {
        if (!canDragging)
            return;
        if (cardPanel != null)
        {
            layoutElement.ignoreLayout = false;
            cardPanel.TryToUseCard(cardData);
        }
    }

    public void ShakeAnimation()
    {
        if (shakeCoroutine == null)
            shakeCoroutine = StartCoroutine(ShakeAnimationIE());
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        canDragging = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }

    public IEnumerator ShakeAnimationIE()
    {
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            // Generate a random rotation offset within the specified intensity
            float randomOffset = UnityEngine.Random.Range(-shakeIntensity, shakeIntensity);
            Quaternion rotationOffset = Quaternion.Euler(0f, 0f, randomOffset);

            // Apply the rotation offset to the UI image
            rectTransform.rotation = originalRotation * rotationOffset;

            // Increment the elapsed time
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // Reset the UI image rotation to its original rotation after the shake
        rectTransform.rotation = originalRotation;

        shakeCoroutine = null;
    }
}
