using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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

    public BattleCardController cardController;
    public BattlePlayerController playerController;
    public EnemyController enemyController;

    private BattleStage previousStage;
    private BattleStage currentStage;
    private bool isPause = false;

    private Action finishBattleCallback;

    //Temp
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

    public void Init(Action callback)
    {
        finishBattleCallback = callback;
        cardController.Init(this);
    }

    public void StartBattle()
    {
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
        Debug.Log("InitBattle");     
        gameObject.SetActive(true);
        RunStage(BattleStage.StartGame);
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
    }

    private void PauseGame(bool _isPause)
    {
        isPause = _isPause;
    }

    private bool ProcessUsingCard(Card card)
    {
        if (!playerController.UseMP(card.data.cost))
            return false;

        for (int i = 0; i < card.data.effectList.Count; i++)
        {
            //process effect
            var effect = card.data.effectList[i];
            if (card.data.cardType == CardType.AttackMelee)
            {
                if (effect.effectTarget == CardEffect.EffectTarget.Enemy)
                    enemyController.GetTargetEnemy().BeAttacked(int.Parse(effect.effectValueStr));
                else if (effect.effectTarget == CardEffect.EffectTarget.Player)
                    playerController.GetBattlePlayer().BeAttacked(int.Parse(effect.effectValueStr));
            }
            else if (card.data.cardType == CardType.Defend)
            {
                if (effect.effectTarget == CardEffect.EffectTarget.Enemy)
                    enemyController.GetTargetEnemy().BeDefenced(int.Parse(effect.effectValueStr));
                else if (effect.effectTarget == CardEffect.EffectTarget.Player)
                    playerController.GetBattlePlayer().BeDefenced(int.Parse(effect.effectValueStr));
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
