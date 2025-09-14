using UnityEngine;

/// <summary>
/// Base class for wall objects.
/// </summary>
public abstract class WallBase
{
    /// <summary>
    /// Reference to the instance of the wrapper class that contains this.
    /// </summary>
    public Wall wallComp;

    /// <summary>
    /// called every frame
    /// </summary>
    public virtual void OnUpdate()
    {
        // does nothing by default
    }

    /// <summary>
    /// called when a projectile collides with the wall
    /// </summary>
    public virtual void OnProjectileCollision(Projectile projectile)
    {
        // does nothing by default
    }

    /// <summary>
    /// called when a player collides with the wall.
    /// </summary>
    /// <remarks>usually not used.</remarks>
    public virtual void OnPlayerCollision(Player player)
    {
        // does nothing by default
    }

    /// <summary>
    /// called when another wall collides with the wall.
    /// </summary>
    /// <remarks>usually not used.</remarks>
    public virtual void OnWallCollision(Wall wall)
    {
        // does nothing by default
    }
}
