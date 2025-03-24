// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.SoundManager
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace BotsAreStupid
{
  internal class SoundManager
  {
    private Dictionary<string, SoundEffect> sounds = new Dictionary<string, SoundEffect>();
    private Dictionary<string, float> soundCooldowns = new Dictionary<string, float>();
    private const float musicVolume = 0.07f;
    private const float defaultSoundCooldown = 0.05f;
    private SoundEffectInstance music;

    public static SoundManager Instance { get; private set; }

    public SoundManager(ContentManager Content)
    {
      SoundManager.Instance = this;
      this.sounds.Add("boostpickup", Content.Load<SoundEffect>("Audio/boostpickup"));
      this.sounds.Add("chargeportal", Content.Load<SoundEffect>("Audio/chargeportal"));
      this.sounds.Add("click", Content.Load<SoundEffect>("Audio/click"));
      this.sounds.Add("lelegsound", Content.Load<SoundEffect>("Audio/lelegsound"));
      this.sounds.Add(nameof (music), Content.Load<SoundEffect>("Audio/music"));
      this.sounds.Add("orbpickup", Content.Load<SoundEffect>("Audio/orbpickup"));
      this.sounds.Add("say", Content.Load<SoundEffect>("Audio/say"));
      this.sounds.Add("success", Content.Load<SoundEffect>("Audio/success"));
      this.sounds.Add("bouncer", Content.Load<SoundEffect>("Audio/bouncer"));
      this.sounds.Add("wave", Content.Load<SoundEffect>("Audio/wave"));
      this.sounds.Add("jump", Content.Load<SoundEffect>("Audio/Movement/jump"));
      this.sounds.Add("failedjump", Content.Load<SoundEffect>("Audio/Movement/failedjump"));
      this.sounds.Add("footstep", Content.Load<SoundEffect>("Audio/Movement/footstep"));
      this.sounds.Add("landing", Content.Load<SoundEffect>("Audio/Movement/landing"));
      this.sounds.Add("hookattach", Content.Load<SoundEffect>("Audio/Movement/hookattach"));
      this.sounds.Add("hookdetach", Content.Load<SoundEffect>("Audio/Movement/hookdetach"));
      for (int index = 0; index < 10; ++index)
        this.sounds.Add("explosion" + index.ToString(), Content.Load<SoundEffect>("Audio/Explosions/explosion" + index.ToString()));
    }

    public static void Play(string name, float volume = 1f, float pitch = 0.0f)
    {
      if (SoundManager.Instance == null || SoundManager.Instance.soundCooldowns.ContainsKey(name))
        return;
      SoundManager.Instance.PlaySound(SoundManager.GetSound(name), pitch, new float?(volume));
      SoundManager.Instance.soundCooldowns.Add(name, 0.05f);
    }

    public static void PlayRandom(string name)
    {
      if (SoundManager.Instance == null)
        return;
      SoundManager.Instance.PlayRandomSound(name);
    }

    public static void UpdateMusicVolume()
    {
      SoundManager.Instance.music.Volume = (float) (0.070000000298023224 * ((double) VarManager.GetInt("mastervolume") / 100.0) * ((double) VarManager.GetInt("musicvolume") / 100.0));
    }

    public void StartMusic()
    {
      this.music = SoundManager.GetSound("music").CreateInstance();
      SoundManager.UpdateMusicVolume();
      this.music.IsLooped = true;
      this.music.Play();
    }

    public void Update(float deltaTime)
    {
      foreach (string key in Enumerable.ToList<string>((IEnumerable<string>) this.soundCooldowns.Keys))
      {
        this.soundCooldowns[key] -= deltaTime;
        if ((double) this.soundCooldowns[key] <= 0.0)
          this.soundCooldowns.Remove(key);
      }
    }

    public static void SetVolume(string name, int value, bool save = false)
    {
      VarManager.SetInt(name + "volume", value);
      SoundManager.UpdateMusicVolume();
      if (!save)
        return;
      VarManager.SaveOptions();
    }

    private static SoundEffect GetSound(string name)
    {
      SoundEffect sound;
      SoundManager.Instance.sounds.TryGetValue(name.ToLower(), out sound);
      return sound;
    }

        private void PlaySound(SoundEffect sound, float pitch = 0.0f, float? volume = null)
        {
            float volume1 = ((float)volume.GetValueOrDefault(1.0f))
                * MathHelper.Clamp((float)VarManager.GetInt("mastervolume") / 100f, 0.0f, 1f) 
                * MathHelper.Clamp((float)VarManager.GetInt("sfxvolume") / 100f, 0.0f, 1f);
            sound?.Play(volume1, pitch, 0.0f);
        }

    private void PlayRandomSound(string name, int amount = 9)
    {
      SoundEffect sound = SoundManager.GetSound(name + Utility.GetRandom(amount).ToString());
      if (sound == null)
        return;
      this.PlaySound(sound, Utility.RandomizeNumber(0.0f, 0.05f));
    }
  }
}
