using UnityEngine;

public class FallingCamera : MonoBehaviour

{
    public float scrollSpeed = 6f; 

    void LateUpdate()
    {
        transform.position += Vector3.down * scrollSpeed * Time.deltaTime;
    }
}