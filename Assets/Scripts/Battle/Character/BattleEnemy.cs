using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;

public class BattleEnemy : BattleCharacter
{
    public int Countdown { get; private set; }

    private EnemySkillData curEnemySkill;
    private EnemySkillData previosEnemySkill;

    public async Task<(EnemySkillData, int)> DoAction(Vector2 pos, List<Vector2> playerPosList)
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
                if(CheckSkillCanUseOrNot(data, pos, playerPosList))
                    avaliableList.Add(data);
            }

            int randomIndex = Random.Range(0, avaliableList.Count);
            curEnemySkill = avaliableList[randomIndex];
            Countdown = curEnemySkill.Countdown;
        }

        return (curEnemySkill, Countdown);
    }

    public override void EndTurn()
    {
        if (curEnemySkill != null && Countdown == 0)
            curEnemySkill = null;
        base.EndTurn();
    }

    private bool CheckSkillCanUseOrNot(EnemySkillData skill, Vector2 pos, List<Vector2> playerPosList)
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

        if (skill.Type != CardType.Move)
        {
            for (int i = 0; i < playerPosList.Count; i++)
            {
                if (Vector2.Distance(pos, playerPosList[i]) > skill.Distance)
                    return false;
            }
        }

        return canUse;
    }

    public override bool IsPlayerCharacter()
    {
        return false;
    }

    protected override void CheckStatus()
    {
        if (statusDic.Count > 0)
        {
            foreach (var status in statusDic)
            {
                if (status.Key == CharacterStatus.Stun)
                    Countdown++;
            }
        }

        base.CheckStatus();
    }

    public int AddCountdown(int value)
    {
        if(curEnemySkill !=  null) {
            Countdown += value;
        }

        return Countdown;
    }

}
