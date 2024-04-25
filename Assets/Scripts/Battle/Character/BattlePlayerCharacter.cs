using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BattlePlayerCharacter : BattleCharacter
{
    private Card countdownCard;
    private int countdown;

    public override void StartTurn()
    {
        base.StartTurn();
    }

    public override bool IsPlayerCharacter()
    {
        return true;
    }
    public void UseCountdownCard(Card card)
    {
        countdownCard = card;
        countdown = card._cardData.Countdown;
    }

    public Card ProcessCountdownCard()
    {
        if (countdownCard == null)
            return null;

        countdown--;

        if (countdown == 0)
        {
            Card card = countdownCard;
            countdownCard = null;
            return card;
        }
        else
            return null;
    }

    public bool IsCastingCard()
    {
        return countdownCard != null;
    }
}
