using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBattleCardsPanel : MonoBehaviour
{
    private Camera uiCamera;
    [SerializeField]
    private GameObject cardPrefab;

    [SerializeField]
    private GameObject showCardsContent;
    private GridLayoutGroup cardsLayoutGroup;
    public RectTransform cardDragAreaRectTrans;

    [SerializeField]
    private TMP_Text mpTxt;
    [SerializeField]
    private TMP_Text stackCountTxt;
    [SerializeField]
    private TMP_Text usedCountTxt;
    [SerializeField]
    private TMP_Text discardCountTxt;

    [SerializeField]
    private List<UIBattleCard> cardList;
    [SerializeField]
    private List<Card> currentCardList;
    [SerializeField]
    private List<Card> usedCardList;
    [SerializeField]
    private List<Card> discardCardList;
    [SerializeField]
    private List<Card> cardStackList;

    private MiniBattleCoreController battleController;
    private bool isProcessingCard = false;

    //Setting
    private int cardAvailableSlot = 10;
    private int drawInitCardAvailableCount = 5;
    private int drawCardAvailableCount = 3;
    private int redrawUsedCardsCount = 0;

    // Start is called before the first frame update
    void Awake()
    {
        uiCamera = BattleGameCoreController.Instance.GetUICamera();
        cardsLayoutGroup = showCardsContent.GetComponent<GridLayoutGroup>();
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void Init(MiniBattleCoreController _battleController)
    {
        battleController = _battleController;   
    }

    public void StartBattle(List<Card> cardDatas)
    {
        InitialCardStack(cardDatas);
        UpdateCardCount();
    }

    private void InitialCardStack(List<Card> cards)
    {
        //Random Shuffle cardstack
        cards = ShuffleGOList(cards);

        cardStackList = cards;
        if(cardList == null)
            cardList = new List<UIBattleCard>();
        else
        {
            for (int i = 0;i < cardList.Count;i++)
            {
                cardList[i].Invisible();
            }
        }
        usedCardList = new List<Card>();
        discardCardList = new List<Card>();
        currentCardList = new List<Card>();
        redrawUsedCardsCount = 0;
    }

    private List<T> ShuffleGOList<T>(List<T> inputList)
    {    //take any list of GameObjects and return it with Fischer-Yates shuffle
        int i = 0;
        int t = inputList.Count;
        int r = 0;
        T p;
        List<T> tempList = new List<T>();
        tempList.AddRange(inputList);

        while (i < t)
        {
            r = Random.Range(i, tempList.Count);
            p = tempList[i];
            tempList[i] = tempList[r];
            tempList[r] = p;
            i++;
        }

        return tempList;
    }

    private void AddCardToStack(Card card, bool isRandom = false)
    {
        if(!isRandom)   //add to the last index
            cardStackList.Add(card);
        else
        {
            int index = Random.Range(0, cardStackList.Count);
            Card swapCard = cardStackList[index];
            cardStackList.Add(swapCard);
            cardStackList[index] = card;
        }
    }

    public int GetCurrentCardListCount()
    {
        return currentCardList.Count;
    }

    public async Task DrawCard()
    {
        Debug.Log("DrawCard");
        if (currentCardList.Count == 0 && usedCardList.Count == 0)// just start
        {
            await DrawCards(drawInitCardAvailableCount);
        }
        else
        {
            await DrawCards(drawCardAvailableCount);
        }
    }

    private async Task DrawCards(int count)
    {
        Debug.Log("DrawCards" + count);
        for (int i = 0; i < count; i++)
        {
            if(cardStackList.Count == 0)
            {
                Debug.Log("cardStackList Empty, Refresh");
                if(usedCardList.Count == 0)
                {
                    Debug.Log("usedCardList Empty, All cards hold on hand");
                    return;
                }

                redrawUsedCardsCount++;
                cardStackList = ShuffleGOList(usedCardList);
                usedCardList = new List<Card>();
            }

            Card card = cardStackList[0];
            cardStackList.RemoveAt(0);

            if (currentCardList.Count == cardAvailableSlot)  // need to burn drawn card
            {
                DiscardCard(card);
            }
            else
            {
                await AddToCurrentCards(card);
            }
        }
    }

    private async Task AddToCurrentCards(Card card)
    {
        Debug.Log("AddToCurrentCards: " + card._cardData.Name);

        currentCardList.Add(card);
        UIBattleCard currentCard;
        //Show card in UI
        if (cardList.Count < currentCardList.Count) //need init new card prefab
        {
            Debug.Log("need init new card prefab");
            currentCard = Instantiate(cardPrefab, showCardsContent.transform).GetComponent<UIBattleCard>();
            currentCard.ApplyCamera(uiCamera);
            currentCard.CreateCard(this, card);
            cardList.Add(currentCard);

            if (cardList.Count > 5)
                cardsLayoutGroup.spacing = new Vector2(cardsLayoutGroup.spacing.x - 10, cardsLayoutGroup.spacing.y);
        }
        else //update card content
        {
            currentCard = cardList[currentCardList.Count - 1];
            currentCard.UpdateCardData(card);
        }

        await currentCard.ShowCard();
    }

    private void DiscardCard(Card card)
    {
        discardCardList.Add(card);
    }

    public void RemoveCardsWithCharacterDie(CharacterData characterData)
    {
        cardStackList.RemoveAll(x => x._characterData.ID == characterData.ID);
        currentCardList.RemoveAll(x => x._characterData.ID == characterData.ID);
        var cards = cardList.FindAll(x => x.cardData._characterData.ID == characterData.ID);
        foreach ( var card in cards )
        {
            card.Invisible();
        }
    }

    public void EndPlayerTurn()
    {
        ////Move current cards to used cards
        //for(int i = 0;i < currentCardList.Count;i++)
        //{
        //    //Move to discard or used list
        //    usedCardList.Add(currentCardList[i]);
        //    currentCardList.RemoveAt(i);
        //}

        //for(int i = 0; i < cardList.Count; i++)
        //{
        //    cardList[i].Invisible();
        //}

        UpdateCardCount();
    }

    private async Task UseCard(Card targetCard)
    {
        if (isProcessingCard)
            return;

        isProcessingCard = true;
        int index = currentCardList.FindIndex(x => x == targetCard);
        cardList[index].Invisible();
        //Check can use or not
        if (!await battleController.UsePlayerCard(targetCard))
        {
            Debug.Log("UsePlayerCard return false");
            cardList[index].ShowCard();
            cardList[index].ShakeAnimation();
            isProcessingCard = false;
            return;
        }

        //Move to discard or used list
        usedCardList.Add(targetCard);
        currentCardList.RemoveAt(index);
        UIBattleCard usedCard = cardList[index];
        cardList.RemoveAt(index);
        cardList.Add(usedCard);

        UpdateCardCount();

        isProcessingCard = false;
    }

    public void TryToUseCard(Card card)
    {
        Debug.Log("TryToUseCard:" + card._cardData.ID);

        if (!CheckMouseInDragArea())
            return;

         UseCard(card);
    }

    private bool CheckMouseInDragArea()
    {
        Vector2 mousePosition = Input.mousePosition;
        // Check if the mouse is within the RectTransform bounds
        if (RectTransformUtility.RectangleContainsScreenPoint(cardDragAreaRectTrans, mousePosition, uiCamera))
        {
            Debug.Log("Mouse is within the RectTransform area. Mouse Position: " + mousePosition);
            return true;
        }

        return false;
    }

    public void ShowEffectRange(Card card)
    {
    }

    private void SelectedTargetTile()
    {

    }

    public bool CheckCardCondition(Card card)
    {
        return true;
    }

    public void UpdateCardCount()
    {
        stackCountTxt.text = "Stack: " + cardStackList.Count.ToString();
        usedCountTxt.text = "Used: " + usedCardList.Count.ToString();
        discardCountTxt.text = "Discard: " + discardCardList.Count.ToString();
    }

    public void UpdateMP(int curMP, int maxMp)
    {
        mpTxt.text = "MP: " + curMP + "/" + maxMp.ToString();
    }
}
