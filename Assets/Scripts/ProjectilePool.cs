using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class ProjectilePool
{
    /// <summary>
    /// HashSet of active projectiles in the scene.
    /// </summary>
    private static readonly HashSet<Projectile> ActiveProjectiles = new();
    /// <summary>
    /// Stack of inactive projectiles ready to be reused.
    /// </summary>
    private static readonly Stack<Projectile> InactiveProjectiles = new();

    /// <summary>
    /// Spawns a projectile of type T at the given position and Velocity.
    /// </summary>
    /// <typeparam name="T">The type of the projectile.</typeparam>
    /// <param name="pos">Where to spawn the projectile.</param>
    /// <param name="vel">The velocity of the projectile.</param>
    public static Projectile SpawnProjectile<T>(Vector2 pos, Vector2 vel) where T : ProjectileBase
    {
        if (InactiveProjectiles.Count > 0)
        {
            Projectile proj = InactiveProjectiles.Pop();
            proj.transform.position = pos;                    // set position
            proj.rb.linearVelocity = vel;                        // set Velocity
            proj.Proj = System.Activator.CreateInstance<T>(); // assign new instance of T
            proj.gameObject.transform.localScale = Vector3.one * proj.Proj.Size; // set size
            var spr = proj.GetComponent<SpriteRenderer>(); // get sprite renderer
            spr.sprite = TextureManager.sprites[proj.Proj.TextureID]; // set sprite
            proj.gameObject.SetActive(true);
            proj.Proj.Init(); // initialize the projectile
            _ = ActiveProjectiles.Add(proj);
            return proj;
        }
        else
        {
            // Create a new projectile if none are available in the pool
            GameObject projObj = new(typeof(T).Name);
            projObj.SetActive(false);                                 // disable it
            projObj.transform.position = pos;                      // set position
            var proj = projObj.AddComponent<Projectile>();            // add Projectile component
            proj.Proj = System.Activator.CreateInstance<T>();         // assign new instance of T
            proj.gameObject.transform.localScale = Vector3.one * proj.Proj.Size; // set size
            var coll = projObj.AddComponent<CircleCollider2D>();      // add collider
            coll.isTrigger = false;                                   // it is not a trigger
            coll.radius = 0.3f;                                       // set collider radius
            var spr = proj.AddComponent<SpriteRenderer>();            // add sprite renderer
            spr.sprite = TextureManager.sprites[proj.Proj.TextureID]; // set sprite
            var rigidbody = projObj.AddComponent<Rigidbody2D>();      // give it a rigidbody2d
            rigidbody.freezeRotation = true;
            rigidbody.linearVelocity = vel; // set Velocity
            proj.rb = rigidbody;
            rigidbody.bodyType = RigidbodyType2D.Dynamic;             // set rigidbody type
            rigidbody.gravityScale = 0f;                              // disable gravity for it
            projObj.SetActive(true);                                  // enable it again
            proj.Proj.Init();                                         // initialize the projectile
            _ = ActiveProjectiles.Add(proj);
            return proj;
        }
    }

    public static void DespawnProjectile(Projectile proj)
    {
        if (!ActiveProjectiles.Remove(proj))
        {
            return;
        }
        
        proj.gameObject.SetActive(false);
        InactiveProjectiles.Push(proj);
    }
}
