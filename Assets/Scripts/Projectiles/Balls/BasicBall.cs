namespace Projectiles.Balls
{
    /// <summary>
    /// a very basic ball
    /// </summary>
    public class BasicBall : BallBase
    {
        public override float Damage => 5f;
        public override float Speed => 7f;
        public override float Lifetime => 20;
        public override float Size => 0.5f;
        public override ushort TextureID => 0;
        public override float BounceFactor => 0.8f;
        public override int Bounces { get; set; } = 3;
    }
}