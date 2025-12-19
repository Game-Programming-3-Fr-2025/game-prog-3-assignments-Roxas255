using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Humans : MonoBehaviour
{
    public enum State { Healthy, Infected, Dead }
    public State Current { get; private set; } = State.Healthy;

    [Header("Movement")]
    public float healthySpeed = 4f;
    public float infectedSpeed = 5.5f;

    [Header("Healthy Behavior")]
    public float fleeStartRange = 6f;
    public float fleeStopRange = 7f;

    [Header("Shooting (guards only)")]
    public bool canShoot = false;
    public Projectile projectilePrefab;
    public float shootCooldown = 1.2f;
    public float shootRange = 7f;
    public float projectileSpeed = 12f;

    [Header("Sprites")]
    public Sprite healthySprite;
    public Sprite infectedSprite;
    public Sprite deadSprite;

    private float nextShootTime;
    private bool isFleeing = false;

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;

        sr = GetComponent<SpriteRenderer>();
        if (sr && healthySprite)
            sr.sprite = healthySprite;
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
            HandleHealthy();
        }
        else if (Current == State.Infected)
        {
            HandleInfected();
        }
    }

    void HandleHealthy()
    {
        Transform threat = FindNearestInfected();

        if (!threat)
        {
            isFleeing = false;
            rb.linearVelocity = Vector2.zero;
            return;
        }
                            //When humans flee at certain range
        float dist = Vector2.Distance(transform.position, threat.position);

        if (!isFleeing && dist <= fleeStartRange)
            isFleeing = true;

        if (isFleeing && dist >= fleeStopRange)
            isFleeing = false;

        if (isFleeing)
        {
            Vector2 dir = ((Vector2)(transform.position - threat.position)).normalized;
            rb.linearVelocity = dir * healthySpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (canShoot && Time.time >= nextShootTime && dist <= shootRange)
        {
            ShootAt(threat.position);
            nextShootTime = Time.time + shootCooldown;
        }
    }
                    //Infected tracks human
    void HandleInfected()
    {
        Transform target = FindNearestHealthy();
        Vector2 dir = Vector2.zero;

        if (target)
            dir = ((Vector2)(target.position - transform.position)).normalized;

        rb.linearVelocity = dir * infectedSpeed;
    }

    public void Infect()
    {
        if (Current != State.Healthy) return;

        Current = State.Infected;

        if (sr && infectedSprite)
            sr.sprite = infectedSprite;
    }

    public void Die()
    {
        if (Current == State.Dead) return;

        Current = State.Dead;
        rb.linearVelocity = Vector2.zero;

        if (sr && deadSprite)
            sr.sprite = deadSprite;

        Collider2D col = GetComponent<Collider2D>();
        if (col) col.enabled = false;

        Destroy(gameObject, 2f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (Current == State.Dead) return;

        Humans otherHuman = other.GetComponent<Humans>();
        if (Current == State.Healthy && otherHuman && otherHuman.Current == State.Infected)
            Infect();

        if (Current == State.Infected && other.CompareTag("Projectile"))
            Die();
    }

    void ShootAt(Vector2 worldPos)
    {
        if (!projectilePrefab) return;

        Projectile p = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
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
            if (!h || h == this) continue;
            if (h.Current != State.Healthy) continue;

            float d = (h.transform.position - transform.position).sqrMagnitude;
            if (d < bestDist)
            {
                bestDist = d;
                best = h.transform;
            }
        }
        return best;
    }
                     //Tracks nearest infected to run away from
    Transform FindNearestInfected()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Transform best = player ? player.transform : null;
        float bestDist = best ? (best.position - transform.position).sqrMagnitude : float.MaxValue;

        Humans[] all = FindObjectsByType<Humans>(FindObjectsSortMode.None);
        foreach (var h in all)
        {
            if (!h || h == this) continue;
            if (h.Current != State.Infected) continue;

            float d = (h.transform.position - transform.position).sqrMagnitude;
            if (d < bestDist)
            {
                bestDist = d;
                best = h.transform;
            }
        }
        return best;
    }
}

