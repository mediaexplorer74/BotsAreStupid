// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.Portal
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

#nullable disable
namespace BotsAreStupid
{
  internal class Portal : ParticleSpawningObject
  {
    private static Microsoft.Xna.Framework.Color hitboxColor = ColorManager.GetColor("red");
    private const int clearParticleAmount = 5;
    private int energyLevel;

    protected override bool ShowHitboxEnabled
    {
      get => base.ShowHitboxEnabled && VarManager.GetBool("showhitboxes_special");
    }

    protected override Microsoft.Xna.Framework.Color HitboxColor => Portal.hitboxColor;

    protected override float ParticleTime => this.CurrentSimulation.SimulationTime;

    public Portal(Simulation simulation, float x, float y)
      : base(simulation, x, y, 50, 50, texture: TextureManager.GetTexture("tileset"), spritePos: new Microsoft.Xna.Framework.Rectangle?(TextureManager.GetSpritePos("portal")), maxParticles: 0)
    {
      this.IsScalable = false;
      this.checkPlayerCollision = true;
      this.particleConfig = new ParticleConfig()
      {
        texture = this.texture,
        spritePos = new Microsoft.Xna.Framework.Rectangle?(TextureManager.GetSpritePos("energyorb")),
        randomLifeTime = new Vector2?(new Vector2(3.5f, 1.5f)),
        randomStartSize = new Vector2?(new Vector2(7f, 2f)),
        color = Microsoft.Xna.Framework.Color.White,
        fadeAway = true,
        fadeIn = true,
        startDir = Vector2.UnitY,
        startDirRandomness = 180f,
        randomStartSpeed = new Vector2?(new Vector2(-1.5f, 0.8f)),
        startOffset = (float) (this.Width / 3 * 2)
      };
    }

    protected override void UpdateParticleConfig()
    {
      base.UpdateParticleConfig();
      this.particleConfig.startPos -= Vector2.One * 2f;
    }

    public override void Update(float deltaTime)
    {
      this.UpdateEnergyLevel();
      base.Update(deltaTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
      base.Draw(spriteBatch);
      if (this.energyLevel >= 1)
        spriteBatch.Draw(this.texture, new Vector2(this.X + 5f, this.Y + 5f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(162, 210, 13, 13)), this.color, 0.0f, Vector2.Zero, this.renderScale, SpriteEffects.None, LayerDepths.Portal);
      if (this.energyLevel >= 2)
        spriteBatch.Draw(this.texture, new Vector2(this.X + 4f, this.Y + 28f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(161, 225, 13, 13)), this.color, 0.0f, Vector2.Zero, this.renderScale, SpriteEffects.None, LayerDepths.Portal);
      if (this.energyLevel >= 3)
        spriteBatch.Draw(this.texture, new Vector2(this.X + 28f, this.Y + 28f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(177, 225, 13, 13)), this.color, 0.0f, Vector2.Zero, this.renderScale, SpriteEffects.None, LayerDepths.Portal);
      if (this.energyLevel >= 4)
        spriteBatch.Draw(this.texture, new Vector2(this.X + 28f, this.Y + 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(177, 209, 13, 13)), this.color, 0.0f, Vector2.Zero, this.renderScale, SpriteEffects.None, LayerDepths.Portal);
      if (this.energyLevel < 5)
        return;
      spriteBatch.Draw(this.texture, new Vector2(this.X + 7.5f, this.Y + 7.5f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(131, 179, 25, 25)), this.color, 0.0f, Vector2.Zero, this.renderScale, SpriteEffects.None, LayerDepths.Portal);
    }

    public override GameObject Copy(Simulation intoSimulation = null)
    {
      return (GameObject) new Portal(intoSimulation ?? this.CurrentSimulation, this.X, this.Y);
    }

    public void UpdateEnergyLevel()
    {
      Simulation simulation = this.CurrentSimulation.IsIntro ? SimulationManager.MainSimulation : this.CurrentSimulation;
      this.energyLevel = Math.Max(0, 5 - simulation.CountType(typeof (EnergyOrb), condition: (Simulation.CountTypeCondition) (g => !(g as EnergyOrb).IsDisabled)) - simulation.CountType(typeof (ParticleTrail), condition: (Simulation.CountTypeCondition) (g => (g as ParticleTrail).IsFromPickup)));
      this.particleStartDelay = (float) (0.059999998658895493 - (double) this.energyLevel * 0.0099999997764825821);
      this.SetMaxParticles(this.energyLevel * 64 / 3);
      this.particlesEnabled = this.energyLevel >= 1;
    }

    protected override void OnPlayerCollision(Player p)
    {
      base.OnPlayerCollision(p);
      if (this.CurrentSimulation.IsFinished || this.energyLevel < 5)
        return;
      this.CurrentSimulation.OnLevelFinish();
      if ((this.CurrentSimulation.RenderDefault || this.CurrentSimulation.IsIntro) && !VarManager.IsOverclocked)
      {
        Vector2 center = this.Center;
        for (int index = 0; index < 5 * VarManager.GetInt("particleamount"); ++index)
        {
          Vector2 vector2 = Utility.RandomizeVector(Vector2.UnitY, 180f);
          ParticleTrail particleTrail = new ParticleTrail(p.CurrentSimulation, (float) (int) Utility.RandomizeNumber(2.3f, 1f), (int) Utility.RandomizeNumber(4f, 1f), Microsoft.Xna.Framework.Color.White, center + vector2 * (float) this.Width / 2f, center + vector2 * Utility.RandomizeNumber(70f, 20f), TextureManager.GetTexture("tileset"), new Microsoft.Xna.Framework.Rectangle?(TextureManager.GetSpritePos("energyorb")), scale: 0.05f);
        }
      }
      if (this.CurrentSimulation.IsIntro)
        SoundManager.Play("success", 0.25f);
    }
  }
}
