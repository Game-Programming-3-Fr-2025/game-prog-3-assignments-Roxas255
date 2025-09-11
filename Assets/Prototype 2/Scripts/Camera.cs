using UnityEngine;

public class Camera : MonoBehaviour
{
    public Transform target;
    public float smooth = 5f;
    public float yOffset = 2f;

    void LateUpdate()
    {
        if (!target) return;
        Vector3 pos = transform.position;
        float targetY = target.position.y + yOffset;
        pos.y = Mathf.Lerp(pos.y, targetY, Time.deltaTime * smooth);
        transform.position = pos;
    }
}