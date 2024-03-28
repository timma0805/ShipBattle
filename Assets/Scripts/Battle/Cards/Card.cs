using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CardData;

[Serializable]
public class Card
{
    public CardData data { get; private set; }
    public List<CardEffect> effectList { get; private set; }


    public Card(CardData _data)
    {
        try
        {
            data = _data;
            effectList = new List<CardEffect>();

            CardEffect effect = new CardEffect(_data.Target, _data.Effect, _data.Value, _data.Distance, _data.Direction, _data.Success, _data.IsAreaEffect);
            effectList.Add(effect);

            if (_data.Target2 != string.Empty)
            {
                CardEffect effect2 = new CardEffect(_data.Target2, _data.Effect2, _data.Value2.Value, _data.Distance2.Value, _data.Direction2, _data.Success2.Value, _data.IsAreaEffect2.Value);
                effectList.Add(effect2);
            }
        }
        catch(System.Exception ex)
        {
            Debug.LogException(ex);
        }

    }

    public string GetCardTypeName()
    {
        if (data.Type == CardType.Attack )
            return "Attack";
        else if (data.Type == CardType.Defense)
            return "Defense";
        else if (data.Type == CardType.Move)
            return "Movement";
        else if (data.Type == CardType.Special)
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

        if (effectList.Count == 0)
            detailStr += "No Effect\n";
        else
        {
            for (int i = 0; i < effectList.Count; i++)
            {
                var effect = effectList[i];
                detailStr += "Effect: \n Target: " + effect.effectTarget + effect.effectProperty + effect.effectValue + "\n";
            }
        }

        return detailStr;
    }
}
