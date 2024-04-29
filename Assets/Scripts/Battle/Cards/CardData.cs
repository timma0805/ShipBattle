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
    public int Countdown;
    public CardEffectType EffectType;
    public float Value;
    public FaceDirection Direction;
    public string PosStr;
    public List<CardEffect> EffectList;

    public bool isCombined = false;

    public CardData() { }
    public CardData(CardData data) {
        ID = data.ID;
        Name = data.Name;
        Description = data.Description;
        Type = data.Type;
        Cost = data.Cost;
        IsExhaust = data.IsExhaust;
        EffectType = data.EffectType;
        Value = data.Value;
        Direction = data.Direction;
        PosStr = data.PosStr;
        Countdown = data.Countdown;
    }

    public void ParseEffect()
    {
        try
        {
            EffectList = new List<CardEffect>();
            List<Vector2> posList = new List<Vector2>();

            if (PosStr.Contains(","))
            {
                string[] vStrList = PosStr.Split(';');
                for (int i = 0; i < vStrList.Length; i++)
                {
                    string[] vStr = vStrList[i].Split(",");
                    posList.Add(new Vector2(int.Parse(vStr[0]), int.Parse(vStr[1])));
                }
            }

            CardEffect effect = new CardEffect(EffectType, Value, Direction, posList);
            EffectList.Add(effect);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }


}
