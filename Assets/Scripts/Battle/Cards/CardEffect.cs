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
    
    public CardEffect() { }

    public CardEffect(CardEffectTarget _effectTarget, CardEffectType _effectProperty, float _effectValue, int _distance, string _direction, int _successPercentage, bool _isAreaEffect)
    {
        effectTarget = _effectTarget;
        effectProperty = _effectProperty;

        //if (_effectTarget == "Enemy")
        //    effectTarget = EffectTarget.Enemy;
        //else if (_effectTarget == "Self")
        //    effectTarget = EffectTarget.Self;
        //else if (_effectTarget == "Ally")
        //    effectTarget = EffectTarget.Ally;
        //else if (_effectTarget == "All")
        //    effectTarget = EffectTarget.Any;
        //else if (_effectTarget == "Ground")
        //    effectTarget = EffectTarget.Ground;

        //if (_effectProperty == "HP")
        //    effectProperty = EffectProperty.HP;
        //else if (_effectProperty == "MP")
        //    effectProperty = EffectProperty.MP;
        //else if (_effectProperty == "Shield")
        //    effectProperty = EffectProperty.Shield;
        //else if (_effectProperty == "Position")
        //    effectProperty = EffectProperty.Position;
        //else if (_effectProperty == "Stun")
        //    effectProperty = EffectProperty.Stun;
        //else if (_effectProperty == "IncreaseAttack")
        //    effectProperty = EffectProperty.IncreaseAttack;
        //else if (_effectProperty == "Posion")
        //    effectProperty = EffectProperty.Posion;
        //else if (_effectProperty == "Blind")
        //    effectProperty = EffectProperty.Blind;
        //else if (_effectProperty == "Unmovement")
        //    effectProperty = EffectProperty.Unmovement;
        //else if (_effectProperty == "Dogde")
        //    effectProperty = EffectProperty.Dogde;
        //else if (_effectProperty == "Gain")
        //    effectProperty = EffectProperty.Gain;
        //else if (_effectProperty == "Push")
        //    effectProperty = EffectProperty.Push;
        //else if (_effectProperty == "Pull")
        //    effectProperty = EffectProperty.Pull;
        //else if (_effectProperty == "Gain")
        //    effectProperty = EffectProperty.Gain;
        //else if (_effectProperty == "Fire")
        //    effectProperty = EffectProperty.Fire;
        //else if (_effectProperty == "Weakness")
        //    effectProperty = EffectProperty.Weakness;
        //else if (_effectProperty == "RemoveStatus")
        //    effectProperty = EffectProperty.RemoveStatus;
        //else if (_effectProperty == "Trap")
        //    effectProperty = EffectProperty.Trap;
        //else if (_effectProperty == "Untargetable")
        //    effectProperty = EffectProperty.Untargetable;

        effectValue = _effectValue;
        distance = _distance;

        if (_direction == "All")
            direction = FaceDirection.All;
        else if (_direction == "Back")
            direction = FaceDirection.Back;
        else if (_direction == "Front")
            direction = FaceDirection.Front;
        else if (_direction == "Right")
            direction = FaceDirection.Right;
        else if (_direction == "Left")
            direction = FaceDirection.Left;
        else if (_direction == "No")
            direction = FaceDirection.No;

        successPercentage = _successPercentage;
        isAreaEffect = _isAreaEffect;
    }


}
