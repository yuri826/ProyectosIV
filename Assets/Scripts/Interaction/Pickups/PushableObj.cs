using System.Collections;
using UnityEngine;

public class PushableObj : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private Rigidbody rb;
    
    [Header("MoveParts")]
    [SerializeField] private ParticleSystem dragParts;

    private PlayerMovement currentPlayer;

    public bool CanStartMoving()
    {
        return currentPlayer == null;
    }

    public void StartMoving(PlayerMovement player)
    {
        dragParts.Play();
        currentPlayer = player;
    }

    public void StopMoving(PlayerMovement player)
    {
        if (currentPlayer != player) return;

        dragParts.Stop();
        currentPlayer = null;
    }

    public void Move(Vector3 moveDirection)
    {
        Vector3 displacement = moveDirection * (moveSpeed * Time.deltaTime);
        displacement.y = 0f;
        Vector3 targetPosition = rb.position + displacement;

        rb.MovePosition(targetPosition);
    }

    private void OnDestroy()
    {
        currentPlayer.DropPushable();
    }
}
