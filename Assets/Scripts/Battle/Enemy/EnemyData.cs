using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class EnemyData
{
    public int ID;
    public string Name;
    public CharacterType Type;
    public int HP;
    public float Speed;
    public string SkillSetStr;
    public string Description;

    public EnemyData()
    {
    }
}
