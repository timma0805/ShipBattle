using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlayerData 
{
    public int SaveIndex;
    public string NickName;
    public int Score;
    public List<int> UnlockMapIndex;
    public List<int> UnlockCardIndex;
    public List<int> UnlockMemberIndex;
    public List<int> UnlockShipSkinIndex;
    public List<int> UnlockShipEquipmentIndex;

    public BasePlayerData() {
        SaveIndex = 0;
        NickName = "Test0";
        Score = 0;
        UnlockMapIndex = new List<int>();
        UnlockCardIndex = new List<int>();
        UnlockMemberIndex = new List<int>();
        UnlockShipSkinIndex = new List<int>();
        UnlockShipEquipmentIndex = new List<int>();
    }
}
