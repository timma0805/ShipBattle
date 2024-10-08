using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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

    private LayoutElement layoutElement;
    private Button button;

    private Card cardData;
    private UIBattleCardsPanel cardPanel;
    private RectTransform rectTransform;
    private Quaternion originalRotation;

    //Setting
    private const float shakeDuration = 0.5f;
    private const float shakeIntensity = 1.0f;

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

        bgImg.sprite = card.data.texture;
        nameTxt.text = card.data.name;
        costTxt.text = card.data.cost.ToString();
        typeTxt.text = card.GetCardTypeName();
        detailTxt.text = card.GetCardDetailString();
    }

    public async Task ShowCard()
    {
        Debug.Log("Start ShowCard");

        layoutElement.ignoreLayout = false;
        this.gameObject.SetActive(true);
    }

    public void Unvisible()
    {
        Debug.Log("Unvisible");

        if(layoutElement == null)
            layoutElement = GetComponent<LayoutElement>();
        
        layoutElement.ignoreLayout = true;

        this.gameObject.SetActive(false);
    }

    private  void onClickCard()
    {
        Debug.Log("ClickCard: " + cardData.data.cardIndex);
    }

    protected override void OnPointerDown()
    {
        if (!canDragging)
            return;

        cardPanel.ShowEffectRange(cardData);
        layoutElement.ignoreLayout = true;
    }

    protected override void OnPointerUp()
    {
        if (!canDragging)
            return;

        layoutElement.ignoreLayout = false;
        cardPanel.TryToUseCard(cardData);
    }

    public void CheckCanUseOrNot()
    {
        canDragging = cardPanel.CheckCardCondition(cardData);
        Debug.Log("canDragging: " + canDragging);
        if (!canDragging)
        {
            if(shakeCoroutine == null)
                shakeCoroutine = StartCoroutine(ShakeAnimation());
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        CheckCanUseOrNot();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }

    public IEnumerator ShakeAnimation()
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
