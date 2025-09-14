namespace Projectiles.Balls
{
    public class BasicBall : BallBase
    {
        public override float Damage => 5f;

        public override float Speed => 7f;

        public override float Lifetime => 20;

        public override float Size => 0.5f;
        public override ushort TextureID => 1; // TODO: add textures (hard cus i suck at drawing)

        public override float BounceFactor => 0.8f;

        public override int Bounces { get; set; } = 3;
    }
}