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
        Invoke(nameof(Die), lifeSeconds);
    }

    public void Launch(Vector2 velocity, Humans shooter)
    {
        this.shooter = shooter;
        rb.linearVelocity = velocity;

        // Ignore collision with the shooter
        if (shooter != null)
        {
            var shooterCol = shooter.GetComponent<Collider2D>();
            if (shooterCol && col) Physics2D.IgnoreCollision(col, shooterCol, true);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // If it hits player then its  game over
        if (other.CompareTag("Player"))
        {
            Gamemanager.Instance.GameOver();
            Die();
            return;
        }

        // If it hits infected human, they die
        var human = other.GetComponent<Humans>();
        if (human != null && human.Current == Humans.State.Infected)
        {
            human.Die();
            Die();
            return;
        }

        // Hit walls/anything else then just delete
        if (!other.isTrigger)
        {
            Die();
        }
    }

    void Die() => Destroy(gameObject);
}

