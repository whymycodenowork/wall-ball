using UnityEngine;

namespace Projectiles
{
    public abstract class BallBase : ProjectileBase
    {
        /// <summary>
        /// How bouncy the ball is. 1 is perfect bounce and 0 is no bounce.
        /// </summary>
        protected virtual float BounceFactor { get; } = 1f;
        /// <summary>
        /// Number of bounces before the ball is destroyed. Use a -1 for an infinite (technically the integer limit) number of bounces.
        /// </summary>
        protected virtual int Bounces { get; set; } = -1;
        public override void OnWallCollision(Wall wall)
        {
            if (BounceFactor <= 0f)
            {
                DestroyProjectile();
                return; // no bounce so destroy it ig
            }
            if (Bounces == 0)
            {
                DestroyProjectile();
                return; // no bounces left so also destroy
            }

            Vector2 normal = (projComp.transform.position - wall.transform.position).normalized;

            var distToPlayer = Vector2.Distance(projComp.transform.position, wall.player.transform.position);

            if (distToPlayer < wall.radius - wall.thickness)
            {
                normal *= -1;
            }

            Vector2 relativeVelocity = Velocity;
            
            relativeVelocity -= wall.player.rb.linearVelocity; // this *might* work
            
            Velocity = Vector2.Reflect(relativeVelocity, normal) * BounceFactor; // reflect velocity
            
            // for testing
            Debug.Log("bonk");
        }
        
        // bounce off of it
        public override void OnObstacleCollision(Collision2D collision)
        {
            
        }
    }
}