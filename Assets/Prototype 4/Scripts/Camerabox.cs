using UnityEngine;

public class Camerabox : MonoBehaviour
{
    public Transform target;          // drag Player here (or it will auto-find by tag)
    public Vector2 offset = Vector2.zero;
    public float smoothTime = 0.15f;  // 0 = snap, higher = smoother
    public bool lockY = false;        

    private Vector3 velocity = Vector3.zero;

    void Awake()
    {
        if (!target)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) target = p.transform;
        }
    }

    void LateUpdate()
    {
        if (!target) return;

        Vector3 desired = new Vector3(
            target.position.x + offset.x,
            (lockY ? transform.position.y : target.position.y + offset.y),
            transform.position.z // keep camera's Z
        );

        transform.position = Vector3.SmoothDamp(transform.position, desired, ref velocity, smoothTime);
    }
}
