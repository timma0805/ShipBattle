using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CardData;

public class Card
{
    public CardData data { get; private set; }

    public Card() { }

    public Card(int _cardIndex, string _name, string _textureName, int _cost,  CardType _cardType)
    {
        data = new CardData(_cardIndex, _name, _textureName, _cost, _cardType);
    }

    public Card(CardData _data)
    {
        data = _data;
    }


    public string GetCardTypeName()
    {
        if (data.cardType == CardType.AttackLongRange || data.cardType == CardType.AttackMelee)
            return "Attack";
        else if (data.cardType == CardType.Defend)
            return "Defense";
        else if (data.cardType == CardType.MovePosition || data.cardType == CardType.MoveShip)
            return "Movement";
        else if (data.cardType == CardType.Other)
            return "Special";
        else
            return "Common";
    }

    public string GetCardDetailString()
    {
        string detailStr = "";

        if (data.conditionList.Count == 0)
            detailStr += "No Condition\n";
        else
        {
            for(int i = 0; i < data.conditionList.Count; i++)
            {
                var condition = data.conditionList[i];
                detailStr += "Condition: \n Target: " + condition.effectTarget + condition.conditionValue + condition.conditionComapre + condition.conditionCompareValueStr + "\n";
            }
        }

        if (data.effectList.Count == 0)
            detailStr += "No Effect\n";
        else
        {
            for (int i = 0; i < data.effectList.Count; i++)
            {
                var effect = data.effectList[i];
                detailStr += "Effect: \n Target: " + effect.effectTarget + effect.effectProperty + effect.effectValueStr+ "\n";
            }
        }

        return detailStr;
    }
}
