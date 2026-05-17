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
    [Header("Event references")] [SerializeField]
    private StudioEventEmitter shoot;

    [SerializeField] private StudioEventEmitter pickUpObj;
    [SerializeField] private StudioEventEmitter throwObj;
    [SerializeField] private StudioEventEmitter putDownObj;
    [SerializeField] private StudioEventEmitter onTool;
    [SerializeField] private StudioEventEmitter onDeTool;
    [SerializeField] private StudioEventEmitter dash;
    [SerializeField] private StudioEventEmitter bulletPick;
    [SerializeField] private StudioEventEmitter reload;
    [SerializeField] private StudioEventEmitter hurt;
    [SerializeField] private StudioEventEmitter die;

    public void PlaySfx(PlayerSFX sfx)
    {
        switch (sfx)
        {
            case PlayerSFX.shoot:
                shoot?.Play();
                break;
            case PlayerSFX.pickUpObj:
                pickUpObj?.Play();
                break;
            case PlayerSFX.throwObj:
                throwObj?.Play();
                break;
            case PlayerSFX.putDownObj:
                putDownObj?.Play();
                break;
            case PlayerSFX.onTool:
                onTool?.Play();
                break;
            case PlayerSFX.onDeTool:
                onDeTool?.Play();
                break;
            case PlayerSFX.dash:
                dash?.Play();
                break;
            case PlayerSFX.bulletPick:
                bulletPick?.Play();
                break;
            case PlayerSFX.reload:
                reload?.Play();
                break;
            case PlayerSFX.hurt:
                hurt?.Play();
                break;
            case PlayerSFX.die:
                die?.Play();
                break;
        }
    }
}
