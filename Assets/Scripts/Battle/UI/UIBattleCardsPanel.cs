using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class UIBattleCardsPanel : MonoBehaviour
{
    private Camera uiCamera;
    [SerializeField]
    private GameObject cardPrefab;

    [SerializeField]
    private GameObject showCardsContent;
   
    public RectTransform cardDragAreaRectTrans;

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

    private BattleCardController cardController;

    //Setting
    private int cardAvailableSlot = 5;
    private int drawInitCardAvailableCount = 3;
    private int drawCardAvailableCount = 1;

    // Start is called before the first frame update
    void Awake()
    {
        uiCamera = BattleGameCoreController.Instance.GetUICamera();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            DrawCard();
        }
    }

    public void Init(BattleCardController _cardController, List<CardData> cardDatas)
    {
        cardController = _cardController;
        List<Card> cards = GenerateCards(cardDatas);
        InitialCardStack(cards);
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
        cardList = new List<UIBattleCard>();
        usedCardList = new List<Card>();
        discardCardList = new List<Card>();
        currentCardList = new List<Card>();  
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

    public void DrawCard()
    {
        if (currentCardList.Count == 0 && usedCardList.Count == 0)// just start
        {
            DrawCards(drawInitCardAvailableCount);
        }
        else
        {
            DrawCards(drawCardAvailableCount);
        }
    }

    private async Task DrawCards(int count)
    {
        Debug.Log("DrawCards" + count);
        for (int i = 0; i < count; i++)
        {
            if(cardStackList.Count == 0)
            {
                Debug.Log("cardStackList Empty");
                return;
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
        Debug.Log("AddToCurrentCards: " + card.data.Name);

        currentCardList.Add(card);
        UIBattleCard currentCard;
        //Show card in UI
        if (cardList.Count < currentCardList.Count) //need init new card prefab
        {
            Debug.Log("need init new card prefab");
            currentCard = GameObject.Instantiate(cardPrefab, showCardsContent.transform).GetComponent<UIBattleCard>();
            currentCard.ApplyCamera(uiCamera);
            currentCard.CreateCard(this, card);
            cardList.Add(currentCard);
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

    private void UseCard(Card targetCard)
    {
        //Check can use or not
        if (!cardController.UsePlayerCard(targetCard))
        {
            Debug.Log("UsePlayerCard return false");
            return;
        }

        //Move to discard or used list
        usedCardList.Add(targetCard);

        int index = currentCardList.FindIndex(x => x == targetCard);
        currentCardList.RemoveAt(index);
        UIBattleCard usedCard = cardList[index];
        usedCard.Unvisible();
        cardList.RemoveAt(index);
        cardList.Add(usedCard);
    }

    public void TryToUseCard(Card card)
    {
        Debug.Log("TryToUseCard:" + card.data.ID);
        cardController.ResetMapTiles();

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
        cardController.ShowEffectRange(card);
    }

    private void SelectedTargetTile()
    {

    }

    public bool CheckCardCondition(Card card)
    {
        Debug.Log("CheckCardCondition");
        return cardController.CheckCardCondition(card);
    }
}
