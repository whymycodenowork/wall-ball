using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class ProjectilePool
{
    /// <summary>
    /// List of active projectiles in the scene.
    /// </summary>
    public static HashSet<Projectile> activeProjectiles = new();
    /// <summary>
    /// Stack of inactive projectiles ready to be reused.
    /// </summary>
    public static Stack<Projectile> inactiveProjectiles = new();

    /// <summary>
    /// Spawns a projectile of type T at the given position and direction.
    /// </summary>
    /// <typeparam name="T">The type of the projectile.</typeparam>
    /// <param name="pos">Where to spawn the projectile.</param>
    /// <param name="vel">The velocity of the projectile.</param>
    public static Projectile SpawnProjectile<T>(Vector2 pos, Vector2 vel) where T : ProjectileBase
    {
        if (inactiveProjectiles.Count > 0)
        {
            Projectile proj = inactiveProjectiles.Pop();
            proj.transform.position = pos; // set position
            proj.velocity = vel; // set velocity
            proj.gameObject.transform.localScale = Vector3.one * proj.Proj.Size; // set size
            proj.Proj = System.Activator.CreateInstance<T>(); // assign new instance of T
            SpriteRenderer spr = proj.GetComponent<SpriteRenderer>(); // get sprite renderer
            proj.gameObject.SetActive(true);
            proj.Proj.Init(); // initialize the projectile
            _ = activeProjectiles.Add(proj);
            return proj;
        }
        else
        {
            // Create a new projectile if none are available in the pool
            GameObject projObj = new(typeof(T).Name);
            projObj.transform.position = pos; // set position
            Projectile proj = projObj.AddComponent<Projectile>(); // add Projectile component
            proj.velocity = vel; // set velocity
            proj.gameObject.transform.localScale = Vector3.one * proj.Proj.Size; // set size
            proj.Proj = System.Activator.CreateInstance<T>(); // assign new instance of T
            proj.Proj.Init(); // initialize the projectile
            CircleCollider2D c = projObj.AddComponent<CircleCollider2D>(); // add collider
            SpriteRenderer spr = proj.AddComponent<SpriteRenderer>(); // add sprite renderer
            // TODO: add sprites and stuff
            c.isTrigger = false;
            _ = activeProjectiles.Add(proj);
            return proj;
        }
    }

    public static void DespawnProjectile(Projectile proj)
    {
        if (activeProjectiles.Remove(proj))
        {
            proj.gameObject.SetActive(false);
            inactiveProjectiles.Push(proj);
        }
        else
        {
            Debug.LogWarning("Attempted to deactivate a projectile that is not active.");
        }
    }
}
