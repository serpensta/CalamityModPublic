using System;
using Microsoft.Xna.Framework;
using Terraria;

namespace CalamityMod.Particles
{
    public class PlagueHumidifierMist : Particle
    {
        public override bool SetLifetime => true;
        public override int FrameVariants => 7;

        public override string Texture => "CalamityMod/Particles/PlagueHumidifierMist";

        public PlagueHumidifierMist(Vector2 relativePosition, int lifetime, float scale, Vector2 speed)
        {
            RelativeOffset = relativePosition;
            Velocity = speed;
            Scale = scale;
            Variant = Main.rand.Next(7);
            Lifetime = lifetime;
        }

        public override void Update()
        {
            float opacity = Utils.GetLerpValue(1f, 0.85f, LifetimeCompletion, true);
            Color *= opacity;
        }

    }
}
