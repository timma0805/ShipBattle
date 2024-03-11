using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGameData", menuName = "CardDataList")]
public class CardDataListSO : ScriptableObject
{
    public List<CardData> cardDataList = new List<CardData>();
}
