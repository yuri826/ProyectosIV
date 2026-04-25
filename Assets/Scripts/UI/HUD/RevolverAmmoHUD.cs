using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RevolverAmmoHUD : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image chamberImage;
    [SerializeField] private Sprite[] chamberSprites;

    [Header("Rotation")]
    [SerializeField] private float chamberStepAngle = -60f;
    [SerializeField] private float reloadInsertRotateDuration = 0.12f;
    [SerializeField] private float reloadFinishSpinDuration = 0.5f;
    [SerializeField] private int reloadFinishSpinTurns = 2;

    private RectTransform chamberRectTransform;

    private Coroutine rotateRoutine;
    private Coroutine shotRoutine;
    private Coroutine reloadFinishRoutine;

    private float currentZRotation = 0f;

    private void Awake()
    {
        chamberRectTransform = chamberImage.rectTransform;
        currentZRotation = chamberRectTransform.localEulerAngles.z;
    }

    public void Initialize(int chamberAmmo)
    {
        SetAmmoInstant(chamberAmmo);
    }

    private void SetAmmoInstant(int chamberAmmo)
    {
        chamberAmmo = Mathf.Clamp(chamberAmmo, 0, chamberSprites.Length - 1);
        chamberImage.sprite = chamberSprites[chamberAmmo];
    }

    public void OnShot(int newChamberAmmo, float shootCooldown)
    {
        StopAllActiveAnimations();
        shotRoutine = StartCoroutine(ShotSequence(newChamberAmmo, shootCooldown));
    }

    public void OnReloadBulletInserted(int newChamberAmmo)
    {
        StopReloadFinishOnly();

        SetAmmoInstant(newChamberAmmo);

        if (rotateRoutine != null)
        {
            StopCoroutine(rotateRoutine);
        }

        rotateRoutine = StartCoroutine(RotateByAngle(chamberStepAngle, reloadInsertRotateDuration));
    }

    public void OnReloadComplete()
    {
        StopReloadFinishOnly();
        reloadFinishRoutine = StartCoroutine(ReloadCompleteSequence());
    }

    private IEnumerator ShotSequence(int newChamberAmmo, float shootCooldown)
    {
        SetAmmoInstant(newChamberAmmo);

        float waitBeforeRotate = shootCooldown * 0.5f;
        float rotateDuration = shootCooldown * 0.5f;

        yield return new WaitForSeconds(waitBeforeRotate);

        rotateRoutine = StartCoroutine(RotateByAngle(chamberStepAngle, rotateDuration));
        yield return rotateRoutine;

        shotRoutine = null;
    }

    private IEnumerator ReloadCompleteSequence()
    {
        if (rotateRoutine != null)
        {
            yield return rotateRoutine;
        }

        yield return StartCoroutine(SpinFullTurns(reloadFinishSpinTurns, reloadFinishSpinDuration));

        reloadFinishRoutine = null;
    }

    private IEnumerator RotateByAngle(float angleDelta, float duration)
    {
        float startAngle = currentZRotation;
        float endAngle = startAngle + angleDelta;

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(timer / duration);

            float currentAngle = Mathf.Lerp(startAngle, endAngle, t);
            SetChamberRotation(currentAngle);

            yield return null;
        }

        SetChamberRotation(endAngle);
        rotateRoutine = null;
    }

    private IEnumerator SpinFullTurns(int fullTurns, float duration)
    {
        float startAngle = currentZRotation;
        float endAngle = startAngle - (360f * fullTurns);

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(timer / duration);

            float currentAngle = Mathf.Lerp(startAngle, endAngle, t);
            SetChamberRotation(currentAngle);

            yield return null;
        }

        SetChamberRotation(endAngle);
    }

    private void SetChamberRotation(float zRotation)
    {
        currentZRotation = zRotation;
        chamberRectTransform.localRotation = Quaternion.Euler(0f, 0f, currentZRotation);
    }

    private void StopAllActiveAnimations()
    {
        if (shotRoutine != null)
        {
            StopCoroutine(shotRoutine);
            shotRoutine = null;
        }

        if (rotateRoutine != null)
        {
            StopCoroutine(rotateRoutine);
            rotateRoutine = null;
        }

        if (reloadFinishRoutine != null)
        {
            StopCoroutine(reloadFinishRoutine);
            reloadFinishRoutine = null;
        }
    }

    private void StopReloadFinishOnly()
    {
        if (reloadFinishRoutine != null)
        {
            StopCoroutine(reloadFinishRoutine);
            reloadFinishRoutine = null;
        }
    }
}
