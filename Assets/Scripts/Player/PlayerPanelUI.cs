using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPanelUI : MonoBehaviour
{
    [SerializeField]
    private Button closeBtn;
    [SerializeField]
    private Image maskImg;

    [SerializeField]
    private Button openCrewPanelBtn;

    [SerializeField]
    private GameObject crewPanelObj;
    [SerializeField]
    private GameObject crewInfoObj;
    [SerializeField]
    private TMP_Text crewNameTxt;
    [SerializeField]
    private TMP_Text crewDescriptionTxt;
    [SerializeField]
    private Image crewIconImg;

    [SerializeField]
    private GameObject crewListObj;
    [SerializeField]
    private ScrollRect crewScrollRect;
    [SerializeField]
    private GridLayoutGroup crewLayoutGroup;
    [SerializeField]
    private GameObject crewPrefab;

    [SerializeField]
    private GameObject cardGpObj;
    [SerializeField]
    private ScrollRect cardsScrollRect;
    [SerializeField]
    private GridLayoutGroup cardLayoutGroup;
    [SerializeField]
    private GameObject cardPrefab;

    private List<UIBattleCard> cardUILIst;
    private List<CrewCardUI> crewUIList;
    private PlayerController _playerController;
    // Start is called before the first frame update
    void Awake()
    {
        closeBtn.onClick.AddListener(CloseOrBack);
        openCrewPanelBtn.onClick.AddListener(ShowCrewPanel);

        cardUILIst = new List<UIBattleCard>();
        crewUIList = new List<CrewCardUI>();

        //Disable All
        maskImg.gameObject.SetActive(false);
        crewPanelObj.gameObject.SetActive(false);
        crewListObj.gameObject.SetActive(false);
        HideCrewInfo();
    }

    public void Init(PlayerController playerController)
    {
        _playerController = playerController;
    }

    public void ShowCrewPanel()
    {
        maskImg.gameObject.SetActive(true);
        crewPanelObj.SetActive(true);
        ShowCrewList(_playerController.GetBattlePlayerData().battlePlayerCharacterList);

        openCrewPanelBtn.gameObject.SetActive(false);
    }

    private void ShowCrewList(List<BattlePlayerCharacterData> datas)
    {
        for (int i = 0; i < crewUIList.Count; i++)
        {
            crewUIList[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < datas.Count; i++)
        {
            if (i >= crewUIList.Count)
            {
                CrewCardUI newCrew = Instantiate(crewPrefab, crewLayoutGroup.transform).GetComponent<CrewCardUI>();
                crewUIList.Add(newCrew);
            }

            crewUIList[i].UpdateCharacter(datas[i], CrewOnCLick);
            crewUIList[i].gameObject.SetActive(true);
        }

        crewScrollRect.verticalNormalizedPosition = 0;
        crewListObj.SetActive(true);
    }

    private void CrewOnCLick(CharacterData characterData)
    {
        if (characterData == null)
            return;

        if(characterData.Type == CharacterType.Crew || characterData.Type == CharacterType.Captain)
        {
            ShowCrewInfo((BattlePlayerCharacterData)characterData);
            crewListObj.SetActive(false);
        }
    }
    

    private void ShowCrewInfo(BattlePlayerCharacterData characterData)
    {
        if (characterData == null)
            return;

        crewNameTxt.text = characterData.Name;
        crewDescriptionTxt.text = characterData.Description;

        ShowCardList(characterData.CardList);

        crewInfoObj.SetActive(true);
    }

    private void ShowCardList(List<Card> cardDatas)
    {
        for (int i = 0; i < cardUILIst.Count; i++)
        {
            cardUILIst[i].Invisible();
        }

        for (int i = 0; i < cardDatas.Count; i++)
        {
            if(i <  cardUILIst.Count) //reuse created
            {
                cardUILIst[i].UpdateCardData(cardDatas[i]);
            }
            else
            {
                UIBattleCard newCard = Instantiate(cardPrefab, cardLayoutGroup.transform).GetComponent<UIBattleCard>();
                newCard.UpdateCardData(cardDatas[i]);

                cardUILIst.Add(newCard);
            }

            cardUILIst[i].ShowCard();
        }

        cardsScrollRect.verticalNormalizedPosition = 0;
        cardGpObj.SetActive(true);
    }

    private void HideCrewInfo()
    {
        crewInfoObj.SetActive(false);
        cardGpObj.SetActive(false);
    }

    private void CloseOrBack()
    {
        if (crewInfoObj.gameObject.activeInHierarchy)
        {
            HideCrewInfo();
            crewListObj.SetActive(true);
        }
        else
        {
            maskImg.gameObject.SetActive(false);
            crewPanelObj.SetActive(false);
            openCrewPanelBtn.gameObject.SetActive(true);
        }
    }
}
