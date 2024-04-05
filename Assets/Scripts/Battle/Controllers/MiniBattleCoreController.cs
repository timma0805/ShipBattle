using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using UnityEngine.TextCore.Text;
using static JsonManager;

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
                playerData.battlePlayerCharacterDatas[i].CurHP = playerData.battlePlayerCharacterDatas[i].HP;
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
                entireMapData.EnemyDataList[i].CurHP = entireMapData.EnemyDataList[i].HP;

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

            if (effect.effectProperty == CardEffectType.Attack)
            {
                int attackValue = Mathf.RoundToInt( effect.effectValue * character.GetCharacterData().Attack);
                if (effect.isAreaEffect)
                {
                    if(effect.IsAreaTriggerAfterDIstance)
                    {
                        Vector2 newpos = await GetTargetPos(pos, effect, character);
                        if (newpos == pos)
                            return false;

                        List<Vector2> effectPosList = GetEffectArea(newpos, effect.distance, effect.direction, character.GetFaceDirection(), effect.effectTarget, false);
                        effectPosList.Add(pos);

                        for (int j = 0; j < effectPosList.Count; j++)
                        {
                            await ProcessAttack(effect, pos, effectPosList[j], attackValue, j == 0);
                        }
                    }
                    else
                    {
                        Vector2 newpos = await GetTargetPos(pos, effect, character); //just for waiting Player click
                        List<Vector2> effectPosList = GetEffectArea(pos, effect.distance, effect.direction, character.GetFaceDirection(), effect.effectTarget, false);
                        for (int j = 0; j < effectPosList.Count; j++)
                        {
                            await ProcessAttack(effect, pos, effectPosList[j], attackValue, j == 0);
                        }
                    }
                }
                else
                {
                    Vector2 newpos = await GetTargetPos(pos, effect, character);
                    if (newpos == pos)
                        return false;

                    await ProcessAttack(effect, pos, newpos, attackValue, true);
                }
            }
            else if (effect.effectProperty == CardEffectType.Move)
            {
                if (effect.effectTarget == CardEffectTarget.Self)
                {
                    Vector2 newpos = await GetTargetPos(pos, effect, character);
                    if (newpos == pos)
                        return false;

                    bool isSucess = await uiCharacterPanel.MoveCharacter(pos, newpos, character.GetCharacterData());
                    if (isSucess)
                        characterPosList[characterIndex] = newpos;
                    else
                        return false;
                }
                else
                {
                    Vector2 targetPos = await GetTargetPos(pos, effect, character);
                    if (targetPos == pos)
                        return false;
                    Vector2 newpos = await GetTargetPos(targetPos, effect, character);
                    if (newpos == pos)
                        return false;

                    ITargetObject target = null;
                    int index = characterPosList.FindIndex(x => x == targetPos);

                    if (index != -1)
                        target = characterList[index];

                    if (target != null)
                    {
                        bool isSucess = await uiCharacterPanel.MoveCharacter(targetPos, newpos, character.GetCharacterData());
                        if (isSucess)
                            characterPosList[index] = newpos;
                        else
                            return false;
                    }
                    else
                        return false;
                }
            }
            else if (effect.effectProperty == CardEffectType.HP)
            {
                Vector2 newpos = pos;

                if (effect.effectTarget != CardEffectTarget.Self)
                {
                    newpos = await GetTargetPos(pos, effect, character);
                    if(newpos == pos)
                        return false;
                }
                else
                    newpos = await uiCharacterPanel.ShowEffectArea(new List<Vector2> { pos }, true, effect.isAreaEffect);


                await uiCharacterPanel.Heal(pos, newpos);
                ITargetObject target = null;
                int index = characterPosList.FindIndex(x => x == newpos);

                if (index != -1)
                    target = characterList[index];

                if (target != null)
                {
                    int hp = target.BeHealed((int)effect.effectValue);
                    uiCharacterPanel.UpdateHP(newpos, hp);
                }

            }
        }

        if (!UseMP(card.data.Cost))
            return false;

        return true;
    }

    private async Task ProcessAttack(CardEffect effect, Vector2 pos, Vector2 newpos, int attackValue, bool needAttackAnimation)
    {
        await uiCharacterPanel.Attack(pos, newpos, needAttackAnimation);
        ITargetObject target = null;
        int index = characterPosList.FindIndex(x => x == newpos);

        if (index != -1)
            target = characterList[index];

        if (target != null)
        {
            int hp = target.BeAttacked(attackValue);
            uiCharacterPanel.UpdateHP(newpos, hp);
        }
    }

    private async Task<Vector2> GetTargetPos(Vector2 pos, CardEffect effect, ITargetObject character)
    {
        List<Vector2> vectors = GetEffectArea(pos, effect.distance, effect.direction, character.GetFaceDirection(), effect.effectTarget, effect.effectProperty == CardEffectType.Move);
        if (vectors.Count == 0)
        {
            Debug.Log("GetEffectArea 0 target");
            return pos;
        }
        if(effect.effectTarget == CardEffectTarget.Self && effect.effectProperty == CardEffectType.HP)
            vectors.Add(pos);

        Vector2 newpos = await uiCharacterPanel.ShowEffectArea(vectors, true, effect.isAreaEffect);

        return newpos;
    }

    private List<Vector2> GetEffectArea(Vector2 pos, int distance, FaceDirection direction, FaceDirection characterDirection, CardEffectTarget effectTarget, bool avoidCharacter)
    {
        List<Vector2> vectors = new List<Vector2>();

        for (int i = 1; i < distance + 1; i++)
        {
            if (direction == FaceDirection.All)
            {
                vectors.Add(new Vector2(pos.x + i, pos.y));
                vectors.Add(new Vector2(pos.x - i, pos.y));
                vectors.Add(new Vector2(pos.x, pos.y + i));
                vectors.Add(new Vector2(pos.x, pos.y - i));
            }
            else if (direction == FaceDirection.Front)
            {
                if (characterDirection == FaceDirection.Front)
                {
                    vectors.Add(new Vector2(pos.x + i, pos.y));
                }
                else if (characterDirection == FaceDirection.Back)
                {
                    vectors.Add(new Vector2(pos.x - i, pos.y));
                }
            }
            else if (direction == FaceDirection.Back)
            {
                if (characterDirection == FaceDirection.Front)
                {
                    vectors.Add(new Vector2(pos.x - i, pos.y));
                }
                else if (characterDirection == FaceDirection.Back)
                {
                    vectors.Add(new Vector2(pos.x + i, pos.y));
                }
            }
        }

        if(effectTarget == CardEffectTarget.Any ) 
            return vectors;

        List<Vector2> resultVectors = new List<Vector2>(vectors);

        for (int i = 0; i < vectors.Count; i++)
        {
            var charIndex = characterPosList.FindIndex(x => x == vectors[i]);
            if (charIndex != -1)
            {
                var charData = characterList[charIndex];
                if(effectTarget == CardEffectTarget.Enemy && charData.IsPlayerCharacter())
                    resultVectors.Remove(vectors[i]);
                else if (effectTarget == CardEffectTarget.Ally && !charData.IsPlayerCharacter())
                    resultVectors.Remove(vectors[i]);
                else if(effectTarget == CardEffectTarget.Ground)
                    resultVectors.Remove(vectors[i]);
                else if(avoidCharacter)
                    resultVectors.Remove(vectors[i]);
            }
            else
            {
                if (effectTarget== CardEffectTarget.Ally || effectTarget == CardEffectTarget.Enemy)
                    resultVectors.Remove(vectors[i]);
            }    
        }


        return resultVectors;
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
