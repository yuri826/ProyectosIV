using UnityEngine;

public class PushableObj : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private Rigidbody rb;

    private PlayerMovement currentPlayer;
    private bool isBeingMoved = false;

    public bool CanStartMoving(PlayerMovement player)
    {
        return currentPlayer == null || currentPlayer == player;
    }

    public void StartMoving(PlayerMovement player)
    {
        currentPlayer = player;
        isBeingMoved = true;
    }

    public void StopMoving(PlayerMovement player)
    {
        if (currentPlayer != player)
        {
            return;
        }

        currentPlayer = null;
        isBeingMoved = false;
    }

    public Vector3 Move(Vector3 moveDirection)
    {
        if (!isBeingMoved)
        {
            return Vector3.zero;
        }

        Vector3 displacement = moveDirection * (moveSpeed * Time.deltaTime);
        Vector3 targetPosition = rb.position + displacement;

        rb.MovePosition(targetPosition);

        displacement.y = 0f;
        return displacement;
    }

    public bool IsBeingMoved()
    {
        return isBeingMoved;
    }
}
