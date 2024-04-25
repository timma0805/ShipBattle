using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Animation;
using UnityEngine;
using static CardData;

[Serializable]
public class Card
{
    public CardData _cardData { get; private set; }
    public CharacterData _characterData { get; private set; }


    public Card(CardData cardData, CharacterData characterData)
    {
        _cardData = cardData;
        _characterData = characterData;
    }

    public string GetCardTypeName()
    {
        if (_cardData.Type == CardType.Attack )
            return "Attack";
        else if (_cardData.Type == CardType.Defense)
            return "Defense";
        else if (_cardData.Type == CardType.Move)
            return "Movement";
        else if (_cardData.Type == CardType.Heal)
            return "Heal";
        else if (_cardData.Type == CardType.Special)
            return "Special";
        else
            return "????";
    }

    public string GetCardDetailString()
    {
        return _cardData.Description;
    }
}
