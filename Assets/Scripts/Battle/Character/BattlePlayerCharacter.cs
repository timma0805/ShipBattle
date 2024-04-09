using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePlayerCharacter : ITargetObject
{
    private BattlePlayerCharacterData characterData;
    public List<CharacterStatus> playerStatusList { get; private set; }
    public CharacterState playerState { get; private set; }
    private FaceDirection currentDirection;
    public Vector2 currentPos { get; private set; }

    private bool CanCancel;
    private int specialCountdown;

    public void Init(BattlePlayerCharacterData data)
    {
        characterData = data;

        //init
        playerStatusList = new List<CharacterStatus>();
        playerState = CharacterState.Idle;
        CanCancel = true;
    }



    public int BeAttacked(int value)
    {
        Debug.Log("BattlePlayer BeAttacked" + value);
        characterData.CurHP -= value;
        Debug.Log("playerData.CurHP" + characterData.CurHP);
        if(characterData.CurHP <= 0 )
            playerState = CharacterState.Dead;

        return characterData.CurHP;
    }

    public void BeMoved(Vector2 pos, FaceDirection rotation)
    {
        Debug.Log("BattlePlayer BeMoved" + pos.x + pos.y + rotation);
        currentPos = pos;
        currentDirection = rotation;
    }

    public void BeTarget()
    {
    }

    public int BeHealed(int value)
    {
        characterData.CurHP += value;
        if (characterData.CurHP > characterData.HP)
            characterData.CurHP = characterData.HP;

            Debug.Log("playerData.CurHP" + characterData.CurHP);

        return characterData.CurHP;
    }

    public void StartTurn()
    {
        Debug.Log($"StartTurn: CurHP {characterData.CurHP}");

        //recovery
        //Gain HP and MP when Start
        //Gain MP when Start
        //if (playerData.CurShipBodyHP < playerData.MaxHP)
        //    playerData.HP += playerData.regHP;

        //if (playerData.MP < playerData.MaxMP)
        //    playerData.MP += playerData.regMP;
    }

    public bool IsPlayerCharacter()
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
        if (playerState == CharacterState.Dead)
            return true;

        return characterData.CurHP <= 0;
    }

    public bool IsRemoved()
    {
        return playerState == CharacterState.Removed;
    }

    public void RemoveFromBattle()
    {
        playerState = CharacterState.Removed;
    }
}
