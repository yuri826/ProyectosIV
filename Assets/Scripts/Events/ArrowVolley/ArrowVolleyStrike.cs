using System.Collections.Generic;
using UnityEngine;

public class ArrowVolleyStrike : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float travelSpeed = 18f;

    [Header("Lifetime")]
    [SerializeField] private float lifeAfterReachingEnd = 0.5f;

    private Vector3 moveDirection;
    private Vector3 endPosition;
    private float damage;

    private readonly List<PlayerHealthManager> damagedPlayers = new();

    public void Initialize(Vector3 startPosition, Vector3 targetPosition, ArrowVolleyDirection volleyDirection, float volleyDamage)
    {
        transform.position = startPosition;
        endPosition = targetPosition;
        damage = volleyDamage;
        damagedPlayers.Clear();

        moveDirection = (targetPosition - startPosition).normalized;
        RotateToDirection(volleyDirection);
    }

    private void Update()
    {
        transform.position += moveDirection * travelSpeed * Time.deltaTime;

        float distanceToEnd = Vector3.Distance(transform.position, endPosition);

        if (distanceToEnd <= lifeAfterReachingEnd)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerHealthManager playerHealth = other.GetComponent<PlayerHealthManager>();

        if (playerHealth == null)
        {
            return;
        }

        if (damagedPlayers.Contains(playerHealth))
        {
            return;
        }

        playerHealth.TakeDamage(damage);
        damagedPlayers.Add(playerHealth);
    }

    private void RotateToDirection(ArrowVolleyDirection volleyDirection)
    {
        switch (volleyDirection)
        {
            case ArrowVolleyDirection.TopToBottom:
                transform.rotation = Quaternion.Euler(0f, 0f, 180f);
                break;

            case ArrowVolleyDirection.BottomToTop:
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                break;

            case ArrowVolleyDirection.LeftToRight:
                transform.rotation = Quaternion.Euler(0f, 0f, -90f);
                break;

            case ArrowVolleyDirection.RightToLeft:
                transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                break;
        }
    }
}
