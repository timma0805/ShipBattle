using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable] // Add this attribute to make the class serializable
public class CardData
{
    public int ID;
    public string Occupation;
    public string Name;
    public CardType Type;
    public int Level;
    public int Cost;
    public bool isExhaust;
    public string Effect;
    public string Target;
    public int Value;
    public string Direction;
    public int Distance;
    public bool IsAreaEffect;
    public int Success;
    public string? Effect2;
    public string? Target2;
    public int? Value2;
    public string? Direction2;
    public int? Distance2;
    public bool? IsAreaEffect2;
    public int? Success2;

}
