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
    private List<Card> baseMoveCardList;
  
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

    public void StartBattle(List<CardData> cardDatas, List<string> occList)
    {
        List<Card> cards = GenerateCards(cardDatas);
        InitialCardStack(cards);
        UpdateCardCount();

        //Create base move cards
        baseMoveCardList = new List<Card>();
        for(int i = 0; i < occList.Count; i++)
        {
            CardData cardData = new CardData(occList[i]);
            Card newcard = new Card(cardData);
            baseMoveCardList.Add(newcard);
        }
    }

    private List<Card> GenerateCards(List<CardData> cardDatas)
    {
        List<Card> cards = new List<Card>();
        for (int i = 0; i < cardDatas.Count; i++)
        {
            Card newcard = new Card(cardDatas[i]);
            cards.Add(newcard);
        }

        return cards;
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
                cardList[i].Unvisible();
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

    public async Task DrawCard()
    {
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
        for(int i = 0; i < baseMoveCardList.Count;i++)
        {
            await AddToCurrentCards(baseMoveCardList[i]);
        }
    }

    private async Task AddToCurrentCards(Card card)
    {
        Debug.Log("AddToCurrentCards: " + card.data.Name);

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

    public void EndPlayerTurn()
    {
        //Move current cards to used cards
        for(int i = 0;i < currentCardList.Count;i++)
        {
            //Move to discard or used list
            if (!currentCardList[i].data.Temp) //
                usedCardList.Add(currentCardList[i]);
            currentCardList.RemoveAt(i);
        }

        for(int i = 0; i < cardList.Count; i++)
        {
            cardList[i].Unvisible();
        }

        UpdateCardCount();
    }

    private async Task UseCard(Card targetCard)
    {
        int index = currentCardList.FindIndex(x => x == targetCard);
        cardList[index].Unvisible();
        //Check can use or not
        if (!await battleController.UsePlayerCard(targetCard))
        {
            Debug.Log("UsePlayerCard return false");
            cardList[index].ShowCard();
            cardList[index].ShakeAnimation();
            return;
        }

        //Move to discard or used list
        if(!targetCard.data.Temp)
            usedCardList.Add(targetCard);

        currentCardList.RemoveAt(index);
        UIBattleCard usedCard = cardList[index];
        usedCard.Unvisible();
        cardList.RemoveAt(index);
        cardList.Add(usedCard);

        UpdateCardCount();
    }

    public void TryToUseCard(Card card)
    {
        Debug.Log("TryToUseCard:" + card.data.ID);

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
