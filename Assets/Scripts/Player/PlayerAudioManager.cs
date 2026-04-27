using System;
using FMODUnity;
using UnityEngine;

public enum PlayerSFX
{
    shoot,
    pickUpObj,
    throwObj,
    putDownObj,
    onTool,
    onDeTool,
    dash,
    bulletPick,
    reload,
    hurt,
    die
}

public class PlayerAudioManager : MonoBehaviour
{
    [Header("Event references")]
    [SerializeField] private EventReference shoot;
    [SerializeField] private EventReference pickUpObj;
    [SerializeField] private EventReference throwObj;
    [SerializeField] private EventReference putDownObj;
    [SerializeField] private EventReference onTool;
    [SerializeField] private EventReference onDeTool;
    [SerializeField] private EventReference dash;
    [SerializeField] private EventReference bulletPick;
    [SerializeField] private EventReference reload;
    [SerializeField] private EventReference hurt;
    [SerializeField] private EventReference die;
    
    [SerializeField] private StudioEventEmitter eventEmitter;

    public void PlaySfx(PlayerSFX sfx)
    {
        switch (sfx)
        {
            case PlayerSFX.shoot: EmmiterPlay(shoot);
                break;
            case PlayerSFX.pickUpObj: EmmiterPlay(pickUpObj);
                break;
            case PlayerSFX.throwObj: EmmiterPlay(throwObj);
                break;
            case PlayerSFX.putDownObj: EmmiterPlay(putDownObj);
                break;
            case PlayerSFX.onTool: EmmiterPlay(onTool);
                break;
            case PlayerSFX.onDeTool: EmmiterPlay(onDeTool);
                break;
            case PlayerSFX.dash: EmmiterPlay(dash);
                break;
            case PlayerSFX.bulletPick: EmmiterPlay(bulletPick);
                break;
            case PlayerSFX.reload: EmmiterPlay(reload);
                break;
            case PlayerSFX.hurt: EmmiterPlay(hurt);
                break;
            case PlayerSFX.die: EmmiterPlay(die);
                break;
        }
    }

    private void EmmiterPlay(EventReference sfx)
    {
        eventEmitter.EventReference = sfx;
        eventEmitter.Play();
    }
}
