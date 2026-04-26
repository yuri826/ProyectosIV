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

    public void Initialize(Vector3 startPosition, Vector3 targetPosition, ArrowVolleyDirection volleyDirection, float volleyDamage)
    {
        transform.position = startPosition;
        endPosition = targetPosition;
        damage = volleyDamage;
        
        moveDirection = (targetPosition - startPosition).normalized;
        RotateToDirection(volleyDirection);
    }

    private void Update()
    {
        transform.position += moveDirection * (travelSpeed * Time.deltaTime);

        float distanceToEnd = Vector3.Distance(transform.position, endPosition);
        if (distanceToEnd <= lifeAfterReachingEnd) Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        //Mucho mejor un trygetcomponent
        // PlayerHealthManager playerHealth = other.GetComponent<PlayerHealthManager>();
        // if ((playerHealth is null) || (damagedPlayers.Contains(playerHealth))) return;
        
        if (other.TryGetComponent(out PlayerHealthManager playerHealth)) playerHealth.TakeDamage(damage);
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
