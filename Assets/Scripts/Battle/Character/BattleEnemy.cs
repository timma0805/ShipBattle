using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;

public class BattleEnemy : ITargetObject
{
    private CharacterState enemyState;

    public int Countdown { get; private set; }
    private FaceDirection currentDirection;

    private EnemyData enemyData;
    private EnemySkillData curEnemySkill;
    private EnemySkillData previosEnemySkill;
    public void Init(EnemyData _enemyData)
    {
        enemyData = _enemyData;
        Countdown = -1;
        enemyState = CharacterState.Idle;
        enemyData.CurHP =enemyData.HP;
    }

    public async Task<EnemySkillData> DoAction()
    {
        if(curEnemySkill != null)
        {
            Countdown--;
        }
        else
        {
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

        if (Countdown == 0)
            return curEnemySkill;
        else
        {
            Debug.Log("Countdown: " +Countdown);
            return null;
        }
    }

    private bool CheckSkillCanUseOrNot(EnemySkillData skill)
    {
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

    public void EndTurn()
    {
        if (Countdown == 0 && curEnemySkill != null)
            curEnemySkill = null;
    }

    public void BeTarget()
    {
        throw new System.NotImplementedException();
    }

    public int BeAttacked(int value)
    {
        enemyData.CurHP -= value;
        Debug.Log("enemyData.HP" + enemyData.CurHP);

        if (enemyData.CurHP <= 0)
            enemyState = CharacterState.Dead;

        return enemyData.CurHP;
    }

    public void BeMoved(Vector2 pos, FaceDirection rotation)
    {
        throw new System.NotImplementedException();
    }

    public int BeHealed(int value)
    {
        enemyData.CurHP += value;
        if (enemyData.CurHP > enemyData.HP)
            enemyData.CurHP = enemyData.HP;

        Debug.Log("enemyData.CurHP" + enemyData.CurHP);

        return enemyData.CurHP;
    }

    public bool IsDead()
    {
        if (enemyState == CharacterState.Dead)
            return true;

        return enemyData.CurHP <= 0;
    }

    public bool IsRemoved()
    {
        return enemyState == CharacterState.Removed;
    }

    public void RemoveFromBattle()
    {
        enemyState = CharacterState.Removed;
    }

    public bool IsPlayerCharacter()
    {
        return false;
    }

    public CharacterData GetCharacterData()
    {
        return enemyData;
    }

    public FaceDirection GetFaceDirection()
    {
        return currentDirection;
    }
}
