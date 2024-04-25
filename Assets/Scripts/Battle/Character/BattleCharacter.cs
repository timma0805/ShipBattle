using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class BattleCharacter
{
    protected CharacterData characterData;
    public Dictionary<CharacterStatus, float> statusDic { get; private set; }
    public CharacterState state { get; private set; }
    protected FaceDirection currentDirection;
    public Vector2 currentPos { get; private set; }

    public virtual void Init(CharacterData data, Vector2 pos, FaceDirection direction)
    {
        characterData = data;

        //init
        state = CharacterState.Idle;
        statusDic = new Dictionary<CharacterStatus, float>();
        currentPos = pos;
        currentDirection = direction;
    }

    public virtual int BeAttacked(int value)
    {
        Debug.Log("BeAttacked" + value);

        if(statusDic.ContainsKey(CharacterStatus.Shield))
        {
            if(value > statusDic[CharacterStatus.Shield]) {
                value -= (int)statusDic[CharacterStatus.Shield];
                statusDic[CharacterStatus.Shield] = 0;
            }
            else
            {
                statusDic[CharacterStatus.Shield] -= value;
                value = 0;
            }
        }

        characterData.CurHP -= value;
        Debug.Log("CurHP" + characterData.CurHP);
        if (characterData.CurHP <= 0)
            state = CharacterState.Dead;

        return characterData.CurHP;
    }

    public virtual void BeMoved(Vector2 pos, FaceDirection direction)
    {
        Debug.Log("BeMoved" + pos.x + pos.y + direction);
        currentPos = pos;
        if(direction == FaceDirection.Front || direction == FaceDirection.Back) 
            currentDirection = direction;
    }

    public virtual int BeHealed(int value)
    {
        characterData.CurHP += value;
        if (characterData.CurHP > characterData.HP)
            characterData.CurHP = characterData.HP;

        Debug.Log("CurHP" + characterData.CurHP);

        return characterData.CurHP;
    }

    public virtual void StartTurn()
    {
        CheckStatus();
    }

    protected virtual void CheckStatus()
    {
        if (statusDic.Count > 0)
        {
            foreach (var status in statusDic)
            {
                if(status.Key == CharacterStatus.Bleed || status.Key == CharacterStatus.Fire || status.Key == CharacterStatus.Freeze || status.Key == CharacterStatus.Posion)
                    characterData.CurHP -= (int)status.Value;
            }

            List<CharacterStatus> keys = new List<CharacterStatus>(statusDic.Keys);
            foreach (CharacterStatus key in keys)
            {
                if (Math.Truncate(statusDic[key]) == statusDic[key])
                {
                    if (key == CharacterStatus.Shield)
                        statusDic[key] = 0;
                    else
                        statusDic[key] = statusDic[key] - 1.0f;
                }
                else
                    statusDic[key] = statusDic[key] - 0.5f;

                if (statusDic[key] <= 0)
                {
                    if (key == CharacterStatus.IncreaseAttack)
                        characterData.Attack /= 2;

                    statusDic.Remove(key);
                }
            }
        }
    }

    public virtual Dictionary<CharacterStatus, float> BeAddStatus(CardEffectType effectType, float value)
    {
        try
        {
            if (effectType.Equals(CardEffectType.RemoveStatus))
            {
                statusDic.Clear();
            }
            else
            {
                if (statusDic.ContainsKey((CharacterStatus)effectType))
                    statusDic[(CharacterStatus)effectType] += value;
                else
                    statusDic.Add((CharacterStatus)effectType, value);

                if(effectType == CardEffectType.IncreaseAttack)
                    characterData.Attack *= 2;

                if(statusDic[(CharacterStatus)effectType] <= 0)
                    statusDic.Remove((CharacterStatus)effectType);
            }

            return statusDic;
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
            return statusDic;
        }
    }

    public virtual void EndTurn()
    {
        CheckStatus();
    }

    public virtual bool IsPlayerCharacter()
    {
        return true;
    }

    public CharacterData GetCharacterData()
    {
        return characterData;
    }

    public FaceDirection GetFaceDirection()
    {
        return currentDirection;
    }

    public bool IsDead()
    {
        if (state == CharacterState.Dead)
            return true;

        return characterData.CurHP <= 0;
    }

    public bool IsRemoved()
    {
        return state == CharacterState.Removed;
    }

    public void RemoveFromBattle()
    {
        state = CharacterState.Removed;
    }
}
