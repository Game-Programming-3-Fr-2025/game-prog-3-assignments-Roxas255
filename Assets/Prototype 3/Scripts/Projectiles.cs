using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float lifeSeconds = 4f;

    private Rigidbody2D rb;
    private Collider2D col;
    private Humans shooter;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;

        col = GetComponent<Collider2D>();

        // Destroy after some time 
        Invoke(nameof(Die), lifeSeconds);
    }

    public void Launch(Vector2 velocity, Humans shooter)
    {
        this.shooter = shooter;

        // Move projectile
        rb.linearVelocity = velocity;

        // Rotate projectile to face movement direction
        if (velocity.sqrMagnitude > 0.0001f)
        {
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        // Ignore collision with the shooter
        if (shooter != null)
        {
            var shooterCol = shooter.GetComponent<Collider2D>();
            if (shooterCol && col)
            {
                Physics2D.IgnoreCollision(col, shooterCol, true);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // If it hits the player then game over
        if (other.CompareTag("Player"))
        {
            Gamemanager.Instance.GameOver();
            Die();
            return;
        }

        // If it hits an infected human it kill them
        var human = other.GetComponent<Humans>();
        if (human != null && human.Current == Humans.State.Infected)
        {
            human.Die();
            Die();
            return;
        }

        // Hit anything solid it dies
        if (!other.isTrigger)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}