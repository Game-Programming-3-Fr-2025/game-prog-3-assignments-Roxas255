using UnityEngine;

public class Infectedplayer : MonoBehaviour
{
    public float moveSpeed = 8f;

    private Rigidbody2D rb;
    private Vector2 input;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
    }

    void Update()
    {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = input * moveSpeed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Infect healthy humans on touch
        var human = other.GetComponent<Humans>();
        if (human != null && human.Current == Humans.State.Healthy)
        {
            human.Infect();
            return;
        }

        
        if (other.CompareTag("Projectile"))
        {
            Gamemanager.Instance.GameOver();
        }
    }
}
