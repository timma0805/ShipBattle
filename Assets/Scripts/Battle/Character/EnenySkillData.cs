using System;
using static CardCondition;

[Serializable]
public class EnemySkillData 
{
    public int ID;
    public string Name;
    public CardType Type;
    public string Description;
    public CardEffectType Effect;
    public EnemyActionTarget Target;
    public float Value;
    public int Countdown;
    public FaceDirection Direction;
    public int Distance;
    public bool IsAreaEffect;
    public CardEffectTarget? ConditionTarget;
    public CardEffectType? ConditionProperty;
    public ConditionCompare? ConditionCompare;
    public float? ConditionValue;
}
