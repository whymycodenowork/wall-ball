using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5;

    // Update is called once per frame
    void Update()
    {
        // Move the player based on input
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");
        Vector2 movement = new(moveHorizontal, moveVertical);
        movement.Normalize();
        movement *= speed * Time.deltaTime;
        transform.Translate(movement, Space.World);

        // Rotate the player to face the mouse cursor
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePosition - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
