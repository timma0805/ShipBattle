using System;

[Serializable]

public class ItemData
{
    public int ID;
    public string Name;
    public string Description;
    public int Price;
    public int Rare;
    public ItemType Type;
    public ItemEffectTime EffectTime;
    public ItemEffectTarget EffectTarget;
    public ItemEffect Effect;
    public int Value;
    public int UseTime;

    public ItemData()
    {

    }
}
