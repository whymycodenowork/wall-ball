using UnityEngine;

/// <summary>
/// wrapper class for projectiles to be pooled
/// </summary>
public class Projectile : MonoBehaviour
{
    private ProjectileBase _proj;
    public ProjectileBase Proj
    {
        get => _proj;
        set
        {
            _proj = value;
            _proj.projComp = this;
        }
    }

    public Vector2 velocity;

    protected void Update() // not for overriding
    {
        Proj.lifeTimer -= Time.deltaTime;
        if (Proj.lifeTimer <= 0 && Proj.exists)
        {
            Proj.DestroyProjectile();
            Proj.OnExpire();
        }
        // Apply drag
        velocity *= Proj.Drag;
        // Move the projectile
        transform.position += (Vector3)(velocity * Time.deltaTime);
        Proj.OnUpdate();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent(out Projectile projectile))
        {
            Proj.OnProjectileCollision(projectile);
        }
        else if (collision.collider.TryGetComponent(out Wall wall))
        {
            Proj.OnWallCollision(wall);
        }
        else if (collision.collider.TryGetComponent(out Player player))
        {
            Proj.OnPlayerCollision(player);
        }
        else
        {
            // Unknown collision, just destroy the projectile
            Proj.DestroyProjectile();
        }
    }
}
