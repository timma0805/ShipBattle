using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePlayerData
{
    public int CurEnergyHP;
    public int MaxEnergyHP;
    public int CurShipBodyHP;
    public int MaxShipBodyHP;
    public int MaxMemberCount;
    public int MoveSpeed;
    public int CurFoodAmount;
    public int MaxFoodAmount;


    public BattlePlayerData()
    {
        CurEnergyHP = 100;
        MaxEnergyHP = 100;
        CurShipBodyHP = 100;
        MaxShipBodyHP = 100;
        MaxMemberCount = 3;
        MoveSpeed = 10;
        CurFoodAmount = 100;
        MaxFoodAmount = 100;
    }
}
