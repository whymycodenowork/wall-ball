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

        /// <summary>
        /// Returns the world-space velocity of the wall surface at the contact point,
        /// combining the player's linear movement and the wall's angular spin.
        /// </summary>
        private Vector2 GetWallVelocityAtPoint(Wall wall, Vector2 contactNormal, float contactDist)
        {
            // Angular velocity in rad/s from the rotation delta since last frame
            float currentAngle = Mathf.Atan2(wall.transform.up.y, wall.transform.up.x);
            float deltaAngleDeg = Mathf.DeltaAngle(wall.lastAngle * Mathf.Rad2Deg, currentAngle * Mathf.Rad2Deg);
            float angularVelocity = deltaAngleDeg * Mathf.Deg2Rad / Time.deltaTime;

            // Tangent is 90 decrees CCW from the outward normal
            Vector2 tangent = new(-contactNormal.y, contactNormal.x);

            // v_tangential = w × r  (scalar × distance to pivot)
            Vector2 rotationalVelocity = tangent * (angularVelocity * contactDist);

            return wall.player.velocity + rotationalVelocity;
        }

        /// <summary>
        /// Bounces the ball, accounting for the wall's linear and rotational movement.
        /// Override and call base to add special effects on bounce.
        /// </summary>
        public override void OnWallCollision(Wall wall)
        {
            if (BounceFactor <= 0f)
            {
                DestroyProjectile();
                return;
            }
            if (Bounces <= 0)
            {
                DestroyProjectile();
                return;
            }
            else
            {
                Bounces--;
            }

            Vector2 normal = (projComp.transform.position - wall.transform.position).normalized;
            float dist = Vector2.Distance(projComp.transform.position, wall.transform.position);

            Vector2 wallVelocity = GetWallVelocityAtPoint(wall, normal, dist);

            // parry?
            bool hit = dist < wall.radius && dist > wall.radius - wall.thickness;
            if (hit) // TODO: fix this, it bounces wrong
            {
                float wallDirection = Mathf.Atan2(wall.transform.up.y, wall.transform.up.x);

                float cross = (normal.x * wall.transform.up.y) - (normal.y * wall.transform.up.x);
                if (cross < 0)
                {
                    wallDirection += wall.angle / 2 * Mathf.Deg2Rad;
                    wallDirection += 90 * Mathf.Deg2Rad;
                }
                else if (cross > 0)
                {
                    wallDirection -= wall.angle / 2 * Mathf.Deg2Rad;
                    wallDirection -= 90 * Mathf.Deg2Rad;
                }

                Vector2 parryNormal = new(Mathf.Cos(wallDirection), Mathf.Sin(wallDirection));

                Vector2 relativeVelocity = Velocity - wallVelocity;
                Velocity = Vector2.Reflect(relativeVelocity, parryNormal) + wallVelocity;

                Debug.DrawRay(projComp.transform.position, parryNormal * Velocity.magnitude, Color.magenta);
                Debug.Log("ssss");
                return;
            }

            if (dist > wall.radius - wall.thickness)
            {
                normal *= -1;
            }

            Vector2 relVel = Velocity - wallVelocity;
            Velocity = (Vector2.Reflect(relVel, normal) * BounceFactor) + wallVelocity;

            projComp.transform.position += (Vector3)(normal * 0.01f);
            Debug.Log("bonk");
        }
    }
}