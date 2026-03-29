using UnityEngine;

public class Player : MonoBehaviour
{
    public Vector2 velocity;

    public float acceleration = 50f; // how fast the player speeds up
    public float deceleration = 60f; // how fast the player slows down when no input
    public float maxSpeed = 5f;      // maximum movement speed

    private void Update()
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
            velocity += input * acceleration * Time.deltaTime;
        }
        else
        {
            // Apply deceleration when no input
            float speed = velocity.magnitude;
            speed -= deceleration * Time.deltaTime;
            speed = Mathf.Max(speed, 0); // don't go negative
            if (velocity != Vector2.zero)
            {
                velocity = velocity.normalized * speed;
            }
        }

        // Clamp to max speed
        if (velocity.magnitude > maxSpeed)
        {
            velocity = velocity.normalized * maxSpeed;
        }

        // Move the player
        transform.Translate(velocity * Time.deltaTime, Space.World);

        // Rotate to face mouse
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}