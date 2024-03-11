using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class EnemyData
{
    public string Name;
    public int HP;
    public int MP;
    public float Speed;
    public int width;
    public int length;
    public float weight;
    public GameObject prefab;
    public List<EnemyPartData> subParts;

    public EnemyData()
    {
        Name = "Enemy";
        HP = 100;
        MP = 0;
        width = 2;
        length = 2;
        Speed = 1;
        weight = 20;
    }
}
