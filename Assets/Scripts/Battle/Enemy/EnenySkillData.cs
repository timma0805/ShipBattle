using System;
using static CardCondition;

[Serializable]
public class EnemySkillData 
{
    public int ID;
    public string Name;
    public CardType Type;
    public string Description;
    public string Effect;
    public string Target;
    public float Value;
    public int Cooldown;
    public FaceDirection Direction;
    public int Distance;
    public bool IsAreaEffect;
    public EffectTarget? ConditionTarget;
    public CardEffectType? ConditionProperty;
    public ConditionCompare? ConditionCompare;
    public float? ConditionValue;
}
