using UnityEngine;
using UnityEngine.InputSystem;   // ⬅️ NEW INPUT SYSTEM NAMESPACE

public class Infectedplayer : MonoBehaviour
{
    public float moveSpeed = 8f;

    private Rigidbody2D rb;
    private PlayerControls controls;   // auto-generated class from Input Actions
    private Vector2 moveInput;         // stores WASD input

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;

        // create controls object
        controls = new PlayerControls();

        // subscribe to Move action events
        controls.Gameplay.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Gameplay.Move.canceled += ctx => moveInput = Vector2.zero;
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }

    private void FixedUpdate()
    {
        // movement using linearVelocity (your style)
        rb.linearVelocity = moveInput * moveSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Infect healthy humans on touch
        var human = other.GetComponent<Humans>();
        if (human != null && human.Current == Humans.State.Healthy)
        {
            human.Infect();
            return;
        }

        // If hit by projectile → Game Over
        if (other.CompareTag("Projectile"))
        {
            Gamemanager.Instance.GameOver();
        }
    }
}
