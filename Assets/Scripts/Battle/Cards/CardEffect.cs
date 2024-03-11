using System;

[Serializable] // Add this attribute to make the class serializable
public class CardEffect
{
    public EffectTarget effectTarget;
    public EffectProperty effectProperty;
    public string effectValueStr;
    public int rangeX;
    public int rangeY;
    public bool haveDirection = false;
    public FaceDirection direction;
    public float successPercentage;

    public enum EffectTarget
    {
        Player,
        Enemy
    }

    public enum EffectProperty
    {
        HP,
        MP,
        Position
    }
    
    public CardEffect() { }

    public CardEffect(EffectTarget _effectTarget, EffectProperty _effectProperty, string _effectValueStr)
    {
        effectTarget = _effectTarget;
        effectProperty = _effectProperty;

        effectValueStr = _effectValueStr;
    }


}
