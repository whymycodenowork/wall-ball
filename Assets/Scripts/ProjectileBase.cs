using System;
using UnityEngine;

public abstract class ProjectileBase
{
    /// <summary>
    /// How much damage the projectile does on hit
    /// </summary>
    public virtual float Damage { get; } = 0f;
    /// <summary>
    /// How much armor the projectile ignores
    /// </summary>
    public virtual float ArmorPenetration { get; } = 0f;
    /// <summary>
    /// How fast the projectile moves
    /// </summary>
    public virtual float Speed { get; } = 5f;
    /// <summary>
    /// The size of the projectile.
    /// </summary>
    /// <remarks>
    /// The scale of the <see cref="Transform"/> of the <see cref="GameObject"/>. This affects both visual size and collider size.
    /// </remarks>
    public virtual float Size { get; } = 1f;
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
    public virtual float Drag { get; } = 1f;

    /// <summary>
    /// The ID of the texture to use for this projectile.
    /// </summary>
    public abstract ushort TextureID { get; }

    public float lifeTimer;

    public bool exists = true;

    /// <summary>
    /// How long the projectile has existed (in seconds)
    /// </summary>
    public float Age => Lifetime - lifeTimer;

    public Vector2 Velocity => projComp.velocity;

    public Projectile projComp;

    public virtual void Init()
    {
        lifeTimer = Lifetime;
    }

    // For subclasses to override
    public virtual void OnUpdate()
    {
        // Does nothing unless overridden
    }

    /// <summary>
    /// Call to destroy the projectile
    /// </summary>
    public virtual void DestroyProjectile()
    {
        ProjectilePool.DespawnProjectile(projComp); // Return to pool
        exists = false;
    }

    /// <summary>
    /// Called when the projectile expires or is destroyed
    /// </summary>
    public virtual void OnExpire()
    {
        // Does nothing unless overridden
    }

    /// <summary>
    /// Hook for when the ball collides with a wall.
    /// </summary>
    /// <param name="wall">The wall that was hit.</param>
    public virtual void OnWallCollision(Wall wall)
    {
        // Does nothing unless overridden
    }

    public virtual void OnProjectileCollision(Projectile projectile)
    {
        // Does nothing unless overridden
        // Most projectiles will just pass through each other
    }

    public virtual void OnPlayerCollision(Player player)
    {
        // usually destroy the projectile and damage the player
    }
}
