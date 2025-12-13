using UnityEngine;

public class CameraPlayer : MonoBehaviour
{
    public Transform target;          // The player
    public float smoothTime = 0.15f;  // Smooth follow (lower = snappier)
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(
            transform.position, desiredPosition,
            ref velocity, smoothTime
        );
    }
}