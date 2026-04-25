using System.Collections;
using UnityEngine;

public class ArrowVolleySystem : MonoBehaviour
{
    public static ArrowVolleySystem Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Camera levelCamera;
    [SerializeField] private ArrowVolleyWarningHUD warningHUD;
    [SerializeField] private ArrowVolleyStrike arrowVolleyStrikePrefab;

    [Header("Spawn Distance")]
    [SerializeField] private float outsideCameraOffset = 5f;

    [Header("Strike")]
    [SerializeField] private float defaultTravelSpeed = 18f;
    [SerializeField] private float warningHideDelayAfterStrikeStarts = 0.35f;

    private void Awake()
    {
        Instance = this;
    }

    public void StartArrowVolley(
        int volleyCount,
        float timeBetweenVolleys,
        float warningTime,
        float damage,
        bool useRandomDirections,
        ArrowVolleyDirection[] fixedDirections)
    {
        StartCoroutine(ArrowVolleyRoutine(
            volleyCount,
            timeBetweenVolleys,
            warningTime,
            damage,
            useRandomDirections,
            fixedDirections
        ));
    }

    private IEnumerator ArrowVolleyRoutine(
        int volleyCount,
        float timeBetweenVolleys,
        float warningTime,
        float damage,
        bool useRandomDirections,
        ArrowVolleyDirection[] fixedDirections)
    {
        for (int i = 0; i < volleyCount; i++)
        {
            ArrowVolleyDirection direction = GetVolleyDirection(i, useRandomDirections, fixedDirections);

            warningHUD.ShowWarning(direction);

            yield return new WaitForSeconds(warningTime);

            SpawnStrike(direction, damage);

            yield return new WaitForSeconds(warningHideDelayAfterStrikeStarts);
            warningHUD.HideWarning(direction);

            yield return new WaitForSeconds(timeBetweenVolleys);
        }
    }

    private ArrowVolleyDirection GetVolleyDirection(int volleyIndex, bool useRandomDirections, ArrowVolleyDirection[] fixedDirections)
    {
        if (useRandomDirections)
        {
            int randomIndex = Random.Range(0, 4);
            return (ArrowVolleyDirection)randomIndex;
        }

        int directionIndex = volleyIndex % fixedDirections.Length;
        return fixedDirections[directionIndex];
    }

    private void SpawnStrike(ArrowVolleyDirection direction, float damage)
    {
        Vector3 startPosition = GetStartPosition(direction);
        Vector3 endPosition = GetEndPosition(direction);

        ArrowVolleyStrike strike = Instantiate(arrowVolleyStrikePrefab);
        strike.Initialize(startPosition, endPosition, direction, damage);
    }

    private Vector3 GetStartPosition(ArrowVolleyDirection direction)
    {
        Vector3 viewportPosition = GetStartViewportPosition(direction);
        Vector3 worldPosition = levelCamera.ViewportToWorldPoint(viewportPosition);

        return worldPosition;
    }

    private Vector3 GetEndPosition(ArrowVolleyDirection direction)
    {
        Vector3 viewportPosition = GetEndViewportPosition(direction);
        Vector3 worldPosition = levelCamera.ViewportToWorldPoint(viewportPosition);

        return worldPosition;
    }

    private Vector3 GetStartViewportPosition(ArrowVolleyDirection direction)
    {
        switch (direction)
        {
            case ArrowVolleyDirection.TopToBottom:
                return new Vector3(0.5f, 1f + outsideCameraOffset, GetCameraDistance());

            case ArrowVolleyDirection.BottomToTop:
                return new Vector3(0.5f, -outsideCameraOffset, GetCameraDistance());

            case ArrowVolleyDirection.LeftToRight:
                return new Vector3(-outsideCameraOffset, 0.5f, GetCameraDistance());

            case ArrowVolleyDirection.RightToLeft:
                return new Vector3(1f + outsideCameraOffset, 0.5f, GetCameraDistance());

            default:
                return new Vector3(0.5f, 1f + outsideCameraOffset, GetCameraDistance());
        }
    }

    private Vector3 GetEndViewportPosition(ArrowVolleyDirection direction)
    {
        switch (direction)
        {
            case ArrowVolleyDirection.TopToBottom:
                return new Vector3(0.5f, -outsideCameraOffset, GetCameraDistance());

            case ArrowVolleyDirection.BottomToTop:
                return new Vector3(0.5f, 1f + outsideCameraOffset, GetCameraDistance());

            case ArrowVolleyDirection.LeftToRight:
                return new Vector3(1f + outsideCameraOffset, 0.5f, GetCameraDistance());

            case ArrowVolleyDirection.RightToLeft:
                return new Vector3(-outsideCameraOffset, 0.5f, GetCameraDistance());

            default:
                return new Vector3(0.5f, -outsideCameraOffset, GetCameraDistance());
        }
    }

    private float GetCameraDistance()
    {
        return Mathf.Abs(levelCamera.transform.position.y);
    }
}
