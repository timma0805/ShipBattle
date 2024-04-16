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
using static UnityEngine.GraphicsBuffer;
using UnityEditor.Experimental.GraphView;

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

    private List<BattleCharacter> characterList;
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

        switch (nextStage)
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
            characterList = new List<BattleCharacter>();
            List<string> characterOccList = new List<string>();
            characterPosList = new List<Vector2>(); //TODO: player can edit before start battle

            List<CardData> cardList = new List<CardData>();

            //Apply data
            for (int i = 0; i < playerData.battlePlayerCharacterList.Count; i++)
            {
                playerData.battlePlayerCharacterList[i].CurHP = playerData.battlePlayerCharacterList[i].HP;
                cardList.AddRange(playerData.battlePlayerCharacterList[i].CardDataList);

                BattlePlayerCharacter battlePlayerCharacter = new BattlePlayerCharacter();
                battlePlayerCharacter.Init(playerData.battlePlayerCharacterList[i]);
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
        //Process Player Items
        ProcessPlayerItems(BattleStage.StartGame);

        RunStage(BattleStage.PlayerTurn);
    }

    private void ProcessPlayerItems(BattleStage battleStage)
    {
        List<ItemData> needProcessItems = new List<ItemData>();
        for (int i = 0; i < playerData.itemList.Count; i++)
        {
            if (playerData.itemList[i].EffectTime == ItemEffectTime.StartBattle && battleStage == BattleStage.StartGame)
                needProcessItems.Add(playerData.itemList[i]);
            else if (playerData.itemList[i].EffectTime == ItemEffectTime.EndBattle && battleStage == BattleStage.EndGame)
                needProcessItems.Add(playerData.itemList[i]);
        }

        foreach (ItemData item in needProcessItems)
        {
            ProcessItemEffect(item);
        }
    }

    private async Task ProcessItemEffect(ItemData item)
    {
        try
        {
            if (item.UseTime == 0)
                return;

            if (item.EffectTarget == ItemEffectTarget.Player)
            {
                if (item.Effect == ItemEffect.IncreaseReward)
                {

                }
                else if (item.Effect == ItemEffect.IncreaseMoney)
                {

                }

                return;
            }

            List<BattleCharacter> _characterList = new List<BattleCharacter>();

            if (item.EffectTarget == ItemEffectTarget.Any)
            {
                Vector2 targetPos = await uiCharacterPanel.ShowEffectArea(characterPosList, true, false);
                _characterList.Add(characterList[characterPosList.FindIndex(x => x == targetPos)]);
            }
            else if (item.EffectTarget == ItemEffectTarget.Enemy)
            {
                _characterList = characterList.FindAll(x => x.IsPlayerCharacter() == false);
            }
            else if (item.EffectTarget == ItemEffectTarget.Ally)
            {
                _characterList = characterList.FindAll(x => x.IsPlayerCharacter());
            }

            foreach (BattleCharacter character in _characterList)
            {
                Vector2 pos = characterPosList[characterList.FindIndex(x => x == character)];

                if (item.Effect == ItemEffect.IncreaseHP)
                {
                    character.BeHealed(item.Value);
                    await uiCharacterPanel.UpdateHP(pos, character.GetCharacterData().CurHP, item.EffectTime == ItemEffectTime.DuringBattle);
                }
                else if (item.Effect == ItemEffect.DecreaseHP)
                {
                    character.BeAttacked(item.Value);
                    await uiCharacterPanel.UpdateHP(pos, character.GetCharacterData().CurHP, item.EffectTime == ItemEffectTime.DuringBattle);
                }
                else if (item.Effect == ItemEffect.IncreaseAttack)
                {
                    character.BeAddStatus(CardEffectType.IncreaseAttack, item.Value);
                    await uiCharacterPanel.UpdateStatus(pos, character.statusDic, item.EffectTime == ItemEffectTime.DuringBattle);
                }
                else if (item.Effect == ItemEffect.Weakness)
                {
                    character.BeAddStatus(CardEffectType.Weakness, item.Value);
                    await uiCharacterPanel.UpdateStatus(pos, character.statusDic, item.EffectTime == ItemEffectTime.DuringBattle);
                }
                else if (item.Effect == ItemEffect.AddCountdown)
                    uiCharacterPanel.UpdateCountdown(pos, ((BattleEnemy)character).AddCountdown(item.Value));

            }

            if (item.UseTime > 0)
            {
                item.UseTime--;
            }
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    private async Task PlayerTurn()
    {
        try
        {
            currentMP++;
            if (currentMP > maxMP)
                currentMP = maxMP;

            uIBattleCardsPanel.UpdateMP(currentMP, maxMP);

            for (int i = 0; i < characterList.Count; i++)
            {
                if (characterList[i].IsPlayerCharacter())
                {
                    characterList[i].StartTurn();
                    uiCharacterPanel.UpdateHP(characterPosList[i], characterList[i].GetCharacterData().CurHP, false);
                    uiCharacterPanel.UpdateStatus(characterPosList[i], characterList[i].statusDic, false);
                }
            }

            await uIBattleCardsPanel.DrawCard();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public async Task<bool> UsePlayerCard(Card card)
    {
        try
        {
            if (currentStage != BattleStage.PlayerTurn || isPause)
                return false;

            if (characterList.Find(x => x.GetCharacterData().Name == card.data.Occupation).statusDic.ContainsKey(CharacterStatus.Stun))
                return false;

            if (characterList.Find(x => x.GetCharacterData().Name == card.data.Occupation).statusDic.ContainsKey(CharacterStatus.Unmovement) && card.data.Type == CardType.Move)
                return false;

            if (!await ProcessUsingCard(card))
                return false;

            uIBattleCardsPanel.UpdateMP(currentMP, maxMP);

            bool isAllDead = await CheckAllCharacterDie();
            if (isAllDead)
            {
                RunStage(BattleStage.EndGame);
                return true;
            }

            if (currentMP == 0)
                await EndPlayerTurn();

            return true;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            return false;
        }
    }

    public void PlayerSelectEndTurn()
    {
        EndPlayerTurn();
    }

    private async Task EndPlayerTurn()
    {
        try
        {
            uIBattleCardsPanel.EndPlayerTurn();
            for (int i = 0; i < characterList.Count; i++)
            {
                if (characterList[i].IsPlayerCharacter())
                {
                    characterList[i].EndTurn();
                    uiCharacterPanel.UpdateStatus(characterPosList[i], characterList[i].statusDic, false);
                }
            }

            bool isAllDead = await CheckAllCharacterDie();
            if (isAllDead)
                RunStage(BattleStage.EndGame);
            else
                RunStage(BattleStage.EnemyTurn);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private async Task EnemyTurn()
    {
        try
        {
            for (int i = 0; i < characterList.Count; i++)
            {
                if (!characterList[i].IsPlayerCharacter())
                {
                    characterList[i].StartTurn();
                    uiCharacterPanel.UpdateStatus(characterPosList[i], characterList[i].statusDic, false);
                    uiCharacterPanel.UpdateHP(characterPosList[i], characterList[i].GetCharacterData().CurHP, false);
                }
            }

            await DoEnemyAction();
        }
        catch(Exception e)
        {
            Debug.LogException(e);
        }
    }

   public async Task DoEnemyAction()
    {
        try
        {
            for (int i = 0; i < characterList.Count; i++)
            {
                if (characterList[i].IsPlayerCharacter())
                    continue;

                BattleEnemy enemy = characterList[i] as BattleEnemy;

                if (enemy.IsDead())
                    continue;

                Vector2 pos = characterPosList[i];
                var (skillData, countDown) = await enemy.DoAction(pos, GetPlayerCharactersPosList());

                if (skillData != null && countDown == 0)
                {
                    if (skillData.Type == CardType.Attack)
                    {
                        Vector2 newpos = FindEnemySkillBestPos(skillData, pos, characterList[i].GetFaceDirection());
                        if (newpos == pos)  //avoid attack himself
                            continue;
                        await uiCharacterPanel.Attack(pos, newpos, true);
                        BattleCharacter target = null;
                        int index = characterPosList.FindIndex(x => x == newpos);

                        if (index != -1)
                            target = characterList[index];

                        if (target != null)
                        {
                            int hp = target.BeAttacked((int)(skillData.Value * characterList[i].GetCharacterData().Attack));
                            await uiCharacterPanel.UpdateHP(newpos, hp, true);
                        }
                    }
                    else if (skillData.Type == CardType.Move)
                    {
                        Vector2 newpos = FindEnemySkillBestPos(skillData, pos, characterList[i].GetFaceDirection());
                        characterPosList[i] = newpos;
                        bool isSuccess = await uiCharacterPanel.MoveCharacter(pos, newpos, characterList[i].GetCharacterData());
                    }
                    else if (skillData.Type == CardType.Heal)
                    {
                        Vector2 newpos = Vector2.zero;
                        if (skillData.Target == EnemyActionTarget.Self)
                            newpos = pos;

                        await uiCharacterPanel.Heal(pos, newpos);
                        int hp = characterList[i].BeHealed((int)skillData.Value);
                        await uiCharacterPanel.UpdateHP(pos, hp, true);
                    }                 
                }
                else if(countDown > 0 && skillData.Type == CardType.Attack) 
                {
                    List<Vector2> effectPosList = FindEnemySkillEffectArea(skillData, pos, characterList[i].GetFaceDirection());
                    if(effectPosList.Count > 0) 
                        uiCharacterPanel.ShowEffectArea(effectPosList, false, false);
                    uiCharacterPanel.UpdateCountdown(pos, enemy.Countdown);
                }

                bool isAllDead = await CheckAllCharacterDie();
                if (isAllDead)
                {
                    RunStage(BattleStage.EndGame);
                    return;
                }
            }

            EndEnemyTurn();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

    }

    private List<Vector2> GetPlayerCharactersPosList()
    {
        List<Vector2> result = new List<Vector2>();
        for (int i = 0; i < characterList.Count; i++)
        {
            if (characterList[i].IsPlayerCharacter())
            {
                result.Add(characterPosList[i]);
            }
        }

        return result;
    }

    private List<Vector2> FindEnemySkillEffectArea(EnemySkillData skill, Vector2 pos, FaceDirection faceDirection)
    {
        if (skill.Target == EnemyActionTarget.Self)
            return new List<Vector2>() { pos };

        List<Vector2> effectPosList = new List<Vector2>();

        if (skill.Type == CardType.Move)
            effectPosList = GetEffectArea(pos, (int)skill.Value, skill.Direction, faceDirection, CardEffectTarget.Any, false);
        else if (skill.Type == CardType.Heal)
            effectPosList = GetEffectArea(pos, (int)skill.Value, skill.Direction, faceDirection, CardEffectTarget.Enemy, false);
        else
            effectPosList = GetEffectArea(pos, (int)skill.Value, skill.Direction, faceDirection, CardEffectTarget.Ally, false);

        return effectPosList;
    }

    private Vector2 FindEnemySkillBestPos(EnemySkillData skill, Vector2 pos, FaceDirection faceDirection)
    {
        Vector2 targetpos = pos;
        float compareValue = -1;

        if (skill.Target == EnemyActionTarget.Self)
            return pos;

        List<Vector2> effectPosList = FindEnemySkillEffectArea(skill, pos, faceDirection);

        int characterIndex = -1;
        for (int i = 0; i < effectPosList.Count; i++)
        {
            characterIndex = characterPosList.FindIndex(x => x == effectPosList[i]);
            if (characterIndex == -1)
                continue;

            var data = characterList[characterIndex].GetCharacterData();

            if (data != null && characterList[characterIndex].IsPlayerCharacter())
            {
                if (compareValue == -1 )
                {
                    if(skill.Target == EnemyActionTarget.Lowest || skill.Target == EnemyActionTarget.Hightest)
                        compareValue = data.CurHP;
                    else if (skill.Target == EnemyActionTarget.Nearest || skill.Target == EnemyActionTarget.Farest)
                        compareValue = Vector2.Distance(pos, effectPosList[i]);

                    targetpos = effectPosList[i];
                }
                else
                {
                    if (skill.Target == EnemyActionTarget.Lowest && compareValue > data.CurHP)
                    {
                        compareValue = data.CurHP;
                        targetpos = effectPosList[i];
                    }
                    else if (skill.Target == EnemyActionTarget.Hightest && compareValue < data.CurHP)
                    {
                        compareValue = data.CurHP;
                        targetpos = effectPosList[i];
                    }
                    else if (skill.Target == EnemyActionTarget.Nearest && compareValue < Vector2.Distance(pos, effectPosList[i]))
                    {
                        compareValue = Vector2.Distance(pos, effectPosList[i]);
                        targetpos = effectPosList[i];
                    }
                    else if (skill.Target == EnemyActionTarget.Farest && compareValue > Vector2.Distance(pos, effectPosList[i]))
                    {
                        compareValue = Vector2.Distance(pos, effectPosList[i]);
                        targetpos = effectPosList[i];
                    }
                }
            }
        }

        if (skill.Type == CardType.Move)
        {
            if (characterIndex == -1)
            {
                if (skill.Target == EnemyActionTarget.Nearest)
                {
                    float distance = 99;
                    for (int i = 0; i < characterPosList.Count; i++) {
                        if (Vector2.Distance(characterPosList[i], pos) < distance && characterList[i].IsPlayerCharacter())
                        {
                            distance = Vector2.Distance(characterPosList[i], pos);
                            characterIndex = i;
                        }    
                    }
                }
                else if (skill.Target == EnemyActionTarget.Farest)
                {
                    float distance = -1;
                    for (int i = 0; i < characterPosList.Count; i++)
                    {
                        if (Vector2.Distance(characterPosList[i], pos) > distance && characterList[i].IsPlayerCharacter())
                        {
                            distance = Vector2.Distance(characterPosList[i], pos);
                            characterIndex = i;
                        }
                    }
                }
                else if (skill.Target == EnemyActionTarget.Lowest)
                {
                    int hp = 9999;
                    for (int i = 0; i < characterList.Count; i++)
                    {
                        if (characterList[i].GetCharacterData().CurHP < hp && characterList[i].IsPlayerCharacter())
                        {
                            hp = characterList[i].GetCharacterData().CurHP;
                            characterIndex = i;
                        }
                    }
                }
                else if (skill.Target == EnemyActionTarget.Hightest)
                {
                    int hp = 0;
                    for (int i = 0; i < characterList.Count; i++)
                    {
                        if (characterList[i].GetCharacterData().CurHP > hp && characterList[i].IsPlayerCharacter())
                        {
                            hp = characterList[i].GetCharacterData().CurHP;
                            characterIndex = i;
                        }
                    }
                }
            }

            targetpos = characterPosList[characterIndex];
            if (targetpos.x > pos.x)
            {
                for (int i = (int)targetpos.x; i > pos.x; i--)
                {
                    if (characterPosList.FindIndex(x => x == new Vector2(i, targetpos.y)) == -1 && effectPosList.FindIndex(x => x == new Vector2(i, targetpos.y)) != -1)
                        return new Vector2(i, targetpos.y);
                }
            }
            else if (targetpos.x < pos.x)
            {
                for (int i = (int)targetpos.x; i < pos.x; i++)
                {
                    if (characterPosList.FindIndex(x => x == new Vector2(i, targetpos.y)) == -1 && effectPosList.FindIndex(x => x == new Vector2(i, targetpos.y)) != -1)
                        return new Vector2(i, targetpos.y);
                }
            }
            
            if (targetpos.y > pos.y)
            {
                for (int i = (int)targetpos.y; i > pos.y; i--)
                {
                    if (characterPosList.FindIndex(x => x == new Vector2(targetpos.x, i)) == -1 && effectPosList.FindIndex(x => x == new Vector2(targetpos.x, i)) != -1)
                        return new Vector2(targetpos.x, i);
                }
            }
            else if (targetpos.y < pos.y)
            {
                for (int i = (int)targetpos.y; i < pos.y; i++)
                {
                    if (characterPosList.FindIndex(x => x == new Vector2(targetpos.x, i)) == -1 && effectPosList.FindIndex(x => x == new Vector2(targetpos.x, i)) != -1)
                        return new Vector2(targetpos.x, i);
                }
            }

            targetpos = pos;    //no available position
        }

        return targetpos;
    }

    private async Task EndEnemyTurn()
    {
        for (int i = 0; i < characterList.Count; i++)
        {
            if (!characterList[i].IsPlayerCharacter())
            {
                characterList[i].EndTurn();
                uiCharacterPanel.UpdateStatus(characterPosList[i], characterList[i].statusDic, false);
            }
        }

        bool isAllDead = await CheckAllCharacterDie();
        if (isAllDead)
            RunStage(BattleStage.EndGame);
        else
            RunStage(BattleStage.PlayerTurn);
    }

    private async Task<bool> CheckAllCharacterDie()
    {
        bool isAllPlayerDead = true;
        bool isAllEnemyDead = true;
        for (int i = 0; i < characterList.Count; i++)
        {
            if (characterList[i].IsDead() && !characterList[i].IsRemoved()) //just dead
            {
                Debug.Log($"Character{characterList[i].GetCharacterData().Name} just dead");
                await uiCharacterPanel.CharacterDie(characterPosList[i]);
                characterPosList[i] = new Vector2(-1, -1);
                characterList[i].RemoveFromBattle();

                if (characterList[i].IsPlayerCharacter())   //need to remove related cards from stack
                {
                    uIBattleCardsPanel.RemoveCardsWithCharacterDie(characterList[i].GetCharacterData());
                }
            }
            else if (!characterList[i].IsDead() && characterList[i].IsPlayerCharacter())
                isAllPlayerDead = false;
            else if (!characterList[i].IsDead() && !characterList[i].IsPlayerCharacter())
                isAllEnemyDead = false;
        }

        return isAllEnemyDead || isAllPlayerDead;
    }

    private async Task EndGame()
    {
        Debug.Log("EndGame");
        bool isPlayerAllDead = true;
        for (int i = 0; i < characterList.Count; i++)
        {
            if (!characterList[i].IsDead() && characterList[i].IsPlayerCharacter())
            {
                isPlayerAllDead = false;
                break;
            }
        }

        if (isPlayerAllDead)
        {
            await uiCharacterPanel.Lose();
        }
        else
        {
            await uiCharacterPanel.Victory();
        }

        ProcessPlayerItems(BattleStage.EndGame);

        if (finishBattleCallback != null)
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
            var effectValue = effect.effectValue;
            if (effect.effectProperty == CardEffectType.Attack)
                effectValue = Mathf.RoundToInt(effect.effectValue * character.GetCharacterData().Attack);

            Vector2 startPos = pos;
            List <Vector2> newPosList = new List<Vector2>();

            if (effect.isAreaEffect)
            {
                if (effect.IsAreaTriggerAfterDIstance)
                {
                    startPos = await GetTargetPos(pos, effect, character);
                }
                else
                {
                    var dummyPos = await GetTargetPos(pos, effect, character);  //Just let player select
                }
                newPosList = GetEffectArea(startPos, effect.distance, effect.direction, character.GetFaceDirection(), effect.effectTarget, false);
            }
            else
            {
                startPos = await GetTargetPos(pos, effect, character);
            }

            if(startPos != pos || (effect.effectTarget == CardEffectTarget.Self && card.data.Type != CardType.Move))
                newPosList.Add(startPos);

            if (newPosList.Count == 0)
                return false;

            if(effect.successPercentage < 100)
            {
                int randomSuccess = UnityEngine.Random.Range(0, 100);
                if(randomSuccess > effect.successPercentage)    //Use failed, but still used the card
                {
                    return true;
                }
            }

            for(int j = 0; j < newPosList.Count; j++)
            {
                Vector2 newpos = newPosList[j];
                BattleCharacter target = null;
                int index = characterPosList.FindIndex(x => x == newpos);

                if (index != -1)
                    target = characterList[index];
                else if (effect.effectProperty != CardEffectType.Move)
                    continue;

              
                if (effect.effectProperty == CardEffectType.Move)
                {
                    if (effect.effectTarget == CardEffectTarget.Self)
                    {
                        if (newpos == pos)
                            return i > 0;

                        bool isSucess = await uiCharacterPanel.MoveCharacter(pos, newpos, character.GetCharacterData());
                        if (isSucess)
                            characterPosList[characterIndex] = newpos;
                        else
                            return i > 0;
                    }
                    else
                    {
                        var targetPos = await GetTargetPos(newpos, effect, character);
                        if (newpos == targetPos)
                            return i > 0;

                        bool isSucess = await uiCharacterPanel.MoveCharacter(newpos, targetPos, character.GetCharacterData());
                        if (isSucess)
                        {
                            characterPosList[index] = targetPos;
                        }
                        else
                            return i > 0;
                    }
                }
                else if (effect.effectProperty == CardEffectType.HP)
                {
                    await uiCharacterPanel.Heal(pos, newpos);
                    int hp = target.BeHealed((int)effect.effectValue);
                    await uiCharacterPanel.UpdateHP(newpos, hp, j == newPosList.Count - 1);
                }
                else
                {
                    if (target.statusDic.ContainsKey(CharacterStatus.Dogde))
                        effectValue = 0;

                    await uiCharacterPanel.Attack(pos, newpos, j == newPosList.Count - 1);

                    if (effect.effectProperty == CardEffectType.Attack)
                    {
                        if (target.statusDic.ContainsKey(CharacterStatus.Weakness))
                            effectValue *= 2;

                        int hp = target.BeAttacked((int)effectValue);
                        await uiCharacterPanel.UpdateHP(newpos, hp, j == newPosList.Count - 1);
                    }
                    else if (effectValue != 0)
                    {
                        if (target.IsPlayerCharacter())
                            effectValue += 0.5f;
                        var statusDic = target.BeAddStatus(effect.effectProperty, effectValue);
                        await uiCharacterPanel.UpdateStatus(newpos, statusDic, j == newPosList.Count - 1);
                    }
                }
            }          
        }

        if (!UseMP(card.data.Cost))
            return false;

        return true;
    }

    private async Task<Vector2> GetTargetPos(Vector2 pos, CardEffect effect, BattleCharacter character)
    {
        List<Vector2> vectors = GetEffectArea(pos, effect.distance, effect.direction, character.GetFaceDirection(), effect.effectTarget, effect.effectProperty == CardEffectType.Move);

        if (vectors.Count == 0)
        {
            Debug.Log("GetEffectArea 0 target");
            return pos;
        }

        if (effect.effectTarget == CardEffectTarget.Self && effect.effectProperty == CardEffectType.HP)
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
                else if (charData.statusDic.ContainsKey(CharacterStatus.Untargetable))
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
