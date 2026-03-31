using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody2D rb;
    
    public float acceleration = 50f; // how fast the player speeds up
    public float deceleration = 60f; // how fast the player slows down when no input
    public float maxSpeed = 5f;      // maximum movement speed

    private void FixedUpdate()
    {
        // Read input
        Vector2 input = new(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (input.sqrMagnitude > 1)
        {
            input.Normalize(); // prevent diagonal being faster
        }

        // Apply acceleration if there is input
        if (input != Vector2.zero)
        {
            rb.linearVelocity += input * (acceleration * Time.deltaTime);
        }
        else
        {
            // Apply deceleration when no input
            float speed = rb.linearVelocity.magnitude;
            speed -= deceleration * Time.deltaTime;
            speed = Mathf.Max(speed, 0); // don't go negative
            if (rb.linearVelocity != Vector2.zero)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * speed;
            }
        }

        // Clamp to max speed
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }

        // Rotate to face mouse
        if (Camera.main != null)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mousePos - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}