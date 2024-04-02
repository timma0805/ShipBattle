using System;

[Serializable] // Add this attribute to make the class serializable
public class CardData
{
    public int ID;
    public string Occupation;
    public string Name;
    public string Description;
    public CardType Type;
    public int Level;
    public int Cost;
    public bool isExhaust;
    public CardEffectType Effect;
    public CardEffectTarget Target;
    public float Value;
    public string Direction;
    public int Distance;
    public bool IsAreaEffect;
    public int Success;
    public CardType? Type2;
    public CardEffectType? Effect2;
    public CardEffectTarget? Target2;
    public float? Value2;
    public string? Direction2;
    public int? Distance2;
    public bool? IsAreaEffect2;
    public int? Success2;

}
