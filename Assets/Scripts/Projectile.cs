using UnityEngine;

/// <summary>
/// wrapper class for projectiles to be pooled
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
    /// the Velocity value belongs to this now because for some reason i have to or else everything breaks
    /// </summary>
    public Vector2 velocity;

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
        velocity *= Mathf.Pow(Proj.Drag, Time.deltaTime);
        // Move the projectile
        transform.position += (Vector3)(velocity * Time.deltaTime);
        Proj.OnUpdate();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("bonk"); // bonk for debug
        if (collider.TryGetComponent(out Projectile projectile))
        {
            Proj.OnProjectileCollision(projectile);
        }
        else if (collider.TryGetComponent(out Wall wall))
        {
            Proj.OnWallCollision(wall);
        }
        else if (collider.TryGetComponent(out Player player))
        {
            Proj.OnPlayerCollision(player);
        }
        else
        {
            // Unknown collider, just destroy the projectile
            Proj.DestroyProjectile();
        }
    }
}
