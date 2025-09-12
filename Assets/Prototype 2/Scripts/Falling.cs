using UnityEngine;

public class Falling : MonoBehaviour
{
    public float fallSpeed = 6f;   
    public float moveX = 8f;       
    public float moveY = 6f;       
    public float screenMargin = 0.4f;

    Rigidbody2D rb;
    Camera cam;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.simulated = true;
        rb.gravityScale = 0f;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        cam = Camera.main; 
    }

    void FixedUpdate()
    {
        // Controls using arrow keys or AWSD keys
        float ix = Input.GetAxisRaw("Horizontal"); 
        float iy = Input.GetAxisRaw("Vertical");   

    
        Vector2 vel = new Vector2(ix * moveX, -fallSpeed + iy * moveY);

        
        Vector2 next = rb.position + vel * Time.fixedDeltaTime;

        // Keep player inside camera view
        if (cam)
        {
            float halfH = cam.orthographicSize;
            float halfW = halfH * cam.aspect;

            next.x = Mathf.Clamp(next.x,
                cam.transform.position.x - halfW + screenMargin,
                cam.transform.position.x + halfW - screenMargin);
            next.y = Mathf.Clamp(next.y,
                cam.transform.position.y - halfH + screenMargin,
                cam.transform.position.y + halfH - screenMargin);
        }

        rb.MovePosition(next);
    }
}