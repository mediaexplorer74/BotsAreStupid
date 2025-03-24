// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.SpawnPipe
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;

#nullable disable
namespace BotsAreStupid
{
  internal class SpawnPipe : GameObject
  {
    private static Microsoft.Xna.Framework.Color hitboxColor = ColorManager.GetColor("red");
    private const int playerWidth = 26;
    private const int playerHeight = 28;
    private const int defaultWidth = 50;
    private const int defaultHeight = 80;
    private const float spawnCooldownMax = 0.1f;
    private bool producingPlayer;
    private bool spawnWhenPossible;
    private float spawnCooldown = 0.0f;

    protected override bool ShowHitboxEnabled => VarManager.GetBool("showhitboxes_background");

    protected override Microsoft.Xna.Framework.Color HitboxColor => SpawnPipe.hitboxColor;

    public int PlayerSize { private set; get; }

    public SpawnPipe(
      Simulation simulation,
      float x,
      float y,
      int? playerSize = null,
      int width = 50,
      int height = 80)
      : base(simulation, x, y, width, height, new Microsoft.Xna.Framework.Color?(Microsoft.Xna.Framework.Color.White), TextureManager.GetTexture("tileset"), new Microsoft.Xna.Framework.Rectangle?(TextureManager.GetSpritePos("spawnpipe")))
    {
      this.layerDepth = LayerDepths.SpawnPipe_Default;
      this.isRotatable = false;
      this.PlayerSize = playerSize ?? 5;
      this.defaultSize = new Vector2?(new Vector2(50f, 80f));
      this.availableParameters.Add(new ObjectParameter()
      {
        name = "Player Size:",
        min = 1,
        max = 7,
        propertyName = nameof (PlayerSize),
        gameObject = (GameObject) this
      });
    }

    public void SpawnPlayerASAP()
    {
      if ((double) this.spawnCooldown <= 0.0)
        this.SpawnPlayer();
      else
        this.spawnWhenPossible = true;
    }

    private void SpawnPlayer()
    {
      this.CurrentSimulation.Player?.Destroy();
      float num = (float) (0.5 + (double) this.PlayerSize / 10.0);
      int width = (int) (26.0 * (double) num);
      int height = (int) (28.0 * (double) num);
      Player player = new Player(this.CurrentSimulation, this.X + (float) (this.Width / 2) - (float) (width / 2), this.Y + (float) height, width, height, Microsoft.Xna.Framework.Color.White, TextureManager.GetTexture("tileset"), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(1, 258, 13, 14)));
      this.layerDepth = LayerDepths.SpawnPipe_Default;
      this.producingPlayer = true;
      this.spawnWhenPossible = false;
      this.spawnCooldown = 0.1f;
    }

    public override GameObject Copy(Simulation intoSimulation = null)
    {
      return (GameObject) new SpawnPipe(intoSimulation ?? this.CurrentSimulation, this.X, this.Y, new int?(this.PlayerSize), this.Width, this.Height);
    }

    public override void Update(float deltaTime)
    {
      base.Update(deltaTime);
      if (this.spawnWhenPossible)
      {
        if ((double) this.spawnCooldown <= 0.0)
          this.SpawnPlayer();
        else
          this.spawnCooldown -= deltaTime;
      }
      else
      {
        if (!this.producingPlayer || this.CurrentSimulation.Player.Rectangle.Intersects(this.Rectangle))
          return;
        this.producingPlayer = false;
        this.layerDepth = LayerDepths.SpawnPipe_Producing;
      }
    }

    public override void SetActive(bool active)
    {
      base.SetActive(active);
      if (!active || this.CurrentSimulation.Player != null)
        return;
      this.SpawnPlayerASAP();
    }
  }
}
