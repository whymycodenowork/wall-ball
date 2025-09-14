using UnityEngine;

namespace Projectiles
{
    public abstract class BallBase : ProjectileBase
    {
        /// <summary>
        /// How bouncy the ball is. 1 is perfect bounce and 0 is no bounce.
        /// </summary>
        public virtual float BounceFactor { get; } = 1f;
        /// <summary>
        /// Number of bounces before the ball is destroyed. Use a -1 for an infinite (technically the integer limit) number of bounces.
        /// </summary>
        public virtual int Bounces { get; set; } = -1;
        public override void OnWallCollision(Wall wall)
        {
            if (BounceFactor <= 0f)
            {
                DestroyProjectile();
                return; // no bounce so destroy
            }
            if (Bounces == 0)
            {
                DestroyProjectile();
                return; // no bounces left so also destroy
            }
            projComp.velocity = BounceFactor * Velocity.magnitude * ((Vector2)(projComp.transform.position - wall.transform.position)).normalized; // reflect velocity
            // for testing
            Debug.Log("bonk");
        }
    }
}