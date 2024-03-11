using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePlayer : MonoBehaviour, ITargetObject
{
    public BattlePlayerData playerData { get; private set; }
    public List<CharacterStatus> playerStatusList { get; private set; }
    public CharacterState playerState { get; private set; }
    public FaceDirection currentDirection { get; private set; }
    public Vector2 currentPos { get; private set; }

    private bool CanCancel;
    private int specialCountdown;
    private List<BattleMapTile> currentTiles;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(BattlePlayerData _playerData)
    {
        playerData = _playerData;

        //init
        playerStatusList = new List<CharacterStatus>();
        playerState = CharacterState.Idle;
        CanCancel = true;
    }



    public void BeAttacked(int value)
    {
        Debug.Log("BattlePlayer BeAttacked" + value);
        playerData.CurShipBodyHP += value;
        Debug.Log("playerData.HP" + playerData.CurShipBodyHP);
    }

    public void BeMoved(Vector2 pos, FaceDirection rotation)
    {
        Debug.Log("BattlePlayer BeMoved" + pos.x + pos.y + rotation);
        currentPos = pos;
        currentDirection = rotation;
        throw new System.NotImplementedException();
    }

    public void BeTarget()
    {
        throw new System.NotImplementedException();
    }

    public void BeDefenced(int value)
    {
        Debug.Log("BattlePlayer BeDefenced" + value);
        playerData.CurShipBodyHP += value;
        Debug.Log("playerData.HP" + playerData.CurShipBodyHP);
    }

    public void StartTurn()
    {
        Debug.Log($"StartTurn: HP {playerData.CurShipBodyHP}");

        //recovery
        //Gain HP and MP when Start
        //Gain MP when Start
        //if (playerData.CurShipBodyHP < playerData.MaxHP)
        //    playerData.HP += playerData.regHP;

        //if (playerData.MP < playerData.MaxMP)
        //    playerData.MP += playerData.regMP;
    }
}
