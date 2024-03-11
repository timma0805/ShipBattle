using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class BattleMapData
{
    [Header("Information")]
    public int mapIndex;
    public string name;
    public MapDifficulty difficulty;
    public int[] unlockMapIndexReqList;

    [Header("Setting")]
    public int Width;
    public int Length;
    public Vector2 playerSpawnPos;
    public Vector2[] enemySpawnPosList;
    public FaceDirection playerSpawnDirection;
    public FaceDirection[] enemySpawnDirectionList;

    [Header("Enemy")]
    public int[] enemyIndexList;

    [Header("Attached")]
    public Dictionary<Vector2, int> propsList;
}
