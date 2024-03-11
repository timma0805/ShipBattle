using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public enum MapEventType
    {
        NA,
        Start,
        Village,
        SpecialEvent,
        Enemy,
        MiniBoss,
        FinalBoss
    }

    public enum SpecialEventType
    {
        NA,
    //NPC
        Priest,
        Blacksmith,
        Tailor,
        Scholar,
        Chronicler,
        Musician,
        Merchant,
      // Accident
        Wave,
        Fog,
        Rainny,
        SinkShip,
        Fishing,
        Helping,
        Monster
    }

    public enum VillageBuildingType
    {
        SupplyShop,
        Bar,
        Repair,
        OtherNPC
    }

    public enum ShipPositionType
    {
        Rudder,
        Attack,
        EnergyRoom
    }

    public enum ItemType
    {
        Equipment,
        Food,
        SpecialMaterial,
        Poison
    }

    public enum MemberJob
    {
        None,
        Captain,
        Cook,
        Sailor,
        Hunter,
        Navigator,
        Knight,
        Doctor,
        Shipwright,
        Pet
    }

    public enum CardType
    {
        None,
        AttackMelee,
        AttackLongRange,
        Defend,
        MovePosition,
        MoveShip,
        Other
    }

    public enum EnemyAttackTarget
    {
        Nearby,
        People,
        EnergyRoom
    }

public enum BattleStage
{
    InitRun,
    StartRun,
    StartEvent,
    EndEvent,
    StartBattle,
    EndBattle,
    StartVillage,
    EndVillage,
    EndRun

}

public enum CharacterState
{
    Idle,
    Prepare,
    Attack,
    AfterAttack,
    Dead
}

public enum CharacterStatus
{
    Sheid,
    Freeze,
    Posion,
    Bleed,
    Countdown,
    UnMovable
}

public enum Element
{
    Fire,
    Water,
    Ice,
    Sharp,
    Light,
    Dark
}

public enum FaceDirection
{
    Front,
    Right,
    Back,
    Left
}

public enum MapTitleEffect
{
    Slow,
    Freeze,
    Burn,
    Posion
}

public enum MapDifficulty
{
    Easy,
    Normal,
    Hard,
    Expert
}

