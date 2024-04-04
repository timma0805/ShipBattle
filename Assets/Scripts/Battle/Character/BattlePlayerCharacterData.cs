using System;
using System.Collections.Generic;

[Serializable] // Add this attribute to make the class serializable

public class BattlePlayerCharacterData: CharacterData
{
    public string SkillSetStr;
    public string CardSetStr;
    public string EquipmentSetStr;

    public List<CardData> CardDataList;

    public BattlePlayerCharacterData()
    {
     
    }
}
