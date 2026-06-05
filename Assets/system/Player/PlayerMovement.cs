using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Physical Specs")]
    public float mass = 100f;           // kg
    public float moveForce = 5000f;     // N
    public float maxVelocity = 8f;      // m/s

    [Header("Custom Damping (Friction)")]
    public float groundDamping = 4f;    // *koef
    public float airDamping = 0.05f;    // *koef
    public float extraGravity = 3.721f;    // m/s^2

    private Rigidbody rb;
    private Vector2 moveInput;
    private Animator anim;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        rb.mass = mass;
        rb.linearDamping = 0;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void Update()
    {
        if (Keyboard.current != null)
        {
            float x = (Keyboard.current.dKey.isPressed ? 1 : 0) - (Keyboard.current.aKey.isPressed ? 1 : 0);
            float z = (Keyboard.current.wKey.isPressed ? 1 : 0) - (Keyboard.current.sKey.isPressed ? 1 : 0);
            moveInput = new Vector2(x, z).normalized;
        }

        float horizontalSpeed = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z).magnitude;

        if (anim != null)
        {
            anim.SetFloat("Speed", horizontalSpeed / maxVelocity);
        }
    }

    void FixedUpdate()
    {
        ApplyMovement();
        ApplyCustomDamping();
        ApplyExtraGravity();
        LimitVelocity();
    }

    void ApplyMovement()
    {
        if (moveInput != Vector2.zero)
        {
            Vector3 worldMove = new Vector3(moveInput.x, 0, moveInput.y);
            rb.AddForce(worldMove * moveForce, ForceMode.Force);
        }
    }

    void ApplyCustomDamping()
    {
        Vector3 vel = rb.linearVelocity;
        Vector3 dampingForce = new Vector3(-vel.x * groundDamping, 0, -vel.z * groundDamping);

        rb.AddForce(dampingForce, ForceMode.Acceleration);
    }

    void ApplyExtraGravity()
    {
        rb.AddForce(Vector3.down * extraGravity, ForceMode.Acceleration);
    }

    void LimitVelocity()
    {
        Vector3 horizontalVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        if (horizontalVel.magnitude > maxVelocity)
        {
            horizontalVel = horizontalVel.normalized * maxVelocity;
            rb.linearVelocity = new Vector3(horizontalVel.x, rb.linearVelocity.y, horizontalVel.z);
        }
    }
}