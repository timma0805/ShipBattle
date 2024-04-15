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

    public async Task<(EnemySkillData, int)> DoAction()
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
                if(CheckSkillCanUseOrNot(data))
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

    private bool CheckSkillCanUseOrNot(EnemySkillData skill)
    {
        EnemyData enemyData = (EnemyData)base.characterData;

        CharacterData characterData = null;
        int compareValue = 0;
        if(skill.ConditionTarget == null)   //No Condition
            return true;

        if (skill.ConditionTarget == CardEffectTarget.Self)
            characterData = enemyData;
        
        if(skill.ConditionProperty == CardEffectType.HP)
            compareValue = characterData.HP;

        if (skill.ConditionCompare == ConditionCompare.Equal)
            return compareValue == skill.ConditionValue.Value;
        else if (skill.ConditionCompare == ConditionCompare.Less)
            return compareValue < skill.ConditionValue.Value;
        else if (skill.ConditionCompare == ConditionCompare.More)
            return compareValue > skill.ConditionValue.Value;

        return true;
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

}
