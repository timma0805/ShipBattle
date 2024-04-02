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

    public enum CharacterType
    {
        Captain,
        Crew,
        Enemy,
        MiniBoss, 
        FinalBoss
    }

    public enum VillageBuildingType
    {
        SupplyShop,
        Bar,
        Repair,
        OtherNPC
    }

    public enum ItemType
    {
        Equipment,
        Food,
        SpecialMaterial,
        Poison
    }

    public enum CardType
    {
        None,
        Attack,
        Defense,
        Heal,
        Move,
        Special
    }

public enum CardEffectTarget
{
    Any,
    Self,
    Enemy,
    Ally,
    Ground
}

public enum CardEffectType
{
    No,
    HP,
    MP,
    Move,
    Shield,
    Stun,
    IncreaseAttack,
    Posion,
    Blind,
    Unmovement,
    Dogde,
    Gain,
    Push,
    Pull,
    Fire,
    Weakness,
    RemoveStatus,
    Trap,
    Untargetable,
    Defense
}

public enum EnemyActionTarget
    {
        Nearest,
        Farest,
        Lowest,
        Hightest,
        Captain
    }

public enum ConditionCompare
{
    Less,
    More,
    Equal
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
        UnMovable,
        Untargetable,
        Blind
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
        Left,
        All,
        No
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

