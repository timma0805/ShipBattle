using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable] // Add this attribute to make the class serializable
public class CardData
{
    [SerializeField]
    private int _cardIndex;
    [SerializeField]
    private string _name;
    [SerializeField]
    private string _textureName;
    [SerializeField]
    private int _cost;
    [SerializeField]
    private float _costTime;
    [SerializeField]
    private CardType _cardType;
    [SerializeField]
    private Sprite _texture;
    [SerializeField]
    private List<CardCondition> _conditionList;
    [SerializeField]
    private List<CardEffect> _effectList;
    [SerializeField]
    private bool isExhaust = false;

    public int cardIndex { get => _cardIndex; }
    public string name { get => _name; }
    public string textureName { get => _textureName; }
    public Sprite texture { get => _texture; }
    public int cost { get => _cost; }
    public float costTime { get => _costTime; }
    public CardType cardType { get => _cardType; }
    public List<CardCondition> conditionList { get => _conditionList; }
    public List<CardEffect> effectList { get => _effectList; }

    public CardData() { }

    public CardData(int cardIndex, string name, string textureName, int cost, CardType cardType)
    {
        _cardIndex = cardIndex;
        _name = name;
        _textureName = textureName;
        _cost = cost;
        _cardType = cardType;
        _conditionList = new List<CardCondition>();
        _effectList = new List<CardEffect>();
    }
}
