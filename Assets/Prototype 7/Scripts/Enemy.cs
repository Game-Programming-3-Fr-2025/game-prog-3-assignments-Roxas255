using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 3.5f;
    public float turnAssist = 12f;          // higher = snappier tracking
    public float separationRadius = 0.7f;   // how close before separating a bit
    public float separationStrength = 1.5f; // small push away

    Transform player;
    Rigidbody2D rb;
    BoxCollider2D boundary;

    static Collider2D[] hits = new Collider2D[12];

    public void Init(Transform playerRef, BoxCollider2D boundaryRef)
    {
        player = playerRef;
        boundary = boundaryRef;
        gameObject.SetActive(true);
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (player == null) return;

        // --- Seek player
        Vector2 toPlayer = ((Vector2)player.position - rb.position).normalized;
        Vector2 desired = toPlayer * speed;

        // --- Light separation (avoid perfect clumps)
        int count = Physics2D.OverlapCircle(rb.position, separationRadius, new ContactFilter2D(), hits);
        Vector2 sep = Vector2.zero;
        for (int i = 0; i < count; i++)
        {
            var h = hits[i];
            if (h == null || h.attachedRigidbody == rb) continue;
            if (h.CompareTag("Enemy"))
            {
                Vector2 away = rb.position - (Vector2)h.transform.position;
                float d = away.magnitude;
                if (d > 0.0001f) sep += away / (d * d); // stronger when very close
            }
        }
        if (sep != Vector2.zero)
            desired += sep.normalized * separationStrength;

        // Smooth towards desired
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, desired, turnAssist * Time.fixedDeltaTime);

        // Safety: if somehow outside, disable (pooled)
        if (!InsideBoundary(rb.position))
            gameObject.SetActive(false);
    }

    bool InsideBoundary(Vector2 pos)
    {
        if (boundary == null) return true;
        // Convert to boundary local space and check bounds
        Vector2 local = boundary.transform.InverseTransformPoint(pos);
        var b = boundary.size * 0.5f;
        return (local.x >= -b.x && local.x <= b.x && local.y >= -b.y && local.y <= b.y);
    }
}
