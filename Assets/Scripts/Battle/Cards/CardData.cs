using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable] // Add this attribute to make the class serializable
public class CardData
{
    public int ID;
    public string Name;
    public string Description;
    public CardType Type;
    public int Cost;
    public bool IsExhaust;
    public CardEffectType EffectType;
    public float Value;
    public FaceDirection Direction;
    public string PosStr;
    public int Countdown;

    public List<Vector2> posList;

    public CardData() {
    }

    public void ParsePosList()
    {
        try
        {
            posList = new List<Vector2>();

            if (PosStr.Contains(","))
            {
                string[] vStrList = PosStr.Split(';');
                for (int i = 0; i < vStrList.Length; i++)
                {
                    string[] vStr = vStrList[i].Split(",");
                    posList.Add(new Vector2(int.Parse(vStr[0]), int.Parse(vStr[1])));
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }


}
