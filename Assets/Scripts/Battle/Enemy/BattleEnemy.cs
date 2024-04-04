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

    public EnemyData enemyData { get; private set; }

    public void Init(EnemyData _enemyData)
    {
        enemyData = _enemyData;
        enemyStatusList = new List<CharacterStatus>();
        currentTiles = new List<BattleMapTile>();
        specialCountdown = -1;
        enemyState = CharacterState.Idle;
    }

    public void DoAction()
    {
    }

    public void BeTarget()
    {
        throw new System.NotImplementedException();
    }

    public void BeAttacked(int value)
    {
        enemyData.HP += value;
        Debug.Log("enemyData.HP" + enemyData.HP);

        if (enemyData.HP <= 0)
            enemyState = CharacterState.Dead;
    }

    public void BeMoved(Vector2 pos, FaceDirection rotation)
    {
        throw new System.NotImplementedException();
    }

    public void BeDefenced(int value)
    {
        enemyData.HP += value;
        Debug.Log("enemyData.HP" + enemyData.HP);
    }

    public bool IsDead()
    {
        if (enemyState == CharacterState.Dead)
            return true;

        return enemyData.HP <= 0;
    }
}
