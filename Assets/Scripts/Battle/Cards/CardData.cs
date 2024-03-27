using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable] // Add this attribute to make the class serializable
public class CardData
{
    public int ID { get; private set; }
    public string Occupation { get; private set; }
    public string Name { get; private set; }
    public CardType Type { get; private set; }
    public int Cost { get; private set; }
    public bool isExhaust { get; private set; }
    public string Effect { get; private set; }
    public string Target { get; private set; }
    public int Value { get; private set; }
    public string Direction { get; private set; }
    public int Distance { get; private set; }
    public bool IsAreaEffect { get; private set; }
    public int Success { get; private set; }
    public string Effect2 { get; private set; }
    public string Target2 { get; private set; }
    public int Value2 { get; private set; }
    public string Direction2 { get; private set; }
    public int Distance2 { get; private set; }
    public bool IsAreaEffect2 { get; private set; }
    public int Success2 { get; private set; }

}
