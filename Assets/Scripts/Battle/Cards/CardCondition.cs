using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable] // Add this attribute to make the class serializable
public class CardCondition
{
    public EffectTarget effectTarget;
    public ConditionValue conditionValue;
    public ConditionComapre conditionComapre;
    public ConditionCompareValueType conditionCompareValueType;
    public string conditionCompareValueStr;

    public enum EffectTarget
    {
        Self,
        Enemy
    }

    public enum ConditionValue
    {
        HP,
        MP,
        Position,
        CharacterStatus,
        FaceDirection
    }

    public enum ConditionComapre
    {
        Equal,
        Less,
        More
    }

    public enum ConditionCompareValueType
    {
        Integer,
        Boolean,
        String,
        Other
    }

    public CardCondition() { }

    public CardCondition (EffectTarget _effectTarget, ConditionValue _conditionValue, ConditionComapre _conditionComapre, ConditionCompareValueType _conditionCompareValueType, string _conditionCompareValueStr)
    {
        effectTarget = _effectTarget;
        conditionValue = _conditionValue;
        conditionComapre = _conditionComapre;
        conditionCompareValueType = _conditionCompareValueType;
        conditionCompareValueStr = _conditionCompareValueStr;
}


}
