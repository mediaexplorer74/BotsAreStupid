// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.ParticleGroup
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;

#nullable disable
namespace BotsAreStupid
{
    internal class ParticleGroup : ParticleSpawningObject
    {
        private static Color hitboxColor = ColorManager.GetColor("white");

        protected override bool ShowHitboxEnabled
        {
            get => base.ShowHitboxEnabled && VarManager.GetBool("showhitboxes_particles");
        }

        protected override Color HitboxColor => ParticleGroup.hitboxColor;

        public ParticleGroup(
          Simulation simulation,
          Vector2 position,
          ParticleConfig config,
          int amount = 16,
          float speedMultiplier = 1f)
          : 
            base(simulation, position.X, position.Y, 5, 5, null, 
                spritePos: null, maxParticles: amount, speedMultiplier: speedMultiplier)
        {
            this.particleConfig = config;
            this.particleStartDelay = 0.0f;
            this.particleIndividualRandomOffsets = true;
            this.enableDraw = false;
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            float num = this.particleConfig.lifeTime;
            if (this.particleConfig.randomLifeTime.HasValue)
                num = this.particleConfig.randomLifeTime.Value.X + this.particleConfig.randomLifeTime.Value.Y;
            if ((double)this.lifeTime <= (double)num)
                return;
            this.Destroy();
        }
    }
}
