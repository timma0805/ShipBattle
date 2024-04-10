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
        CaptainChoosing,
        SupplyShop,
        Bar,
        Repair
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
    Attack,
    HP,
    MP,
    Move,   
    Gain,
    Push,
    Pull,
    Trap,
    Defense,
    RemoveStatus,
    Untargetable = 100, //need same int value as CharacterStatus
    Shield,
    Stun,
    IncreaseAttack,
    Posion,
    Blind,
    Unmovement,
    Dogde,
    Weakness,
    Bleed,
    Fire,
    Freeze
}

public enum EnemyActionTarget
    {
        Nearest,
        Farest,
        Lowest,
        Hightest,
        Captain,
        Self
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
        Choosing,
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
        Dead,
        Removed
    }

    public enum CharacterStatus
    {
        Untargetable = 100, //need same int value as CardEffectType
        Shield,
        Stun,
        IncreaseAttack,
        Posion,
        Blind,
        Unmovement,
        Dogde,
        Weakness,
        Bleed,
        Fire,
        Freeze
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
        Back,
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

