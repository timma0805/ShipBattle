using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;

public class BattleEnemy : BattleCharacter
{
    public int Countdown { get; private set; }

    public EnemySkillData curEnemySkill { get; private set; }
    private EnemySkillData previosEnemySkill;
    public List< Vector2> effectPosList { get; private set; }


    public async Task<(EnemySkillData, int)> DoAction(Vector2 pos, List<Vector2> playerPosList, List<Vector2> enemyPosList, List<Vector2> avaliablePosList)
    {
        if(curEnemySkill != null)
        {
            Countdown--;
        }
        else
        {
            EnemyData enemyData = (EnemyData)base.characterData;
            previosEnemySkill = curEnemySkill;

            List<EnemySkillData> avaliableList = new List<EnemySkillData>();
            for(int i = 0; i < enemyData.SkillList.Count; i++)
            {
                var data = enemyData.SkillList[i];
                if(CheckSkillCanUseOrNot(data, pos, playerPosList, enemyPosList, avaliablePosList))
                    avaliableList.Add(data);
            }

            if (avaliableList.FindIndex(x => x.Type == CardType.Attack || x.Type == CardType.Special) > 0)
                avaliableList = avaliableList.FindAll(x => x.Type == CardType.Attack || x.Type == CardType.Special);

            int randomIndex = Random.Range(0, avaliableList.Count);
            curEnemySkill = avaliableList[randomIndex];
            Countdown = curEnemySkill.Countdown;
        }

        return (curEnemySkill, Countdown);
    }

    public void SaveEffectPosList(List<Vector2> vectors)
    {
        effectPosList = vectors;
    }

    public override void EndTurn()
    {
        if (curEnemySkill != null && Countdown == 0)
        {
            curEnemySkill = null;
            effectPosList = new List<Vector2>();
        }

        base.EndTurn();
    }

    private bool CheckSkillCanUseOrNot(EnemySkillData skill, Vector2 pos, List<Vector2> playerPosList, List<Vector2> enemyPosList, List<Vector2> avaliablePosList)
    {
        bool canUse = true;
        EnemyData enemyData = (EnemyData)base.characterData;

        //Check Compare
        int compareValue = 0;
        if (skill.ConditionTarget != null)
        {
            if (skill.ConditionTarget == CardEffectTarget.Self)
                characterData = enemyData;

            if (skill.ConditionProperty == CardEffectType.HP)
                compareValue = characterData.HP;

            if (skill.ConditionCompare == ConditionCompare.Equal)
                canUse = compareValue == skill.ConditionValue.Value;
            else if (skill.ConditionCompare == ConditionCompare.Less)
                canUse = compareValue < skill.ConditionValue.Value;
            else if (skill.ConditionCompare == ConditionCompare.More)
                canUse = compareValue > skill.ConditionValue.Value;
        }

        if(!canUse)
            return false;

        canUse = false;
        if (skill.Type != CardType.Move)
        {
            for (int i = 0; i < playerPosList.Count; i++)
            {
                if (CalculateDistance(pos, playerPosList[i]) < skill.Distance)
                    return true;
            }
        }
        else
        {
            for(int i = 0;i < avaliablePosList.Count;i++)
            {
                if(CalculateDistance(pos, avaliablePosList[i]) <= skill.Distance && playerPosList.FindIndex(x => x == avaliablePosList[i]) == -1 && enemyPosList.FindIndex(x => x == avaliablePosList[i]) == -1)
                    return true;
            }
        }

        return canUse;
    }

    private int CalculateDistance(Vector2 posA, Vector2 posB)
    {
        float distance = 0;
        distance = Mathf.Abs(posA.x - posB.x);
        distance+= Mathf.Abs(posA.y - posB.y);

        return Mathf.RoundToInt(distance);
    }

    public override bool IsPlayerCharacter()
    {
        return false;
    }

    protected override void ActiveStatus()
    {
        if (statusDic.Count > 0)
        {
            foreach (var status in statusDic)
            {
                if (status.Key == CharacterStatus.Stun)
                    Countdown++;
            }
        }

        base.ActiveStatus();
    }

    public int AddCountdown(int value)
    {
        if(curEnemySkill !=  null) {
            Countdown += value;
        }

        return Countdown;
    }

}
