using Newtonsoft.Json.Linq;
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
    private Action<CharacterData,CharacterStatus> activeStatusCallback;


    public virtual void Init(CharacterData data, Vector2 pos, FaceDirection direction, Action<CharacterData,CharacterStatus> statusCallback)
    {
        //init
        characterData = data;
        state = CharacterState.Idle;
        statusDic = new Dictionary<CharacterStatus, float>();
        currentPos = pos;
        currentDirection = direction;
        activeStatusCallback = statusCallback;
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
        ActiveStatus();
    }

    protected virtual void ActiveStatus()
    {
        if (statusDic.Count > 0)
        {
            List<CharacterStatus> keys = new List<CharacterStatus>(statusDic.Keys);
            foreach (CharacterStatus key in keys)
            {
                if (Math.Truncate(statusDic[key]) == statusDic[key])
                {
                    if (key == CharacterStatus.Shield) //Shield will disappear after a round
                        statusDic[key] = 0;
                    else
                        statusDic[key] = statusDic[key] - 1.0f;

                    ProcessStatus(key);
                    if (activeStatusCallback  != null)
                        activeStatusCallback(characterData, key);
                }
                else
                    statusDic[key] = statusDic[key] - 0.5f;

                if (statusDic[key] <= 0)
                {
                    statusDic.Remove(key);
                }
            }
        }
    }
    protected virtual void ProcessStatus(CharacterStatus status)
    {
        if (status == CharacterStatus.Bleed || status == CharacterStatus.Fire || status == CharacterStatus.Posion)
            characterData.CurHP -= (int)statusDic[status];
        else if (status == CharacterStatus.AfterPrepare)
        {
            if (!statusDic.ContainsKey(status))
                statusDic.Add(status, 1);
        }
    }

    public bool CheckSuccessWithStatus(CardType cardType, bool isTarget)
    {
        if(statusDic.ContainsKey(CharacterStatus.Stun) && !isTarget)
            return false;
        else if (statusDic.ContainsKey(CharacterStatus.Untargetable) && isTarget)
            return false;

        if (cardType == CardType.Attack || cardType == CardType.Special)
        {
            if(statusDic.ContainsKey(CharacterStatus.Dogde) && isTarget)
                return false;

            if (statusDic.ContainsKey(CharacterStatus.Blind) && !isTarget)
                return false;

            if (statusDic.ContainsKey(CharacterStatus.Shield) && isTarget)
            {
                statusDic.Remove(CharacterStatus.Shield);
                return false;
            }
        }
        else if(cardType == CardType.Move)
        {
            if (statusDic.ContainsKey(CharacterStatus.Unmovement) && !isTarget)
                return false;
        }

        return true;
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
                {
                    statusDic.Add((CharacterStatus)effectType, value);
                }

                if (statusDic[(CharacterStatus)effectType] <= 0)
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
        ActiveStatus();
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
