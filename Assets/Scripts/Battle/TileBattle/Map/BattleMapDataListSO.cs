using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGameData", menuName = "MapDataList")]
public class BattleMapDataListSO : ScriptableObject
{
    public List<BattleMapData> mapDataList;
}
