using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RevolverAmmoHUD : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image chamberImage;
    [SerializeField] private Sprite[] chamberSprites; // 0 a 6 balas

    [Header("Rotation")]
    [SerializeField] private float chamberStepAngle = -60f;
    [SerializeField] private float rotateDuration = 0.12f;
    [SerializeField] private float reloadSpinDuration = 0.5f;
    [SerializeField] private int reloadSpinTurns = 2;

    private RectTransform chamberRectTransform;
    private Coroutine rotateRoutine;
    private Coroutine reloadSpinRoutine;

    private void Awake()
    {
        chamberRectTransform = chamberImage.rectTransform;
    }

    public void Initialize(int chamberAmmo)
    {
        SetAmmoInstant(chamberAmmo);
    }

    public void SetAmmoInstant(int chamberAmmo)
    {
        chamberAmmo = Mathf.Clamp(chamberAmmo, 0, chamberSprites.Length - 1);
        chamberImage.sprite = chamberSprites[chamberAmmo];
    }

    public void OnShot(int newChamberAmmo)
    {
        StopActiveAnimations();

        SetAmmoInstant(newChamberAmmo);
        rotateRoutine = StartCoroutine(RotateChamberByAngle(chamberStepAngle, rotateDuration));
    }

    public void OnReloadBulletInserted(int newChamberAmmo)
    {
        StopReloadSpinOnly();

        SetAmmoInstant(newChamberAmmo);
        rotateRoutine = StartCoroutine(RotateChamberByAngle(chamberStepAngle, rotateDuration));
    }

    public void OnReloadComplete()
    {
        StopReloadSpinOnly();
        reloadSpinRoutine = StartCoroutine(PlayReloadFinishSpin());
    }

    private IEnumerator RotateChamberByAngle(float angleDelta, float duration)
    {
        Quaternion startRotation = chamberRectTransform.localRotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(0f, 0f, angleDelta);

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(timer / duration);

            chamberRectTransform.localRotation = Quaternion.Lerp(startRotation, endRotation, t);
            yield return null;
        }

        chamberRectTransform.localRotation = endRotation;
        rotateRoutine = null;
    }

    private IEnumerator PlayReloadFinishSpin()
    {
        Quaternion startRotation = chamberRectTransform.localRotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(0f, 0f, -360f * reloadSpinTurns);

        float timer = 0f;

        while (timer < reloadSpinDuration)
        {
            timer += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(timer / reloadSpinDuration);

            chamberRectTransform.localRotation = Quaternion.Lerp(startRotation, endRotation, t);
            yield return null;
        }

        chamberRectTransform.localRotation = endRotation;
        reloadSpinRoutine = null;
    }

    private void StopActiveAnimations()
    {
        if (rotateRoutine != null)
        {
            StopCoroutine(rotateRoutine);
            rotateRoutine = null;
        }

        if (reloadSpinRoutine != null)
        {
            StopCoroutine(reloadSpinRoutine);
            reloadSpinRoutine = null;
        }
    }

    private void StopReloadSpinOnly()
    {
        if (reloadSpinRoutine != null)
        {
            StopCoroutine(reloadSpinRoutine);
            reloadSpinRoutine = null;
        }
    }
}
