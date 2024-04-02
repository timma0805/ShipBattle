using System;

[Serializable] // Add this attribute to make the class serializable

public class BattlePlayerCharacterData
{
    public int ID;
    public string Name;
    public CharacterType Type;
    public int HP;
    public int Speed;
    public int Attack;
    public string SkillSetStr;
    public string CardSetStr;
    public string EquipmentSetStr;

    public int CurHP;


    public BattlePlayerCharacterData()
    {
     
    }
}
