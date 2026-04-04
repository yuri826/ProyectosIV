using UnityEngine;

public enum PlayerState
{
    Locked,
    Move,
    Dash,
    Repair,
    GameOver
}

public enum DepositState
{
    Objects,
    Tool,
    Completed
}

public enum LevelFlowState
{
    Intro,
    Gameplay,
    Win,
    GameOver
}

public enum SpeedState
{
    Low,
    Middle,
    High
}

public enum MapNodeState
{
    Locked,
    Unlocked,
    Completed
}

public enum OutlawState
{
    PickSabotage,
    MoveToSabotage,
    PlantDynamite,
    MoveToSafePosition,
    WaitExplosion,
    Patrol,
    Combat,
    Laugh,
    Sandstorm
}

public enum SabotagePointState
{
    Free,
    Reserved,
    Broken
}

public enum PickableState
{
    Ground,
    Picked,
    Throw
}

public enum LevelEventType
{
    OutlawWave,
    Sandstorm
}

public enum ResourceType
{
    Log,
    Rope,
    Nail,
    Gunpowder,
    Metal,
    Coal,
    Planks,
    Lasso,
    Dynamite,
    Bullets
}