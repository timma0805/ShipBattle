using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEnemy : ITargetObject
{
    private List<CharacterStatus> enemyStatusList;
    private CharacterState enemyState;

    private bool CanCancel;
    private int specialCountdown;
    private FaceDirection currentDirection;
    private List<BattleMapTile> currentTiles;

    private EnemyData enemyData;

    public void Init(EnemyData _enemyData)
    {
        enemyData = _enemyData;
        enemyStatusList = new List<CharacterStatus>();
        currentTiles = new List<BattleMapTile>();
        specialCountdown = -1;
        enemyState = CharacterState.Idle;
        enemyData.CurHP =enemyData.HP;
    }

    public void DoAction()
    {
    }

    public void BeTarget()
    {
        throw new System.NotImplementedException();
    }

    public int BeAttacked(int value)
    {
        enemyData.CurHP -= value;
        Debug.Log("enemyData.HP" + enemyData.CurHP);

        if (enemyData.HP <= 0)
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

        return enemyData.HP <= 0;
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
