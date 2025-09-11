using UnityEngine;

public class Falling : MonoBehaviour
{
    public float fall = 6f;   
    public float move = 8f;   
    public float cam = 4.5f;    

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f; 
    }

    void Update()
    {
        // can use either right/left arrow keys or A/D to move left or right
        float x = Input.GetAxisRaw("Horizontal");
        Vector2 v = new Vector2(x * move, -fall);

        
        rb.linearVelocity = v;
        

        // Keeping camera on player
        Vector3 p = transform.position;
        p.x = Mathf.Clamp(p.x, -cam, cam);
        transform.position = p;
    }
}