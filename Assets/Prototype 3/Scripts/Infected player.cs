using UnityEngine;
using UnityEngine.InputSystem;   

public class Infectedplayer : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;

    [Header("Dash")]
    public float dashSpeed = 20f;      // how fast the dash is 
    public float dashDuration = 0.15f; // how long the dash lasts for
    public float dashCooldown = 1.0f;  // cooldown for dash

    private Rigidbody2D rb;
    private PlayerControls controls;
    private Vector2 moveInput;

    private bool isDashing = false;
    private float dashEndTime = 0f;
    private float nextDashTime = 0f;
    private Vector2 dashDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;

        controls = new PlayerControls();

        // Movement input 
        controls.Gameplay.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Gameplay.Move.canceled += ctx => moveInput = Vector2.zero;

        // Dash input
        controls.Gameplay.Dash.performed += ctx => TryDash();
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
        if (isDashing)
        {
            // During dash it override normal movement
            rb.linearVelocity = dashDirection * dashSpeed;

            // End dash after time
            if (Time.time >= dashEndTime)
            {
                isDashing = false;
            }
        }
        else
        {
            // Normal movement
            rb.linearVelocity = moveInput * moveSpeed;
        }
    }

    private void TryDash()
    {
        // Can't dash if on cooldown
        if (Time.time < nextDashTime)
            return;

        // Can't dash if not moving
        if (moveInput.sqrMagnitude <= 0.01f)
            return;

        // Start dashing
        isDashing = true;
        dashDirection = moveInput.normalized;
        dashEndTime = Time.time + dashDuration;
        nextDashTime = Time.time + dashCooldown;
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

        // If hit by projectile then Game Over
        if (other.CompareTag("Projectile"))
        {
            Gamemanager.Instance.GameOver();
        }
    }
}