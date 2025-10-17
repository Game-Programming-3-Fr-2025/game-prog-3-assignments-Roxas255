using UnityEngine;

public class Camerabox : MonoBehaviour
{
    public Transform target;          
    public Vector2 offset = Vector2.zero;
    public float smoothTime = 0.15f; 
    public bool lockY = false;        

    private Vector3 velocity = Vector3.zero;
    // setting camera to follow player
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
            transform.position.z 
        );

        transform.position = Vector3.SmoothDamp(transform.position, desired, ref velocity, smoothTime);
    }
}
