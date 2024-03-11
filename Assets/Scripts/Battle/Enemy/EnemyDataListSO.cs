using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGameData", menuName = "EnemyDataList")]
public class EnemyDataListSO : ScriptableObject
{
    public List<EnemyData> enemyDataList = new List<EnemyData>();
}
