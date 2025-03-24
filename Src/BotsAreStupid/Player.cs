// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.Player
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

#nullable disable
namespace BotsAreStupid
{
  internal class Player : PhysicsObject
  {
    private const float acceleration = 40f;
    private const float maxSpeed = 200f;
    private const float jumpSpeed = 400f;
    private float walkSoundCooldown = 0.0f;
    private const float walkSoundCooldownMax = 0.15f;
    private float walkParticleCooldown = 0.0f;
    private const float walkParticleCooldownMax = 0.05f;
    private float jumpEffectCooldown = 0.0f;
    private const float jumpEffectCooldownMax = 0.05f;
    private Animation idleAnimation;
    private Animation jumpAnimation;
    private Animation walkAnimation;
    private Animation waveAnimation;
    private bool manualControlsEnabled;
    private Vector2 lastPos;
    private float idleTime = 0.0f;
    private float groundedTime = -1f;
    private float afterGroundedTime = 0.0f;
    private bool hasActed;
    private float playTime = 0.0f;
    private int maxLifeTime = 999;
    private float timeSinceAction = 0.0f;
    private static ParticleConfig walkParticleConfig = new ParticleConfig()
    {
      texture = TextureManager.GetCircleTexture(5),
      randomLifeTime = new Vector2?(new Vector2(0.5f, 0.3f)),
      randomStartSize = new Vector2?(new Vector2(5f, 2f)),
      randomizeColor = true,
      randomColorRange = 50,
      color = ColorManager.GetColor("gray"),
      accelerationY = 3f,
      fadeAway = true
    };
    private static ParticleConfig landingParticleConfig = new ParticleConfig()
    {
      texture = TextureManager.GetCircleTexture(5),
      randomLifeTime = new Vector2?(new Vector2(0.5f, 0.3f)),
      randomStartSize = new Vector2?(new Vector2(5f, 2f)),
      startDir = -Vector2.UnitY,
      startDirRandomness = 90f,
      randomizeColor = true,
      randomColorRange = 20,
      color = ColorManager.GetColor("gray"),
      accelerationY = 1f,
      fadeAway = true,
      randomStartSpeed = new Vector2?(new Vector2(40f, 30f)),
      maxRepeats = 1
    };
    private static ParticleConfig waveParticleConfig = new ParticleConfig()
    {
      startSize = 5,
      fadeIn = true,
      fadeAway = true,
      color = ColorManager.GetColor("white"),
      trail = true,
      trailColor = ColorManager.GetColor("white"),
      trailSize = 1f,
      round = true,
      trailInterval = 0.01f,
      startDir = -Vector2.UnitY,
      startDirRandomness = 90f,
      randomStartOffset = new Vector2?(new Vector2(30f, 10f)),
      randomStartSpeed = new Vector2?(new Vector2(150f, 50f)),
      randomLifeTime = new Vector2?(new Vector2(0.2f, 0.1f)),
      randomTrailLifeTime = new Vector2?(new Vector2(0.25f, 0.05f))
    };
    private static ParticleConfig jumpParticleConfig = new ParticleConfig()
    {
      startSize = 5,
      fadeIn = true,
      fadeAway = true,
      trail = true,
      randomizeColor = true,
      randomColorRange = 32,
      trailSize = 1f,
      round = true,
      trailInterval = 0.01f,
      randomStartSize = new Vector2?(new Vector2(4f, 1f)),
      startDir = -Vector2.UnitY,
      startDirRandomness = 70f,
      randomStartSpeed = new Vector2?(new Vector2(200f, 66f)),
      randomLifeTime = new Vector2?(new Vector2(0.09f, 0.045f)),
      randomTrailLifeTime = new Vector2?(new Vector2(0.25f, 0.125f))
    };
    private static ParticleConfig failedJumpParticleConfig = new ParticleConfig()
    {
      startSize = 5,
      fadeIn = true,
      fadeAway = true,
      trail = true,
      randomizeColor = true,
      randomColorRange = 32,
      trailSize = 1f,
      round = true,
      trailInterval = 0.01f,
      randomStartSize = new Vector2?(new Vector2(5f, 1f)),
      startDir = Vector2.UnitY,
      startDirRandomness = 70f,
      randomStartSpeed = new Vector2?(new Vector2(150f, 50f)),
      randomLifeTime = new Vector2?(new Vector2(0.07f, 0.035f)),
      randomTrailLifeTime = new Vector2?(new Vector2(0.2f, 0.1f))
    };

    public ScriptInterpreter ScriptInterpreter { get; private set; }

    public bool HasPickedUpOrb { get; set; } = false;

    public float PlayTime => (float) Math.Round((double) this.playTime, 3);

    public float GroundedTime => (float) Math.Round((double) this.groundedTime, 3);

    public float AfterGroundedTime => (float) Math.Round((double) this.afterGroundedTime, 3);

    protected override bool ShowHitboxEnabled => VarManager.GetBool("showhitboxes_special");

    protected override Microsoft.Xna.Framework.Color HitboxColor => Microsoft.Xna.Framework.Color.Green;

    private bool CanAct
    {
      get
      {
        PopupMenu instance = PopupMenu.Instance;
        return instance == null || !instance.IsActive;
      }
    }

    private Animation IdleAnimation
    {
      get => !this.ScriptInterpreter.IsWaving ? this.idleAnimation : this.waveAnimation;
    }

    public Player(
      Simulation simulation,
      float x,
      float y,
      int width,
      int height,
      Microsoft.Xna.Framework.Color color,
      Texture2D texture,
      Microsoft.Xna.Framework.Rectangle? spritePos = null)
      : base(simulation, x, y, width, height, color, texture, spritePos)
    {
      this.layerDepth = LayerDepths.Player;
      this.IsSelectable = false;
      this.ScriptInterpreter = new ScriptInterpreter(this);
      this.CreateAnimations();
      this.SetAnimation(this.IdleAnimation);
      this.defaultSize = new Vector2?(new Vector2(26f, 28f));
      this.hook.SetRangeMultiplier(this.GetSizeMultiplier());
      this.maxLifeTime = VarManager.GetInt("maxplayerlifetime");
    }

    public override object Clone()
    {
      Player player = (Player) base.Clone();
      ScriptInterpreter scriptInterpreter = (ScriptInterpreter) this.ScriptInterpreter.Clone();
      player.ScriptInterpreter = scriptInterpreter;
      scriptInterpreter.SetPlayer(player);
      return (object) player;
    }

    public override void Update(float deltaTime)
    {
      base.Update(deltaTime);
      if ((double) this.walkSoundCooldown > 0.0)
        this.walkSoundCooldown -= deltaTime;
      if ((double) this.walkParticleCooldown > 0.0)
        this.walkParticleCooldown -= deltaTime;
      if ((double) this.jumpEffectCooldown > 0.0)
        this.jumpEffectCooldown -= deltaTime;
      if (this.IsGrounded && !this.CurrentSimulation.CheckMode && (double) this.walkParticleCooldown <= 0.0 && (double) Math.Abs(this.Velocity.X - (float) this.ConveyorSpeed * 0.8f) > 40.0)
      {
        if (VarManager.GetInt("particleamount") > 0)
        {
          float num = this.Velocity.X - (float) this.ConveyorSpeed;
          Vector2 vector2 = Utility.RandomizeVector(new Vector2((double) num > 0.0 ? -1f : 1f, -3f), 35f);
          Player.walkParticleConfig.startPos = this.Position + new Vector2((double) num > 0.0 ? 0.0f : (float) this.Width, (float) this.Height * 0.85f);
          Player.walkParticleConfig.startDir = vector2;
          Player.walkParticleConfig.startSpeed = Math.Abs(num) / 200f * Utility.RandomizeNumber(140f, 60f);
          Particle particle = new Particle(this.CurrentSimulation, Player.walkParticleConfig);
        }
        this.walkParticleCooldown = 0.05f;
      }
      if (this.IsGrounded)
        this.groundedTime = (double) this.groundedTime == -1.0 ? 0.0f : this.groundedTime + deltaTime;
      else if ((double) this.groundedTime > 0.0)
        this.groundedTime = 0.0f;
      if ((double) this.groundedTime != -1.0)
        this.afterGroundedTime += deltaTime;
      this.UpdateAnimation();
      this.ScriptInterpreter.Update(deltaTime);
      this.isBeeingMoved = false;
      int num1;
      if (!this.ScriptInterpreter.IsWalkingRight)
      {
        if (this.manualControlsEnabled && this.CanAct)
        {
          Simulation currentSimulation = this.CurrentSimulation;
          if ((currentSimulation != null ? (currentSimulation.IsMain ? 1 : 0) : 0) != 0)
          {
            num1 = Input.IsDown(KeyBind.GoRight) ? 1 : (Input.IsDown(KeyBind.Right) ? 1 : 0);
            goto label_22;
          }
        }
        num1 = 0;
      }
      else
        num1 = 1;
label_22:
      if (num1 != 0)
      {
        this.Walk();
      }
      else
      {
        int num2;
        if (!this.ScriptInterpreter.IsWalkingLeft)
        {
          if (this.manualControlsEnabled && this.CanAct)
          {
            Simulation currentSimulation = this.CurrentSimulation;
            if ((currentSimulation != null ? (currentSimulation.IsMain ? 1 : 0) : 0) != 0)
            {
              num2 = Input.IsDown(KeyBind.GoLeft) ? 1 : (Input.IsDown(KeyBind.Left) ? 1 : 0);
              goto label_31;
            }
          }
          num2 = 0;
        }
        else
          num2 = 1;
label_31:
        if (num2 != 0)
          this.Walk(false);
      }
      if (this.hasActed && this.CurrentSimulation != null && !this.CurrentSimulation.IsFinished)
      {
        if (this.manualControlsEnabled && this.CurrentSimulation.IsMain && (double) this.playTime == 0.0)
          SimulationManager.ForEachSimulation((SimulationManager.ForEachHandler) (simulation =>
          {
            if (simulation.IsMain || simulation.HasStarted)
              return;
            simulation.StartASAP();
          }));
        this.playTime += deltaTime;
      }
      this.CheckIdle(deltaTime);
      this.lastPos = this.Position;
      this.HasPickedUpOrb = false;
      if (!this.manualControlsEnabled)
        return;
      this.timeSinceAction += deltaTime;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
      base.Draw(spriteBatch);
      this.ScriptInterpreter?.Draw(spriteBatch);
      if (!VarManager.GetBool("showvelocityvector"))
        return;
      Utility.DrawLine(spriteBatch, this.Center, this.Center + this.RealVelocity * 2f, 3, new Microsoft.Xna.Framework.Color?(this.CurrentSimulation.IsMain ? Microsoft.Xna.Framework.Color.LimeGreen : Microsoft.Xna.Framework.Color.Green), this.CurrentSimulation.IsMain ? 1f : 0.99f, 0);
    }

    public void EnableManualControls()
    {
      if (this.manualControlsEnabled || !this.CurrentSimulation.IsMain)
        return;
      this.manualControlsEnabled = true;
      Input.Register(KeyBind.Jump, new InputAction.InputHandler(this.Jump));
      Input.Register(KeyBind.Up, new InputAction.InputHandler(this.Jump));
      Input.RegisterOnDown(KeyBind.UseHook, new InputAction.InputHandler(this.HookBind));
      if (VarManager.GetBool("record"))
      {
        Input.RegisterOnDown(KeyBind.GoLeft, new InputAction.InputHandler(this.RecordStartLeft));
        Input.RegisterOnUp(KeyBind.GoLeft, new InputAction.InputHandler(this.RecordStopLeft));
        Input.RegisterOnDown(KeyBind.GoRight, new InputAction.InputHandler(this.RecordStartRight));
        Input.RegisterOnUp(KeyBind.GoRight, new InputAction.InputHandler(this.RecordStopRight));
      }
    }

    private void UnRegisterInput()
    {
      int num;
      if (Input.Instance != null)
      {
        Simulation currentSimulation = this.CurrentSimulation;
        num = currentSimulation != null ? (currentSimulation.IsMain ? 1 : 0) : 0;
      }
      else
        num = 0;
      if (num == 0)
        return;
      Input.UnRegister(KeyBind.Jump, new InputAction.InputHandler(this.Jump));
      Input.UnRegister(KeyBind.Up, new InputAction.InputHandler(this.Jump));
      Input.UnRegister(KeyBind.UseHook, new InputAction.InputHandler(this.HookBind));
      Input.UnRegister(KeyBind.GoRight, new InputAction.InputHandler(this.RecordStartRight));
      Input.UnRegister(KeyBind.GoRight, new InputAction.InputHandler(this.RecordStopRight));
      Input.UnRegister(KeyBind.GoLeft, new InputAction.InputHandler(this.RecordStartLeft));
      Input.UnRegister(KeyBind.GoLeft, new InputAction.InputHandler(this.RecordStopLeft));
    }

    private void Walk(bool right = true)
    {
      Vector2 velocity = this.Velocity;
      velocity.X += (float) (40.0 * (right ? 1.0 : -1.0));
      if ((double) Math.Abs(velocity.X - (float) this.ConveyorSpeed) <= 200.0 * (double) this.GetSizeMultiplier() || (double) Math.Abs(this.Velocity.X) > (double) Math.Abs(velocity.X))
        this.SetVelocity(velocity);
      this.isBeeingMoved = true;
      if (this.IsGrounded && (double) this.walkSoundCooldown <= 0.0)
      {
        SoundManager.Play("footstep", 0.75f);
        this.walkSoundCooldown = 0.15f;
      }
      if (!this.hasActed)
        this.hasActed = true;
      this.flipTexture = !right;
    }

    public void Jump()
    {
      if (!this.CanAct)
        return;
      if (this.manualControlsEnabled && VarManager.GetBool("record"))
      {
        this.RecordWaitTime();
        TextEditor.Instance.AddLine("jump", true);
      }
      this.PlayJumpEffect();
      if (this.IsGrounded)
      {
        this.SetVelocity(this.Velocity.X, -400f * this.GetSizeMultiplier());
        SoundManager.Play("jump", 0.7f);
        this.SetAnimation(this.jumpAnimation);
        if (!this.hasActed)
          this.hasActed = true;
        this.IsGrounded = false;
      }
    }

    public void WallJump()
    {
      if (this.IsGrounded)
        return;
      this.hook.Use();
      SoundManager.Play("jump");
      this.SetAnimation(this.jumpAnimation);
      this.SetVelocity(this.Velocity.X, (float) (-400.0 * (this.IsBoosted ? 1.5 : 0.5)));
    }

    public void Hook(bool? right = null, bool down = false)
    {
      if (!this.CanAct)
        return;
      bool? nullable;
      if (this.manualControlsEnabled && VarManager.GetBool("record"))
      {
        this.RecordWaitTime();
        if (this.hook.IsAttached)
        {
          TextEditor.Instance.AddLine("unhook", true);
        }
        else
        {
          TextEditor instance = TextEditor.Instance;
          nullable = right;
          string s = "hook " + ((nullable.GetValueOrDefault() ? 1 : 0) != 0 ? nameof(right) : "left");
          instance.AddLine(s, true);
        }
      }
      BotsAreStupid.Hook hook = this.hook;
      nullable = right;
      bool? right1 = new bool?(nullable.GetValueOrDefault() ? true : !this.flipTexture);
      int num = down ? 1 : 0;
      hook.Use(right1, down: num != 0);
      if (!this.hasActed)
        this.hasActed = true;
    }

    public void Unhook() => this.hook.Use(retract: true);

    private void HookBind() => this.Hook(down: Input.IsDown(KeyBind.Shift));

    public void Kill(bool invokedByButton = false)
    {
      int num;
      if (!VarManager.IsOverclocked)
      {
        if (invokedByButton)
        {
          Simulation currentSimulation = this.CurrentSimulation;
          num = currentSimulation != null ? (currentSimulation.IsMain ? 1 : 0) : 0;
        }
        else
          num = 1;
      }
      else
        num = 0;
      if (num != 0)
      {
        Explosion explosion = new Explosion(this.CurrentSimulation.IsIntro ? SimulationManager.MainSimulation : this.CurrentSimulation, this.Center, new Microsoft.Xna.Framework.Rectangle?(this.Rectangle), this.animator.GetCurrentSpritePos());
      }
      this.SetActive(false);
      this.CurrentSimulation.OnPlayerDeath(invokedByButton);
      if (!this.manualControlsEnabled)
        return;
      this.UnRegisterInput();
    }

    public override void Destroy()
    {
      this.UnRegisterInput();
      Simulation currentSimulation = this.CurrentSimulation;
      Type[] typeArray = new Type[1]{ typeof (TextObject) };
      foreach (TextObject textObject in currentSimulation.GetAllByType(typeArray))
      {
        if (textObject.AttachedToID == this.Id)
          textObject.Destroy();
      }
      base.Destroy();
    }

    private void CreateAnimations()
    {
      Texture2D texture = TextureManager.GetTexture("tileset");
      this.idleAnimation = new Animation(true, new Vector2?(), new AnimationFrame[6]
      {
        new AnimationFrame(0.2f, texture, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(1, 258, 13, 14))),
        new AnimationFrame(0.2f, texture, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(17, 258, 13, 14))),
        new AnimationFrame(0.2f, texture, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(33, 258, 13, 14))),
        new AnimationFrame(0.2f, texture, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(49, 258, 13, 14))),
        new AnimationFrame(0.2f, texture, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(33, 258, 13, 14))),
        new AnimationFrame(0.2f, texture, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(17, 258, 13, 14)))
      });
      this.jumpAnimation = new Animation(false, new Vector2?(), new AnimationFrame[3]
      {
        new AnimationFrame(0.1f, texture, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(1, 273, 13, 14))),
        new AnimationFrame(0.1f, texture, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(17, 272, 13, 14))),
        new AnimationFrame(0.2f, texture, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(33, 274, 13, 14)))
      });
      this.walkAnimation = new Animation(true, new Vector2?(), new AnimationFrame[8]
      {
        new AnimationFrame(0.06f, texture, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(1, 273, 13, 14))),
        new AnimationFrame(0.06f, texture, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(17, 272, 13, 14))),
        new AnimationFrame(0.06f, texture, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(33, 274, 13, 14))),
        new AnimationFrame(0.06f, texture, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(49, 274, 13, 14))),
        new AnimationFrame(0.06f, texture, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(66, 273, 13, 14))),
        new AnimationFrame(0.06f, texture, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(82, 272, 13, 14))),
        new AnimationFrame(0.06f, texture, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(97, 274, 13, 14))),
        new AnimationFrame(0.06f, texture, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(113, 274, 13, 14)))
      });
      this.waveAnimation = new Animation(false, new Vector2?(new Vector2(-1.5f, -1f)), new AnimationFrame[17]
      {
        new AnimationFrame(0.06f, texture, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 448, 16, 16))),
        new AnimationFrame(0.03f, texture, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(16, 448, 16, 16))),
        new AnimationFrame(0.015f, texture, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(32, 448, 16, 16))),
        new AnimationFrame(0.03f, texture, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(48, 448, 16, 16)), (AnimationFrame.FrameFinishHandler) (g => playWaveEffect(g))),
        new AnimationFrame(0.0899999961f, texture, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(64, 448, 16, 16))),
        new AnimationFrame(0.03f, texture, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(48, 448, 16, 16))),
        new AnimationFrame(0.015f, texture, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(32, 448, 16, 16))),
        new AnimationFrame(0.03f, texture, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(16, 448, 16, 16))),
        new AnimationFrame(0.03f, texture, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 448, 16, 16))),
        new AnimationFrame(0.03f, texture, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(16, 448, 16, 16))),
        new AnimationFrame(0.015f, texture, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(32, 448, 16, 16))),
        new AnimationFrame(0.03f, texture, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(48, 448, 16, 16)), (AnimationFrame.FrameFinishHandler) (g => playWaveEffect(g, 0.1f))),
        new AnimationFrame(0.0899999961f, texture, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(64, 448, 16, 16))),
        new AnimationFrame(0.03f, texture, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(48, 448, 16, 16))),
        new AnimationFrame(0.015f, texture, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(32, 448, 16, 16))),
        new AnimationFrame(0.03f, texture, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(16, 448, 16, 16))),
        new AnimationFrame(0.06f, texture, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 448, 16, 16)))
      });

      static void playWaveEffect(GameObject obj, float pitch = 0.0f)
      {
        for (int index = 0; index < 8; ++index)
        {
          Player.waveParticleConfig.startPos = obj.Center;
          Particle particle = new Particle(obj.CurrentSimulation, Player.waveParticleConfig);
        }
        SoundManager.Play("wave", 0.5f, pitch);
      }
    }

    private void UpdateAnimation()
    {
      if (this.IsGrounded)
      {
        if (this.animator.InAnimation(this.jumpAnimation))
          this.SetAnimation(this.IdleAnimation);
        float num1 = Math.Abs(this.Velocity.X);
        float num2 = 50f;
        bool flag = this.animator.InAnimation(this.IdleAnimation);
        if (this.IsGrounded && (double) num1 == 0.0 && !flag)
          this.SetAnimation(this.IdleAnimation);
        else if (((!this.isBeeingMoved ? 0 : ((double) num1 > (double) num2 ? 1 : 0)) & (flag ? 1 : 0)) != 0)
        {
          this.SetAnimation(this.walkAnimation);
        }
        else
        {
          if (this.isBeeingMoved || (double) num1 > (double) num2 + (double) Math.Abs(this.ConveyorSpeed) || flag)
            return;
          this.SetAnimation(this.IdleAnimation);
        }
      }
      else
      {
        if (this.animator.InAnimation(this.jumpAnimation))
          return;
        this.SetAnimation(this.jumpAnimation);
      }
    }

    private void CheckIdle(float deltaTime)
    {
      bool flag1 = this.CurrentSimulation != null && this.CurrentSimulation.IsIntro;
      bool flag2 = StateManager.IsState(GameState.MainMenu);
      if (flag1 && this.IsGrounded && !this.CurrentSimulation.HasStarted)
      {
        this.SetActive(true);
        this.CurrentSimulation.StartASAP();
      }
      if ((flag2 || this.ScriptInterpreter.IsRandom || flag1 && !this.ScriptInterpreter.IsSaying && SimulationManager.MainSimulation.IsFinished) && this.Position == this.lastPos)
        this.idleTime += deltaTime;
      else
        this.idleTime = 0.0f;
      if ((double) this.lifeTime <= 0.30000001192092896)
        return;
      if (VarManager.GetBool("manualControls") && (StateManager.IsState(GameState.InLevel_Default) || StateManager.IsState(GameState.InLevel_FromEditor)))
        this.EnableManualControls();
      else if ((double) this.idleTime > 2.0)
        this.Kill();
      else if (flag2 && (double) this.lifeTime > (VarManager.HasInt("lifetime") ? (double) VarManager.GetInt("lifetime") : 25.0))
        this.Kill();
      else if (!this.ScriptInterpreter.HasInstructed && !this.ScriptInterpreter.IsInstructing)
      {
        if (VarManager.GetBool("randombots"))
          this.ScriptInterpreter.StartRandomInstructions();
        else if (!this.manualControlsEnabled & flag2)
          this.ScriptInterpreter.StartMenuInstructions();
        else if (SimulationManager.HasMainStarted && !this.CurrentSimulation.HasStarted && this.CurrentSimulation.CanBeStarted)
          this.CurrentSimulation.StartASAP();
      }
      else if ((double) this.playTime > (double) this.maxLifeTime)
      {
        this.Kill();
        PopupMenu.Instance.Inform("The maximum lifetime of your*robot has been reached!*Max: " + this.maxLifeTime.ToString() + "s", new PromptMenu.InteractionHandler(((UIElement) PopupMenu.Instance).ToggleActive), false);
      }
    }

    private void RecordStartRight()
    {
      this.RecordWaitTime();
      TextEditor.Instance.AddLine("start right", true);
    }

    private void RecordStopRight()
    {
      this.RecordWaitTime();
      if (Input.IsDown(KeyBind.GoLeft))
        TextEditor.Instance.AddLine("start left", true);
      else
        TextEditor.Instance.AddLine("stop right", true);
    }

    private void RecordStartLeft()
    {
      if (Input.IsDown(KeyBind.GoRight))
        return;
      this.RecordWaitTime();
      TextEditor.Instance.AddLine("start left", true);
    }

    private void RecordStopLeft()
    {
      this.RecordWaitTime();
      TextEditor.Instance.AddLine("stop left", true);
    }

    private void RecordWaitTime()
    {
      string s = "wait " + Math.Round((double) this.timeSinceAction, 3).ToString().Replace(',', '.');
      this.timeSinceAction = 0.0f;
      TextEditor.Instance.AddLine(s, true);
    }

    private float GetSizeMultiplier() => (float) this.Width / this.DefaultSize.X;

    protected override void OnLanding(float fallTime)
    {
      base.OnLanding(fallTime);
      if (VarManager.IsOverclocked)
        return;
      ColorManager.GetColor("gray");
      float val1 = 3f;
      int num1 = (int) ((double) Math.Min(val1, fallTime) / (double) val1 * 16.0);
      int num2 = VarManager.GetInt("particleamount");
      if (!this.CurrentSimulation.CheckMode && num2 > 0)
      {
        ParticleGroup particleGroup = new ParticleGroup(this.CurrentSimulation, this.Position + new Vector2((float) (this.Width / 2), (float) this.Height), Player.landingParticleConfig, num1 * num2, (float) (num1 / 2));
      }
    }

    private void PlayJumpEffect()
    {
      if (this.CurrentSimulation.CheckMode || VarManager.IsOverclocked || (double) this.jumpEffectCooldown > 0.0)
        return;
      this.jumpEffectCooldown = 0.05f;
      SoundManager.Play("failedjump", 0.6f);
      Microsoft.Xna.Framework.Color color1 = ColorManager.GetColor("orange");
      Microsoft.Xna.Framework.Color color2 = this.IsGrounded ? ColorManager.GetColor("gray") : Microsoft.Xna.Framework.Color.LightGray;
      ParticleConfig config = this.IsGrounded ? Player.jumpParticleConfig : Player.failedJumpParticleConfig;
      config.trailRandomColorRange = config.randomColorRange;
      int random = Utility.GetRandom(this.IsGrounded ? 2 : 0);
      int from = this.Width / 8;
      for (int index = 0; index < (this.IsGrounded ? 2 : 1) * VarManager.GetInt("particleamount"); ++index)
      {
        config.color = index < random ? color1 : color2;
        config.trailColor = config.color;
        config.startPos = this.Position + new Vector2(Utility.Lerp((float) from, (float) (this.Width - from), Utility.GetRandom(0.0f, 1f)), (float) this.Height + config.startDir.Y * (float) Utility.GetRandom(5));
        Particle particle = new Particle(this.CurrentSimulation, config);
      }
    }
  }
}
