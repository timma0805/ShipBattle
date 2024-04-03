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
            enemyList = new List<BattleEnemy>();
            List<CardData> cardList = new List<CardData>();

            //Apply data
            for (int i = 0; i < playerData.battlePlayerCharacterDatas.Count; i++)
            {
                cardList.AddRange(playerData.battlePlayerCharacterDatas[i].CardDataList);

                BattlePlayerCharacter battlePlayerCharacter = transform.AddComponent<BattlePlayerCharacter>();
                battlePlayerCharacter.Init(playerData.battlePlayerCharacterDatas[i]);
                characterList.Add(battlePlayerCharacter);
            }

            cardController.StartBattle(cardList);

            for (int i = 0; i < entireMapData.EnemyDataList.Count; i++)
            {
                BattleEnemy battleEnemy = transform.AddComponent<BattleEnemy>();
                battleEnemy.Init(entireMapData.EnemyDataList[i]);
                enemyList.Add(battleEnemy);
            }

            uiCharacterPanel.StartBattle(characterList, enemyList);

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

    public bool UsePlayerCard(Card card)
    {
        if (currentStage != BattleStage.PlayerTurn || isPause)
            return false;

        if (!ProcessUsingCard(card))
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

    private bool ProcessUsingCard(Card card)
    {
        if (!UseMP(card.data.Cost))
            return false;

        for (int i = 0; i < card.effectList.Count; i++)
        {
            //process effect
            var effect = card.effectList[i];
            if (card.data.Type == CardType.Attack)
            {
                //if (effect.effectTarget == CardEffect.EffectTarget.Enemy)
                //    enemyController.GetTargetEnemy().BeAttacked(effect.effectValue);
                //else if (effect.effectTarget == CardEffect.EffectTarget.Self)
                //    enemyController.GetTargetEnemy().BeAttacked(effect.effectValue);
            }
        }

        return true;
    }

    public void ShowCardEffectRange(int rangeX, int rangeY, FaceDirection direction, Color color)
    {
        //Vector2 startPos = mapController.GetPlayerCurrentPos();
        //mapController.ShowCardEffectWithTitlesColor(startPos, rangeX, rangeY, direction, color);
    }

    public void ResetMapTiles()
    {
        //mapController.ResetMapTitlesColor();
    }

}
