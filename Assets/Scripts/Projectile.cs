using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// wrapper MonoBehaviour for projectiles
/// </summary>
public class Projectile : MonoBehaviour
{
    private ProjectileBase _proj;

    /// <summary>
    /// an instance of the class that contains the logic for the projectile
    /// </summary>
    public ProjectileBase Proj
    {
        get => _proj;
        set
        {
            _proj = value;
            _proj.projComp = this;
        }
    }

    /// <summary>
    /// reference to the rigidbody of the projectile this is attached to
    /// </summary>
    public Rigidbody2D rb;

    /// <summary>
    /// the Velocity value belongs to this now because for some reason I have to or else everything breaks
    /// </summary>
    public Vector2 Velocity
    {
        get => rb.linearVelocity;
        set => rb.linearVelocity = value;
    }

    protected void Update() // not for overriding
    {
        Proj.lifeTimer -= Time.deltaTime;
        if (Proj.lifeTimer <= 0 && Proj.exists)
        {
            Proj.DestroyProjectile();
            Proj.OnExpire();
            return;
        }
        // Apply drag
        Velocity *= Mathf.Pow(Proj.Drag, Time.deltaTime);
        // Move the projectile
        transform.position += (Vector3)(Velocity * Time.deltaTime);
        Proj.OnUpdate();
    }

    private void OnTriggerEnter2D([NotNull] Collider2D collidedCollider)
    {
        if (collidedCollider.TryGetComponent(out Projectile projectile))
        {
            Proj.OnProjectileCollision(projectile);
        }
        else if (collidedCollider.TryGetComponent(out Wall wall))
        {
            Proj.OnWallCollision(wall);
        }
        else if (collidedCollider.TryGetComponent(out Player player))
        {
            Proj.OnPlayerCollision(player);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Obstacle"))
        {
            
        }
    }
}
