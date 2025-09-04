using System.Collections.Generic;
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
    public static void SpawnProjectile<T> (Vector2 pos, Vector2 vel) where T : Projectile
    {
        if (activeProjectiles.Count > 0)
        {
            Projectile proj = inactiveProjectiles.Pop();
            proj.transform.position = pos;
            proj.velocity = vel;
            RemoveAllComponents(proj.gameObject);
            proj = proj.gameObject.AddComponent<T>();
            proj.gameObject.SetActive(true);
            _ = activeProjectiles.Add(proj);
        }
        else
        {
            // Create a new projectile if none are available in the pool
            GameObject projObj = new(typeof(T).Name);
            projObj.transform.position = pos;
            T proj = projObj.AddComponent<T>();
            proj.velocity = vel;
            _ = activeProjectiles.Add(proj);
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

    /// <summary>
    /// Removes all components from a GameObject except for the Transform component.
    /// </summary>
    /// <param name="obj"></param>
    public static void RemoveAllComponents(GameObject obj)
    {
        Component[] comps = obj.GetComponents<Component>();

        foreach (Component comp in comps)
        {
            if (comp is not Transform) // don’t destroy Transform
            {
                Object.Destroy(comp);
            }
        }
    }

}
