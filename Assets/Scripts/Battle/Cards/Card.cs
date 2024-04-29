using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Animation;
using UnityEngine;
using static CardData;

[Serializable]
public class Card
{
    private CardData _cardData;
    private CardData _combinedCardData;
    public CharacterData _characterData { get; private set; }

    public CardData CardData
    {
        get {
            if (_combinedCardData != null)
                return _combinedCardData;
            else
                return _cardData;
        }
    }

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

    public bool  MakeCombineCard(CardData anotherCardData)
    {
        bool canCombine = true;
        //Check Combine rule

        if(!canCombine)
            return false;

        CardData combinedCard = new CardData(_cardData) ;
        combinedCard.ID = int.Parse("9" + _cardData.ID.ToString()  + anotherCardData.ID.ToString());
        combinedCard.Name = _cardData.Name + "+";
        combinedCard.Description = _cardData.Description + "\n->\n" + anotherCardData.Description;

        return canCombine;
    }

    public string GetCardDetailString()
    {
        return _cardData.Description;
    }
}
