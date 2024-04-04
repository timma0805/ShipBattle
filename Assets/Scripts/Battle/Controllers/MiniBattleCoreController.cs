using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

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

    public UIBattleCharacterPanel uiCharacterPanel;
    public BattleCardController cardController;

    private BattleStage previousStage;
    private BattleStage currentStage;
    private bool isPause = false;

    private BattlePlayerData playerData;
    private EntireMapData entireMapData;

    private Action finishBattleCallback;

    private List<BattlePlayerCharacter> characterList;
    private List<BattleEnemy> enemyList;
    private List<Vector2> characterPosList;
    private List<Vector2> enemyPosList;

    //Setting
    private const int MaxMP = 3;
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
        cardController.Init(this);
        uiCharacterPanel.Init();
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
            characterList = new List<BattlePlayerCharacter>();
            characterPosList = new List<Vector2>(); //TODO: player can edit before start battle
            enemyList = new List<BattleEnemy>();
            enemyPosList = new List<Vector2>();

            List<CardData> cardList = new List<CardData>();

            //Apply data
            for (int i = 0; i < playerData.battlePlayerCharacterDatas.Count; i++)
            {
                cardList.AddRange(playerData.battlePlayerCharacterDatas[i].CardDataList);

                BattlePlayerCharacter battlePlayerCharacter = new BattlePlayerCharacter();
                battlePlayerCharacter.Init(playerData.battlePlayerCharacterDatas[i]);
                characterList.Add(battlePlayerCharacter);

                if(i < 3)
                    characterPosList.Add(new Vector2(0, i));    //Temp
                else
                    characterPosList.Add(new Vector2(1, i-3));    //Temp
            }

            cardController.StartBattle(cardList);

            for (int i = 0; i < entireMapData.EnemyDataList.Count; i++)
            {
                BattleEnemy battleEnemy = new BattleEnemy();
                battleEnemy.Init(entireMapData.EnemyDataList[i]);
                enemyList.Add(battleEnemy);

                if (i < 3)
                    enemyPosList.Add(new Vector2(5, i));    //Temp
                else
                    enemyPosList.Add(new Vector2(4, i-3));    //Temp
            }

            uiCharacterPanel.StartBattle(characterList, characterPosList, enemyList, enemyPosList);

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
        cardController.StartPlayerTurn();
    }

    public async Task<bool> UsePlayerCard(Card card)
    {
        if (currentStage != BattleStage.PlayerTurn || isPause)
            return false;

        if (!await ProcessUsingCard(card))
            return false;

        EndPlayerTurn();

        return true;
    }

    private async Task EndPlayerTurn()
    {
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
        if (currentMP + cost <= MaxMP)
        {
            currentMP -= cost;
            return true;
        }

        return false;
    }

    private async Task<bool> ProcessUsingCard(Card card)
    {
        if (!UseMP(card.data.Cost))
            return false;

        card.data.Type = CardType.Move;

        for (int i = 0; i < card.effectList.Count; i++)
        {
            //process effect
            var effect = card.effectList[i];
            var characterIndex = characterList.FindIndex(x => x.characterData.Name == card.data.Occupation);
            if (characterIndex == -1)
            {
                Debug.LogError("UsingCard character null");
                return false;
            }

            var pos = characterPosList[characterIndex];
            var character = characterList[characterIndex];

            if (card.data.Type == CardType.Attack)
            {
                int attackValue = Mathf.RoundToInt( effect.effectValue * character.characterData.Attack);
                List<ITargetObject> targetObjects = GetEffectTargetObjects(pos, character.currentDirection, effect.direction, effect.distance);

                foreach (ITargetObject targetObject in targetObjects) { 
                    if (targetObject != null) {
                        targetObject.BeAttacked(attackValue);
                    }
                }

                uiCharacterPanel.CharacterAttack(pos);
            }
            else if(card.data.Type == CardType.Move)
            {
                var newpos = pos;
                if(character.currentDirection == FaceDirection.Front)
                {
                    newpos = new Vector2(newpos.x + effect.distance, newpos.y);
                }
                else if (character.currentDirection == FaceDirection.Back)
                {
                    newpos = new Vector2(newpos.x - effect.distance, newpos.y);
                }
                else if (character.currentDirection == FaceDirection.Left)
                {
                    newpos = new Vector2(newpos.x , newpos.y - effect.distance);
                }
                else if (character.currentDirection == FaceDirection.Front)
                {
                    newpos = new Vector2(newpos.x , newpos.y + effect.distance);
                }

                bool isSucess = await uiCharacterPanel.MoveCharacter(pos, newpos);
                characterPosList[characterIndex] = newpos;
            }
        }

        return true;
    }

    private List<ITargetObject> GetEffectTargetObjects(Vector2 startPos, FaceDirection direction, FaceDirection effectDirection, int distance)
    {
        List<ITargetObject> targetObjects = new List<ITargetObject>();

        return targetObjects;
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
