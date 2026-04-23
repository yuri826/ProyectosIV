using System.Collections;
using UnityEngine;

public class CollapseRockWarning : MonoBehaviour
{
    [SerializeField] private Transform visualRoot;
    [SerializeField] private Vector3 startScale = new Vector3(0.4f, 1f, 0.4f);
    [SerializeField] private Vector3 endScale = new Vector3(1.2f, 1f, 1.2f);

    public void StartWarning(float duration)
    {
        StartCoroutine(WarningRoutine(duration));
    }

    private IEnumerator WarningRoutine(float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / duration);
            visualRoot.localScale = Vector3.Lerp(startScale, endScale, t);

            yield return null;
        }

        Destroy(gameObject);
    }
}
