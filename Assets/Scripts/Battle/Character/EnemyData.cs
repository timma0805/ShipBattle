using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class EnemyData: CharacterData
{ 
    public string SkillSetStr;
    
    public List<EnemySkillData> SkillList;
    public EnemyData()
    {
    }
}
