using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    /// <summary>
    /// The velocity of the projectile in units per second
    /// </summary>
    public Vector2 velocity;

    /// <summary>
    /// How long the projectile lasts (in seconds) before disappearing
    /// </summary>
    public virtual float Lifetime { get; } = 5f;

    /// <summary>
    /// How much the projectile slows down over time
    /// </summary>
    /// <remarks>
    /// 1 means no drag, and 0 means it can't move. Please don't set it to 0.
    /// </remarks>
    public virtual float Drag { get; } = 0.98f;

    public float lifeTimer;

    protected virtual void Start()
    {
        lifeTimer = Lifetime;
    }

    protected void Update() // not for overriding
    {
        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0)
        {
            DestroyProjectile();
            OnExpire();
        }
        // Apply drag
        velocity *= Drag;
        // Move the projectile
        transform.position += (Vector3)(velocity * Time.deltaTime);
        OnUpdate();
    }

    // For subclasses to override
    protected virtual void OnUpdate()
    {
        // Does nothing unless overridden
    }

    /// <summary>
    /// Call to destroy the projectile
    /// </summary>
    public void DestroyProjectile()
    {
        ProjectilePool.DespawnProjectile(this); // Return to pool
    }

    /// <summary>
    /// Called when the projectile expires or is destroyed
    /// </summary>
    protected virtual void OnExpire()
    {
        // Does nothing unless overridden
    }

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent<Projectile>(out Projectile projectile))
        {
            OnProjectileCollision(projectile);
        }
        else if (collision.collider.TryGetComponent<Wall>(out Wall wall))
        {
            OnWallCollision(wall);
        }
        else if (collision.collider.TryGetComponent<Player>(out Player player))
        {
            OnPlayerCollision(player);
        }
        else
        {
            // Unknown collision, just destroy the projectile
            DestroyProjectile();
        }
    }

    protected virtual void OnWallCollision(Wall wall)
    {
        // Does nothing unless overridden
    }

    protected virtual void OnProjectileCollision(Projectile projectile)
    {
        // Does nothing unless overridden
        // Most projectiles will just pass through each other
    }

    protected virtual void OnPlayerCollision(Player player)
    {
        // usually destroy the projectile and damage the player
    }
}
