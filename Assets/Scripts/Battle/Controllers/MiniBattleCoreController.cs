using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using UnityEngine.TextCore.Text;

public class MiniBattleCoreController : MonoBehaviour
{
    public enum BattleStage
    {
        Init,
        StartGame,
        PlayerTurn,
        EnemyTurn,
        EndGame,
        PauseGame
    }

    [SerializeField]
    private UIBattleCharacterPanel uiCharacterPanel;
    [SerializeField]
    private UIBattleCardsPanel uIBattleCardsPanel;

    private BattleStage previousStage;
    private BattleStage currentStage;
    private bool isPause = false;

    private BattlePlayerData playerData;
    private EntireMapData entireMapData;

    private Action finishBattleCallback;

    private List<ITargetObject> characterList;
    private List<Vector2> characterPosList;

    //Setting
    private const int maxMP = 99;
    private int currentMP;
    public int selectedMapIndex = 0;

    private void Awake()
    {
        currentStage = BattleStage.Init;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Init(Action callback )
    {
        finishBattleCallback = callback;
        uIBattleCardsPanel.Init(this);
        uiCharacterPanel.Init(this);
    }

    public void StartBattle(BattlePlayerData _battlePlayerData, EntireMapData _entireMapData)
    {
        playerData = _battlePlayerData;
        entireMapData = _entireMapData;
        RunStage(BattleStage.Init);
    }

    private void RunStage(BattleStage nextStage)
    {
        if (nextStage == currentStage && nextStage != BattleStage.Init)
            return;

        Debug.Log("RunStage:" + nextStage);
        previousStage = currentStage;
        currentStage = nextStage;

        switch (currentStage)
        {
            case BattleStage.Init:
                InitBattle();
                break;
            case BattleStage.StartGame:
                StartGame();
                break;
            case BattleStage.PlayerTurn:
                PlayerTurn();
                break;
            case BattleStage.EnemyTurn:
                EnemyTurn();
                break;
            case BattleStage.EndGame:
                EndGame();
                break;
            default:
                break;
        }
    }

    private async Task InitBattle()
    {
        try
        {
            Debug.Log("InitBattle");
            //Init List
            characterList = new List<ITargetObject>();
            List<string> characterOccList = new List<string>();
            characterPosList = new List<Vector2>(); //TODO: player can edit before start battle

            List<CardData> cardList = new List<CardData>();

            //Apply data
            for (int i = 0; i < playerData.battlePlayerCharacterDatas.Count; i++)
            {
                cardList.AddRange(playerData.battlePlayerCharacterDatas[i].CardDataList);

                BattlePlayerCharacter battlePlayerCharacter = new BattlePlayerCharacter();
                battlePlayerCharacter.Init(playerData.battlePlayerCharacterDatas[i]);
                characterList.Add(battlePlayerCharacter);
                characterOccList.Add(battlePlayerCharacter.GetCharacterData().Name);

                if (i < 3)
                    characterPosList.Add(new Vector2(0, i));    //Temp
                else
                    characterPosList.Add(new Vector2(1, i-3));    //Temp
            }

            uIBattleCardsPanel.StartBattle(cardList, characterOccList);

            for (int i = 0; i < entireMapData.EnemyDataList.Count; i++)
            {
                BattleEnemy battleEnemy = new BattleEnemy();
                battleEnemy.Init(entireMapData.EnemyDataList[i]);
                characterList.Add(battleEnemy);

                if (i < 3)
                    characterPosList.Add(new Vector2(5, i));    //Temp
                else
                    characterPosList.Add(new Vector2(4, i-3));    //Temp
            }

            uiCharacterPanel.StartBattle(characterList, characterPosList);

            currentMP = maxMP;
            uIBattleCardsPanel.UpdateMP(currentMP, maxMP);

            gameObject.SetActive(true);
            RunStage(BattleStage.StartGame);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private async Task StartGame()
    {
        RunStage(BattleStage.PlayerTurn);
    }

    private async Task PlayerTurn()
    {
        currentMP = maxMP;
        uIBattleCardsPanel.UpdateMP(currentMP, maxMP);

        await uIBattleCardsPanel.DrawCard();
    }

    public async Task<bool> UsePlayerCard(Card card)
    {
        if (currentStage != BattleStage.PlayerTurn || isPause)
            return false;

        if (!await ProcessUsingCard(card))
            return false;

        uIBattleCardsPanel.UpdateMP(currentMP, maxMP);

        if(currentMP == 0)
            await EndPlayerTurn();

        return true;
    }

    public void PlayerSelectEndTurn()
    {
        EndPlayerTurn();
    }

    private async Task EndPlayerTurn()
    {
        uIBattleCardsPanel.EndPlayerTurn();
        RunStage(BattleStage.EnemyTurn);
    }

    private async Task EnemyTurn()
    {
        DoEnemyAction();
    }

   public void DoEnemyAction()
    {
        EndEnemyTurn();
    }

    private async Task EndEnemyTurn()
    {      
        RunStage(BattleStage.PlayerTurn);
    }


    private async Task EndGame()
    {
        Debug.Log("EndGame");
        if(finishBattleCallback != null)
            finishBattleCallback();
    }

    private void PauseGame(bool _isPause)
    {
        isPause = _isPause;
    }

    private bool UseMP(int cost)
    {
        if (currentMP - cost >= 0)
        {
            currentMP -= cost;
            return true;
        }

        return false;
    }

    private async Task<bool> ProcessUsingCard(Card card)
    {
        //Check MP
        if (currentMP - card.data.Cost < 0)
            return false;
           
        for (int i = 0; i < card.effectList.Count; i++)
        {
            //process effect
            var effect = card.effectList[i];
            var characterIndex = characterList.FindIndex(x => x.GetCharacterData().Name == card.data.Occupation);
            if (characterIndex == -1)
            {
                Debug.LogError("UsingCard character null");
                return false;
            }

            var pos = characterPosList[characterIndex];
            var character = characterList[characterIndex];

            if (card.data.Type == CardType.Attack)
            {
                int attackValue = Mathf.RoundToInt( effect.effectValue * character.GetCharacterData().Attack);
                Vector2 newpos = await GetTargetPos(pos, effect, character);
                await uiCharacterPanel.Attack(pos, newpos);
                ITargetObject target = null;
                int index = characterPosList.FindIndex(x => x == newpos);

                if (effect.effectTarget == CardEffectTarget.Any)
                {
                    if (index != -1)
                        target = characterList[index];
                }
                else if (effect.effectTarget == CardEffectTarget.Enemy)
                {
                    if (index != -1 && !characterList[index].IsPlayerCharacter())
                        target = characterList[index];
                }
                else if (effect.effectTarget == CardEffectTarget.Ally || effect.effectTarget == CardEffectTarget.Self)
                {
                    if (index != -1 && characterList[index].IsPlayerCharacter())
                        target = characterList[index];
                }

                if (target != null)
                {
                    int hp = target.BeAttacked(attackValue);
                    uiCharacterPanel.UpdateHP(newpos, hp);
                }
            }
            else if(card.data.Type == CardType.Move)
            {
                Vector2 newpos = await GetTargetPos(pos, effect, character);
                bool isSucess = await uiCharacterPanel.MoveCharacter(pos, newpos);
                if (isSucess)
                    characterPosList[characterIndex] = newpos;
                else
                    return false;
            }
        }

        if (!UseMP(card.data.Cost))
            return false;

        return true;
    }

    private async Task<Vector2> GetTargetPos(Vector2 pos, CardEffect effect, ITargetObject character)
    {
        List<Vector2> vectors = new List<Vector2>();

        vectors.Add(pos);

        for (int i = 1; i < effect.distance+1; i++)
        {
            if (effect.direction == FaceDirection.All)
            {
                vectors.Add(new Vector2(pos.x +i, pos.y));
                vectors.Add(new Vector2(pos.x - i, pos.y));
                vectors.Add(new Vector2(pos.x, pos.y +i));
                vectors.Add(new Vector2(pos.x, pos.y - i));
            }
            else if (effect.direction == FaceDirection.Front)
            {
                if (character.GetFaceDirection() == FaceDirection.Front)
                {
                    vectors.Add(new Vector2(pos.x + i, pos.y));
                }
                else if (character.GetFaceDirection() == FaceDirection.Back)
                {
                    vectors.Add(new Vector2(pos.x - i, pos.y));
                }
            }
            else if (effect.direction == FaceDirection.Back)
            {
                if (character.GetFaceDirection() == FaceDirection.Front)
                {
                    vectors.Add(new Vector2(pos.x - i, pos.y));
                }
                else if (character.GetFaceDirection() == FaceDirection.Back)
                {
                    vectors.Add(new Vector2(pos.x + i, pos.y));
                }
            }
        }

        Vector2 newpos = await uiCharacterPanel.ShowEffectArea(vectors, true);

        return newpos;
    }
    public void ShowCardEffectRange(Card card)
    {
        Debug.Log("ShowCardEffectRange");
        //Vector2 startPos = mapController.GetPlayerCurrentPos();
        //mapController.ShowCardEffectWithTitlesColor(startPos, rangeX, rangeY, direction, color);
    }

    public void ResetMapTiles()
    {
        //mapController.ResetMapTitlesColor();
    }

}
