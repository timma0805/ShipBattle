using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(EnemyController))]
[RequireComponent(typeof(PlayerController))]

public class BattleCoreController : MonoBehaviour
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

    public BattleMapController mapController;
    public BattleCardController cardController;
    public EnemyController enemyController;
    public BattlePlayerController playerController;
    public UIBattleCoreController uIBattleCoreController;

    private BattleStage previousStage;
    private BattleStage currentStage;
    private bool isPause = false;

    //Temp
    public int selectedMapIndex = 0;

    private void Awake()
    {
        currentStage = BattleStage.Init;

        if(playerController == null)
            playerController = GetComponent<BattlePlayerController>();
        if (enemyController == null)
            enemyController = GetComponent<EnemyController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        RunStage(BattleStage.Init);
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void RunStage(BattleStage nextStage)
    {
        if (nextStage == currentStage && nextStage != BattleStage.Init)
            return;

        Debug.Log("RunStage:" + nextStage);
        previousStage = currentStage;
        currentStage = nextStage;

        cardController.UpdateCharacterData(playerController.GetBattlePlayer(), enemyController.GetAllEnemies());

        switch (currentStage)
        {
            case BattleStage.Init:
                InitControllers();
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

    private async Task InitControllers()
    {
        Debug.Log("InitControllers");
        int[] enemyIndexList = await mapController.Init(new BattleMapController.BattleMapInitInitParameters() { mapIndex = selectedMapIndex});
        Debug.Log("mapController init: " + enemyIndexList.Length);
        GameObject playerObj = playerController.GetPlayerPrefab();
        Debug.Log("playerController init: " + playerObj.name);
        GameObject[] enemyObjs = enemyController.GetEnemiesPrefabs(enemyIndexList);
        Debug.Log("enemyController init: " + enemyObjs.Length);
        await mapController.SpawnCharacters(new BattleMapController.BattleMapCharactersParameters() {
            playerWidth = playerController.GetPlayerWidth(),
            playerLength = playerController.GetPlayerLength(),
            playerPrefab = playerObj,
            enemyPrefabsList = enemyObjs,
            enemyWidthList = enemyController.GetTargetEnemiesWidth(),
            enemyLengthList = enemyController.GetTargetEnemiesLength()
        });
        Debug.Log("mapController init: " + mapController.GetEnemyObjs().Length);

        await playerController.Init(this, mapController.GetPlayerObj());
        await enemyController.Init(this, mapController.GetEnemyObjs());
        cardController.Init(this);
        RunStage(BattleStage.StartGame);
        uIBattleCoreController.CheckCharacterDataUpdate(playerController.GetBattlePlayer(), enemyController.GetAllEnemies());

    }

    private async Task StartGame()
    {
        RunStage(BattleStage.PlayerTurn);
    }

    private async Task PlayerTurn()
    {
        playerController.StartPlayerTurn();
        cardController.StartPlayerTurn();
    }

    public bool UsePlayerCard(Card card)
    {
        if (currentStage != BattleStage.PlayerTurn || isPause)
            return false;

        if (!ProcessUsingCard(card))
            return false;

        uIBattleCoreController.CheckCharacterDataUpdate(playerController.GetBattlePlayer(), enemyController.GetAllEnemies());

        if (enemyController.CheckIsAllEnemyDead())
            RunStage(BattleStage.EndGame);
        else
            EndPlayerTurn();

        return true;
    }

    private async Task EndPlayerTurn()
    {
        RunStage(BattleStage.EnemyTurn);
    }

    private async Task EnemyTurn()
    {
        enemyController.StartEnemyTurn();
    }

   public void DoEnemyAction()
    {
        playerController.GetBattlePlayer().BeAttacked(-1);
        EndEnemyTurn();
    }

    private async Task EndEnemyTurn()
    {
        if (enemyController.CheckIsAllEnemyDead())
            RunStage(BattleStage.EndGame);
        else
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
        Vector2 startPos = mapController.GetPlayerCurrentPos();
        mapController.ShowCardEffectWithTitlesColor(startPos, rangeX, rangeY, direction, color);
    }

    public void ResetMapTiles()
    {
        mapController.ResetMapTitlesColor();
    }

}
