using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable] // Add this attribute to make the class serializable
public class CardEffect
{
    public CardEffectType Type;
    public float Value;
    public FaceDirection Direction;
    public List<Vector2> PosList;

    public CardEffect( CardEffectType effectType, float effectValue, FaceDirection direction, List<Vector2> vectors)
    {
        Type = effectType;
        Value = effectValue;
        Direction = direction;
        PosList = new List<Vector2>(vectors); ;
    }
}
