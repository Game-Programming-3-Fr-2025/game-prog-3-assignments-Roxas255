using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Humans : MonoBehaviour
{
    public enum State { Healthy, Infected, Dead }
    public State Current { get; private set; } = State.Healthy;

    [Header("Movement")]
    public float healthySpeed = 4f;
    public float infectedSpeed = 5.5f;

    [Header("Shooting (for guards)")]
    public bool canShoot = false;
    public Projectile projectilePrefab;
    public float shootCooldown = 1.2f;
    public float shootRange = 7f;
    public float projectileSpeed = 12f;

    private float nextShootTime = 0f;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
    }

    void FixedUpdate()
    {
        if (Current == State.Dead)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (Current == State.Healthy)
        {
            // Flee from nearest infected player or infected humans
            Transform threat = FindNearestInfected();
            Vector2 dir = Vector2.zero;

            if (threat != null)
            {
                dir = ((Vector2)(transform.position - threat.position)).normalized;
            }

            rb.linearVelocity = dir * healthySpeed;

            // shoot at infected if guard is within range
            if (canShoot && Time.time >= nextShootTime && threat != null)
            {
                float dist = Vector2.Distance(transform.position, threat.position);
                if (dist <= shootRange)
                {
                    ShootAt(threat.position);
                    nextShootTime = Time.time + shootCooldown;
                }
            }
        }
        else if (Current == State.Infected)
        {
            // Chase nearest healthy human
            Transform target = FindNearestHealthy();
            Vector2 dir = Vector2.zero;

            if (target != null)
            {
                dir = ((Vector2)(target.position - transform.position)).normalized;
            }

            rb.linearVelocity = dir * infectedSpeed;
        }
    }

    public void Infect()
    {
        if (Current != State.Healthy) return;
        Current = State.Infected;
        // Change color to  red to show zombie
        var sr = GetComponent<SpriteRenderer>();
        if (sr) sr.color = new Color(0.9f, 0.2f, 0.2f);
    }

    public void Die()
    {
        if (Current == State.Dead) return;
        Current = State.Dead;
        rb.linearVelocity = Vector2.zero;
        // Change color to grey when zombie is hit by human
        var sr = GetComponent<SpriteRenderer>();
        if (sr) sr.color = Color.gray;
        var col = GetComponent<Collider2D>();
        if (col) col.enabled = false;

        Destroy(gameObject, 2f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (Current == State.Dead) return;

        // Infection spreads from player or infected humans
        var otherHuman = other.GetComponent<Humans>();
        if (Current == State.Healthy && otherHuman != null && otherHuman.Current == State.Infected)
            Infect();

        // Projectiles kill infected humans only
        if (Current == State.Infected && other.CompareTag("Projectile"))
            Die();
    }

    void ShootAt(Vector2 worldPos)
    {
        if (projectilePrefab == null) return;
        var p = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Vector2 dir = (worldPos - (Vector2)transform.position).normalized;
        p.Launch(dir * projectileSpeed, shooter: this);
    }

    Transform FindNearestHealthy()
    {
        Humans[] all = FindObjectsByType<Humans>(FindObjectsSortMode.None);
        Transform best = null;
        float bestDist = float.MaxValue;
        foreach (var h in all)
        {
            if (h == this) continue;
            if (h.Current != State.Healthy) continue;
            float d = (h.transform.position - transform.position).sqrMagnitude;
            if (d < bestDist) { bestDist = d; best = h.transform; }
        }
        return best;
    }

    Transform FindNearestInfected()
    {
        // Player counts as infected source
        var player = GameObject.FindGameObjectWithTag("Player");
        Transform best = player != null ? player.transform : null;
        float bestDist = best != null ? (best.position - transform.position).sqrMagnitude : float.MaxValue;

        Humans[] all = FindObjectsByType<Humans>(FindObjectsSortMode.None);
        foreach (var h in all)
        {
            if (h == this) continue;
            if (h.Current != State.Infected) continue;
            float d = (h.transform.position - transform.position).sqrMagnitude;
            if (d < bestDist) { bestDist = d; best = h.transform; }
        }
        return best;
    }
}

