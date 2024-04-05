using System;

[Serializable] // Add this attribute to make the class serializable
public class CardEffect
{
    public CardEffectTarget effectTarget { get; private set; }
    public CardEffectType effectProperty { get; private set; }
    public float effectValue { get; private set; }
    public int distance { get; private set; }
    public FaceDirection direction { get; private set; }
    public float successPercentage { get; private set; }
    public bool isAreaEffect { get; private set; }
    public bool IsAreaTriggerAfterDIstance { get; private set; }

    public CardEffect() { }

    public CardEffect(CardEffectTarget _effectTarget, CardEffectType _effectProperty, float _effectValue, int _distance, FaceDirection _direction, int _successPercentage, bool _isAreaEffect, bool isAreaTriggerAfterDIstance)
    {
        effectTarget = _effectTarget;
        effectProperty = _effectProperty;

        effectValue = _effectValue;
        distance = _distance;
        direction = _direction;

        successPercentage = _successPercentage;
        isAreaEffect = _isAreaEffect;
        IsAreaTriggerAfterDIstance = isAreaTriggerAfterDIstance;
    }


}
