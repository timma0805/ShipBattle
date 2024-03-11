using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private BasePlayerData basePlayerData;
    private BattlePlayerData battlePlayerData;

    private void Awake()
    {
        //init
        basePlayerData = new BasePlayerData();
        battlePlayerData = new BattlePlayerData();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

  
    public BasePlayerData GetBasePlayerData()
    {
        return basePlayerData;
    }

    public BattlePlayerData GetBattlePlayerData()
    {
        return battlePlayerData;
    }
}
