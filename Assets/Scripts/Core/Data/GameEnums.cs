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
    Sandstorm,
    RenegadeWave,
    Collapse
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

public enum CartState
{
    MoveToCart,
    InCart,
    Shoot,
    Dead,
    Return
}

public enum CartShootState
{
    LookLeftToSide,
    LookRightToCenter,
    LookRightToSide,
    LookLeftToCenter,
}

public enum RenegadeTargetType
{
    TrainConnection,
    SafeBox
}

public enum RenegadeTargetState
{
    Free,
    Reserved,
    Broken
}

public enum ArrowVolleyDirection
{
    TopToBottom,
    BottomToTop,
    LeftToRight,
    RightToLeft
}