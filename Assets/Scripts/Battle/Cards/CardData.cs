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
    public FaceDirection Direction;
    public int Distance;
    public bool IsAreaEffect;
    public int Success;
    public CardType? Type2;
    public CardEffectType? Effect2;
    public CardEffectTarget? Target2;
    public float? Value2;
    public FaceDirection? Direction2;
    public int? Distance2;
    public bool? IsAreaEffect2;
    public int? Success2;
    public bool Temp = false;
    public CardData() { }
    public CardData(string _Occupation) //For base move
    {
        ID = 1001;
        Occupation = _Occupation;
        Name = "Base move";
        Description = "Move 1";
        Type = CardType.Move;
        Level = 0;
        Cost = 0;
        isExhaust = false;
        Effect = CardEffectType.Posion;
        Target = CardEffectTarget.Self;
        Value = 1;
        Direction = FaceDirection.All;
        Distance = 1;
        IsAreaEffect = false;
        Success = 100;
        Temp = true;
    }

}
