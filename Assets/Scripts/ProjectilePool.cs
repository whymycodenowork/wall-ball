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
    /// Spawns a projectile of type T at the given position and Velocity.
    /// </summary>
    /// <typeparam name="T">The type of the projectile.</typeparam>
    /// <param name="pos">Where to spawn the projectile.</param>
    /// <param name="vel">The Velocity of the projectile.</param>
    public static Projectile SpawnProjectile<T>(Vector2 pos, Vector2 vel) where T : ProjectileBase
    {
        if (inactiveProjectiles.Count > 0)
        {
            Projectile proj = inactiveProjectiles.Pop();
            proj.transform.position = pos;                    // set position
            proj.velocity = vel;                              // set Velocity
            proj.Proj = System.Activator.CreateInstance<T>(); // assign new instance of T
            proj.gameObject.transform.localScale = Vector3.one * proj.Proj.Size; // set size
            SpriteRenderer spr = proj.GetComponent<SpriteRenderer>(); // get sprite renderer
            spr.sprite = TextureManager.sprites[proj.Proj.TextureID]; // set sprite
            proj.gameObject.SetActive(true);
            proj.Proj.Init(); // initialize the projectile
            _ = activeProjectiles.Add(proj);
            return proj;
        }
        else
        {
            // Create a new projectile if none are available in the pool
            GameObject projObj = new(typeof(T).Name);
            projObj.SetActive(false);                                 // disable it
            projObj.transform.position = pos; // set position
            Projectile proj = projObj.AddComponent<Projectile>();     // add Projectile component
            proj.velocity = vel; // set Velocity
            proj.Proj = System.Activator.CreateInstance<T>();         // assign new instance of T
            proj.gameObject.transform.localScale = Vector3.one * proj.Proj.Size; // set size
            CircleCollider2D coll = projObj.AddComponent<CircleCollider2D>(); // add collider
            coll.isTrigger = false;
            coll.radius = 0.6f * proj.Proj.Size;                      // set collider radius
            SpriteRenderer spr = proj.AddComponent<SpriteRenderer>(); // add sprite renderer
            spr.sprite = TextureManager.sprites[proj.Proj.TextureID]; // set sprite
            Rigidbody2D rgdbdy = projObj.AddComponent<Rigidbody2D>(); // give it a rigidbody2d
            rgdbdy.bodyType = RigidbodyType2D.Kinematic;              // set body type to kinematic because it only for collisions not physics
            rgdbdy.gravityScale = 0f;                                 // disable gravity for it
            projObj.SetActive(true);                                  // enable it again
            proj.Proj.Init();                                         // initialize the projectile
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
