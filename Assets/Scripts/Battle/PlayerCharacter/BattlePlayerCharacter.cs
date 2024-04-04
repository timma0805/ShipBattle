using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePlayerCharacter : ITargetObject
{
    public BattlePlayerCharacterData characterData { get; private set; }
    public List<CharacterStatus> playerStatusList { get; private set; }
    public CharacterState playerState { get; private set; }
    public FaceDirection currentDirection { get; private set; }
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



    public void BeAttacked(int value)
    {
        Debug.Log("BattlePlayer BeAttacked" + value);
        characterData.CurHP += value;
        Debug.Log("playerData.HP" + characterData.CurHP);
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

    public void BeDefenced(int value)
    {
        Debug.Log("BattlePlayer BeDefenced" + value);
        characterData.CurHP += value;
        Debug.Log("playerData.HP" + characterData.CurHP);
    }

    public void StartTurn()
    {
        Debug.Log($"StartTurn: HP {characterData.CurHP}");

        //recovery
        //Gain HP and MP when Start
        //Gain MP when Start
        //if (playerData.CurShipBodyHP < playerData.MaxHP)
        //    playerData.HP += playerData.regHP;

        //if (playerData.MP < playerData.MaxMP)
        //    playerData.MP += playerData.regMP;
    }
}
