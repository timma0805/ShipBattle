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
using UnityEditor.U2D.Animation;

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
    private List<Vector2> boardPosList;
    private int currentRound;

    //Setting
    private int maxMP;
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
        boardPosList = GetAllPosList();
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

            List<Card> cardList = new List<Card>();

            //Apply data
            for (int i = 0; i < playerData.battlePlayerCharacterList.Count; i++)
            {
                playerData.battlePlayerCharacterList[i].CurHP = playerData.battlePlayerCharacterList[i].HP;
                cardList.AddRange(playerData.battlePlayerCharacterList[i].CardList);

                BattlePlayerCharacter battlePlayerCharacter = new BattlePlayerCharacter();
                if (i < uiCharacterPanel.maxSlotPerColumn)
                    battlePlayerCharacter.Init(playerData.battlePlayerCharacterList[i], new Vector2(0, i), FaceDirection.Front, ActiveCharacterStatus);
                else
                    battlePlayerCharacter.Init(playerData.battlePlayerCharacterList[i], new Vector2(1, i-3), FaceDirection.Front, ActiveCharacterStatus);

                characterList.Add(battlePlayerCharacter);
            }

            maxMP = playerData.GetMaxMP();

            uIBattleCardsPanel.StartBattle(cardList);

            for (int i = 0; i < entireMapData.EnemyDataList.Count; i++)
            {
                entireMapData.EnemyDataList[i].CurHP = entireMapData.EnemyDataList[i].HP;

                BattleEnemy battleEnemy = new BattleEnemy();
                if (i < uiCharacterPanel.maxSlotPerColumn)
                    battleEnemy.Init(entireMapData.EnemyDataList[i], new Vector2(5, i), FaceDirection.Front, ActiveCharacterStatus);
                else
                    battleEnemy.Init(entireMapData.EnemyDataList[i], new Vector2(4, i - 3), FaceDirection.Front, ActiveCharacterStatus);

                characterList.Add(battleEnemy);
            }

            uiCharacterPanel.StartBattle(characterList);

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
        currentRound = 0;
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
                Vector2 targetPos = await uiCharacterPanel.ShowEffectArea(GetAllPosList(), true, false, true);
                _characterList.Add(characterList.Find(x => x.currentPos == targetPos));
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
                Vector2 pos = character.currentPos;

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
            currentRound++;

            currentMP++;
            if (currentMP > maxMP)
                currentMP = maxMP;

            uIBattleCardsPanel.UpdateMP(currentMP, maxMP);

            for (int i = 0; i < characterList.Count; i++)
            {
                if (characterList[i].IsPlayerCharacter())
                {
                    characterList[i].StartTurn();
                    Card card = ((BattlePlayerCharacter)characterList[i]).ProcessCountdownCard();
                    if (card != null)
                    {
                        await ProcessUsingCard(card, true);
                    }
                    uiCharacterPanel.UpdateHP(characterList[i].currentPos, characterList[i].GetCharacterData().CurHP, false);
                    uiCharacterPanel.UpdateStatus(characterList[i].currentPos, characterList[i].statusDic, false);
                }
            }

            if (uIBattleCardsPanel.GetCurrentCardListCount() == 0)
            {
                await uIBattleCardsPanel.DrawCard();
            }
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
            BattleCharacter character = characterList.Find(x => x.GetCharacterData().ID == card._characterData.ID);

            if (!character.CheckSuccessWithStatus(card._cardData.Type, false))
                return false;

            if (!await ProcessUsingCard(card, false))
                return false;

            if (!UseMP(character, card._cardData.Cost))
                return false;

            uIBattleCardsPanel.UpdateMP(currentMP, maxMP);

            bool isAllDead = await CheckAllCharacterDie();
            if (isAllDead)
            {
                RunStage(BattleStage.EndGame);
                return true;
            }

            if (currentMP == 0)
                EndPlayerTurn();

            return true;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            return false;
        }
    }

    public async void PlayerSelectEndTurn()
    {
        if (currentStage == BattleStage.PlayerTurn)
        {
            await uIBattleCardsPanel.DrawCard();
            await EndPlayerTurn();
        }
    }

    private async Task EndPlayerTurn()
    {
        try
        {
            uIBattleCardsPanel.EndPlayerTurn();
            await Task.Delay(100);
            for (int i = 0; i < characterList.Count; i++)
            {
                if (characterList[i].IsPlayerCharacter())
                {
                    characterList[i].EndTurn();
                }
                uiCharacterPanel.UpdateStatus(characterList[i].currentPos, characterList[i].statusDic, false);
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
                    uiCharacterPanel.UpdateStatus(characterList[i].currentPos, characterList[i].statusDic, false);
                    uiCharacterPanel.UpdateHP(characterList[i].currentPos, characterList[i].GetCharacterData().CurHP, false);
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

                Vector2 pos = characterList[i].currentPos;
                var (skillData, countDown) = await enemy.DoAction(pos, GetPlayerCharactersPosList(), GetEnemyCharactersPosList(), boardPosList);

                if (skillData != null && countDown == 0)
                {
                    if (skillData.Type == CardType.Attack)
                    {
                        if(skillData.Countdown > 0) //need to remove show effect
                        {
                            List<Vector2> effectPosList = FindEnemySkillEffectArea(skillData, pos, characterList[i].GetFaceDirection(), skillData.Countdown);
                            uiCharacterPanel.ClearEffectArea(effectPosList);
                        }


                        Vector2 newpos = FindEnemySkillBestPos(skillData, pos, characterList[i].GetFaceDirection(), countDown);
                        if (newpos == pos)  //avoid attack himself
                            continue;
                        await uiCharacterPanel.Attack(pos, newpos, true);
                        BattleCharacter target = characterList.Find(x => x.currentPos == newpos);                     

                        if (target != null)
                        {
                            int hp = target.BeAttacked((int)(skillData.Value * characterList[i].GetCharacterData().Attack));
                            await uiCharacterPanel.UpdateHP(newpos, hp, true);
                        }
                    }
                    else if (skillData.Type == CardType.Move)
                    {
                        Vector2 newpos = FindEnemySkillBestPos(skillData, pos, characterList[i].GetFaceDirection(), countDown);
                        characterList[i].BeMoved(newpos, skillData.Direction);
                        bool isSuccess = await uiCharacterPanel.MoveCharacter(pos, newpos, skillData.Direction, characterList[i].GetCharacterData());
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
                    List<Vector2> effectPosList = FindEnemySkillEffectArea(skillData, pos, characterList[i].GetFaceDirection(), countDown);
                    if(effectPosList.Count > 0) 
                        uiCharacterPanel.ShowEffectArea(effectPosList, false, false, false);
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

    private List<Vector2> GetEnemyCharactersPosList()
    {
        List<Vector2> result = new List<Vector2>();
        for (int i = 0; i < characterList.Count; i++)
        {
            if (!characterList[i].IsPlayerCharacter())
            {
                result.Add(characterList[i].currentPos);
            }
        }

        return result;
    }

    private List<Vector2> GetAllCharacterPosList()
    {
        List<Vector2> result = new List<Vector2>();
        for (int i = 0; i < characterList.Count; i++)
        {
            result.Add(characterList[i].currentPos);
        }

        return result;
    }

    private List<Vector2> GetAllPosList()
    {
        List<Vector2> result = new List<Vector2>();
        for (int x = 0; x < uiCharacterPanel.maxSlotPerRow; x++)
        {
            for (int y = 0; y < uiCharacterPanel.maxSlotPerRow; y++)
            {
                result.Add(new Vector2(x, y)) ;
            }
        }

        return result;
    }

    private List<Vector2> GetPlayerCharactersPosList()
    {
        List<Vector2> result = new List<Vector2>();
        for (int i = 0; i < characterList.Count; i++)
        {
            if (characterList[i].IsPlayerCharacter())
            {
                result.Add(characterList[i].currentPos);
            }
        }

        return result;
    }

    private List<Vector2> FindEnemySkillEffectArea(EnemySkillData skill, Vector2 pos, FaceDirection faceDirection, int countdown)
    {
        if (skill.Target == EnemyActionTarget.Self)
            return new List<Vector2>() { pos };

        List<Vector2> effectPosList = new List<Vector2>();

        if (skill.Type == CardType.Move || countdown > 0)
            effectPosList = GetEffectArea(pos, (int)skill.Value, skill.Direction,  skill.Type == CardType.Move);
        else if (skill.Type == CardType.Heal)
            effectPosList = GetEffectArea(pos, (int)skill.Value, skill.Direction, false);
        else
            effectPosList = GetEffectArea(pos, (int)skill.Value, skill.Direction, false);

        return effectPosList;
    }

    private Vector2 FindEnemySkillBestPos(EnemySkillData skill, Vector2 pos, FaceDirection faceDirection, int countdown)
    {
        float compareValue = -1;

        if (skill.Target == EnemyActionTarget.Self)
            return pos;

        List<Vector2> effectPosList = FindEnemySkillEffectArea(skill, pos, faceDirection, countdown);
        BattleCharacter targetCharacterdata = null;

        for (int i = 0; i < effectPosList.Count; i++)
        {
            BattleCharacter characterdata = characterList.Find(x => x.currentPos == effectPosList[i]);

            if (characterdata == null)
                continue;

            var data = characterdata.GetCharacterData();

            if (data != null && characterdata.IsPlayerCharacter())
            {
                if (compareValue == -1 )
                {
                    if(skill.Target == EnemyActionTarget.Lowest || skill.Target == EnemyActionTarget.Hightest)
                        compareValue = data.CurHP;
                    else if (skill.Target == EnemyActionTarget.Nearest || skill.Target == EnemyActionTarget.Farest)
                        compareValue = CalculateDistance(pos, effectPosList[i]);

                    targetCharacterdata = characterdata;
                }
                else
                {
                    if (skill.Target == EnemyActionTarget.Lowest && compareValue > data.CurHP)
                    {
                        compareValue = data.CurHP;
                        targetCharacterdata = characterdata;
                    }
                    else if (skill.Target == EnemyActionTarget.Hightest && compareValue < data.CurHP)
                    {
                        compareValue = data.CurHP;
                        targetCharacterdata = characterdata;
                    }
                    else if (skill.Target == EnemyActionTarget.Nearest && compareValue < CalculateDistance(pos, effectPosList[i]))
                    {
                        compareValue = CalculateDistance(pos, effectPosList[i]);
                        targetCharacterdata = characterdata;
                    }
                    else if (skill.Target == EnemyActionTarget.Farest && compareValue > CalculateDistance(pos, effectPosList[i]))
                    {
                        compareValue = CalculateDistance(pos, effectPosList[i]);
                        targetCharacterdata = characterdata;
                    }
                }
            }
        }

        if (skill.Type == CardType.Move)
        {
            if (targetCharacterdata == null)
            {
                if (skill.Target == EnemyActionTarget.Nearest)
                {
                    float distance = 99;
                    for (int i = 0; i < characterList.Count; i++) {
                        if (CalculateDistance(characterList[i].currentPos, pos) < distance && characterList[i].IsPlayerCharacter())
                        {
                            distance = CalculateDistance(characterList[i].currentPos, pos);
                            targetCharacterdata = characterList[i];
                        }    
                    }
                }
                else if (skill.Target == EnemyActionTarget.Farest)
                {
                    float distance = -1;
                    for (int i = 0; i < characterList.Count; i++)
                    {
                        if (CalculateDistance(characterList[i].currentPos, pos) > distance && characterList[i].IsPlayerCharacter())
                        {
                            distance = CalculateDistance(characterList[i].currentPos, pos);
                            targetCharacterdata = characterList[i];
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
                            targetCharacterdata = characterList[i];
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
                            targetCharacterdata = characterList[i];
                        }
                    }
                }
            }

            Vector2 closePos = new Vector2(pos.x, pos.y);
            for(int i = 0; i < effectPosList.Count; i++)
            {
                if (CalculateDistance(closePos, targetCharacterdata.currentPos) > CalculateDistance(effectPosList[i] , targetCharacterdata.currentPos))
                {
                    closePos = effectPosList[i];
                }
            }

            return closePos;
        }

        if (targetCharacterdata != null)
            return targetCharacterdata.currentPos;
        else
            return pos;
    }

    private async Task EndEnemyTurn()
    {
        for (int i = 0; i < characterList.Count; i++)
        {
            if (!characterList[i].IsPlayerCharacter())
            {
                characterList[i].EndTurn();
            }
            uiCharacterPanel.UpdateStatus(characterList[i].currentPos, characterList[i].statusDic, false);
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
                await uiCharacterPanel.CharacterDie(characterList[i].currentPos);
                characterList[i].BeMoved(new Vector2(-1, -1), FaceDirection.Back);
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

    private bool UseMP(BattleCharacter character, int cost)
    {
        if(character.statusDic.ContainsKey(CharacterStatus.AfterPrepare))
        {
            character.BeAddStatus(CardEffectType.AfterPrepare, -1);
            return true;
        }
        else if (currentMP - cost >= 0)
        {
            currentMP -= cost;
            return true;
        }

        return false;
    }

    private async Task<bool> ProcessUsingCard(Card card, bool ignoreCountdown)
    {
        var characterIndex = characterList.FindIndex(x => x.GetCharacterData().ID == card._characterData.ID);
        if (characterIndex == -1)
        {
            Debug.LogError("UsingCard character null");
            return false;
        }
        var pos = characterList[characterIndex].currentPos;
        var character = (BattlePlayerCharacter)characterList[characterIndex];
        var effectValue = card._cardData.Value;

        //Check MP
        if (character.statusDic.ContainsKey(CharacterStatus.AfterPrepare)) //No need MP cost
        {

        }
        else if (currentMP - card._cardData.Cost < 0)
            return false;
        else if (character.IsCastingCard())
            return false;

        if(card._cardData.Countdown > 0 && !ignoreCountdown)
        {
            character.UseCountdownCard(card);
            uiCharacterPanel.CharacterCasting(character.currentPos);
            return true;
        }

        if (card._cardData.EffectType == CardEffectType.Attack)
            effectValue = Mathf.RoundToInt(card._cardData.Value * character.GetCharacterData().Attack);

        if (card._cardData.Direction != character.GetFaceDirection() && card._cardData.Direction != FaceDirection.NA) //Rotate character first
        {
            await uiCharacterPanel.MoveCharacter(pos, pos, card._cardData.Direction, character.GetCharacterData());
            character.BeMoved(pos, card._cardData.Direction);
        }

        List<Vector2> newPosList = GetEffectPosList(pos, card._cardData.posList, card._cardData.Type == CardType.Move);
        bool firstAction = true;
        for(int j = 0; j < newPosList.Count; j++)
        {
            Vector2 newpos = newPosList[j];
            BattleCharacter target  = characterList.Find(x => x.currentPos == newpos);

            if (target == null && card._cardData.Type != CardType.Move)
                continue;
            else if (target != null && !target.CheckSuccessWithStatus(card._cardData.Type, target == character))
            {
                uiCharacterPanel.UpdateStatus(target.currentPos, target.statusDic, true);
                continue;
            }
              
            if (card._cardData.Type == CardType.Move)
            {
                bool isSucess = await uiCharacterPanel.MoveCharacter(pos, newpos, card._cardData.Direction, character.GetCharacterData());
                if (isSucess)
                    character.BeMoved(newpos, card._cardData.Direction);
                else
                    return false;
            }
            else if (card._cardData.Type == CardType.Heal)
            {
                await uiCharacterPanel.Heal(pos, newpos);
                int hp = target.BeHealed((int)effectValue);
                await uiCharacterPanel.UpdateHP(newpos, hp, firstAction);
            }
            else
            {
                if (target.statusDic.ContainsKey(CharacterStatus.Dogde))
                    effectValue = 0;

                await uiCharacterPanel.Attack(pos, newpos, firstAction);

                if (card._cardData.EffectType == CardEffectType.Attack)
                {
                    if (target.statusDic.ContainsKey(CharacterStatus.Weakness))
                        effectValue *= 2;

                    if (target.statusDic.ContainsKey(CharacterStatus.Defense))
                    {
                        effectValue = 1;
                        var statusDic = target.BeAddStatus( CardEffectType.Defense, -1);
                        await uiCharacterPanel.UpdateStatus(newpos, statusDic, firstAction);
                    }

                    int hp = target.BeAttacked((int)effectValue);
                    await uiCharacterPanel.UpdateHP(newpos, hp, firstAction);
                }
                else if (card._cardData.EffectType == CardEffectType.Push)
                {
                    Vector2 targetNewPos = new Vector2();
                    if(card._cardData.Direction == FaceDirection.Front)
                        targetNewPos = new Vector2(target.currentPos.x+1, target.currentPos.y);
                    else
                        targetNewPos = new Vector2(target.currentPos.x - 1, target.currentPos.y);

                    bool isSucess = await uiCharacterPanel.MoveCharacter(target.currentPos, targetNewPos, card._cardData.Direction, target.GetCharacterData());
                    if (isSucess)
                        target.BeMoved(targetNewPos, target.GetFaceDirection());
                }
                else if (effectValue != 0)
                {
                    if (target.IsPlayerCharacter())
                        effectValue += 0.5f;
                    var statusDic = target.BeAddStatus(card._cardData.EffectType, effectValue);
                    await uiCharacterPanel.UpdateStatus(newpos, statusDic, firstAction);
                }

                firstAction = false;
            }

            if(target != null)
                uiCharacterPanel.UpdateStatus(target.currentPos, target.statusDic, true);
        }

        uiCharacterPanel.UpdateStatus(character.currentPos, character.statusDic, true);

        return true;
    } 
    
    private List<Vector2> GetEffectPosList(Vector2 pos, List<Vector2> effectPosList, bool avoidCharacter)
    {

        if(effectPosList.Count == 0)
        {
            return new List<Vector2> { pos };
        }

        List<Vector2> vectors = new List<Vector2>();

        for (int i = 0; i < effectPosList.Count; i++)
        {
            Vector2 newPos = pos + effectPosList[i];
            var charIndex = characterList.FindIndex(x => x.currentPos == newPos);

            if (newPos.x < 0 || newPos.x > uiCharacterPanel.maxSlotPerRow - 1 || newPos.y < 0 || newPos.y > uiCharacterPanel.maxSlotPerColumn - 1)
                continue;
            else if (charIndex != -1 && avoidCharacter)
                continue;

            vectors.Add(newPos);
        }

        return vectors;
    }

    private List<Vector2> GetEffectArea(Vector2 pos, int distance, FaceDirection direction , bool avoidCharacter)
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
            else 
            {
                if (direction == FaceDirection.Front)
                {
                    vectors.Add(new Vector2(pos.x + i, pos.y));
                }
                else if (direction == FaceDirection.Back)
                {
                    vectors.Add(new Vector2(pos.x - i, pos.y));
                }
                else if (direction == FaceDirection.Up)
                {
                    vectors.Add(new Vector2(pos.x, pos.y-i));
                }
                else if (direction == FaceDirection.Down)
                {
                    vectors.Add(new Vector2(pos.x, pos.y + i));
                }
            }
           
        }

        List<Vector2> resultVectors = new List<Vector2>(vectors);

        for (int i = 0; i < vectors.Count; i++)
        {
            var charIndex = characterList.FindIndex(x => x.currentPos == vectors[i]);

            if (vectors[i].x < 0 || vectors[i].x > uiCharacterPanel.maxSlotPerRow - 1 || vectors[i].y < 0 || vectors[i].y > uiCharacterPanel.maxSlotPerColumn - 1)
                resultVectors.Remove(vectors[i]);
            else if (charIndex != -1 && avoidCharacter)
                resultVectors.Remove(vectors[i]);
        }

        return resultVectors;
    }

    private int CalculateDistance(Vector2 posA, Vector2 posB)
    {
        float distance = 0;
        distance = Mathf.Abs(posA.x - posB.x);
        distance += Mathf.Abs(posA.y - posB.y);

        return Mathf.RoundToInt(distance);
    }

    private void ActiveCharacterStatus(CharacterData characterData, CharacterStatus characterStatus)
    {
        if(characterData == null) return;

    }

}
