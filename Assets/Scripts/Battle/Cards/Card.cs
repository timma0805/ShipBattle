using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CardData;

public class Card
{
    public CardData data { get; private set; }


    public Card(CardData _data)
    {
        data = _data;
        effects = new List<CardEffect>();

        CardEffect effect = new CardEffect(_data.Target, _data.Effect, _data.Value, _data.Distance, _data.Direction, _data.Success, _data.IsAreaEffect);
        effects.Add(effect);

        if(_data.Target2 != null)
        {
            CardEffect effect2 = new CardEffect(_data.Target2, _data.Effect2, _data.Value2, _data.Distance2, _data.Direction2, _data.Success2, _data.IsAreaEffect2);
            effects.Add(effect2);
        }
    }

    public List<CardEffect> effects { get; private set; }


    public string GetCardTypeName()
    {
        if (data.Type == CardType.Attack )
            return "Attack";
        else if (data.Type == CardType.Defend)
            return "Defense";
        else if (data.Type == CardType.MovePosition || data.Type == CardType.MoveShip)
            return "Movement";
        else if (data.Type == CardType.Other)
            return "Special";
        else
            return "Common";
    }

    public string GetCardDetailString()
    {
        string detailStr = "";

        //if (data.conditionList.Count == 0)
        //    detailStr += "No Condition\n";
        //else
        //{
        //    for(int i = 0; i < data.conditionList.Count; i++)
        //    {
        //        var condition = data.conditionList[i];
        //        detailStr += "Condition: \n Target: " + condition.effectTarget + condition.conditionValue + condition.conditionComapre + condition.conditionCompareValueStr + "\n";
        //    }
        //}

        //if (data.effectList.Count == 0)
        //    detailStr += "No Effect\n";
        //else
        //{
        //    for (int i = 0; i < data.effectList.Count; i++)
        //    {
        //        var effect = data.effectList[i];
        //        detailStr += "Effect: \n Target: " + effect.effectTarget + effect.effectProperty + effect.effectValueStr+ "\n";
        //    }
        //}

        return detailStr;
    }
}
