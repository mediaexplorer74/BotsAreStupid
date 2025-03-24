// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.NoisePath
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

#nullable disable
namespace BotsAreStupid
{
  internal class NoisePath
  {
    private List<Vector2> path;
    private float amplitude = 180f;
    private int waveLength = 70;
    private Random r;
    private const long M = 4294967296;
    private const long A = 1664525;
    private const int C = 1;
    private float Z;
    private Vector2 start;
    private Vector2 end;
    private Vector2 xAxis;
    private Vector2 yAxis;
    private float totalLength;

    public Vector2 Last => this.path[this.path.Count - 1];

    public Vector2 First => this.path[0];

    public NoisePath(Vector2 start, Vector2 end, float scale = 1f)
    {
      this.start = start;
      this.end = end;
      this.amplitude *= scale;
      this.waveLength = (int) ((double) this.waveLength * (double) scale);
      this.r = new Random();
      this.amplitude = (float) (this.r.Next(120) + 130);
      while (this.path == null || (double) (this.path[this.path.Count - 1] - end).Length() > 20.0)
        this.GetPath();
      this.totalLength = 0.0f;
      Vector2 vector2 = this.path[0];
      for (int index = 1; index < this.path.Count; ++index)
      {
        this.totalLength += (this.path[index] - vector2).Length();
        vector2 = this.path[index];
      }
    }

    public Vector2 GetPoint(float distance, out Vector2 currentDir)
    {
      float num1 = this.totalLength * distance;
      float num2 = 0.0f;
      Vector2 vector2 = this.path[0];
      for (int index = 1; index < this.path.Count; ++index)
      {
        currentDir = this.path[index] - vector2;
        if ((double) num2 + (double) currentDir.Length() > (double) num1)
        {
          float num3 = num1 - num2;
          currentDir.Normalize();
          return vector2 + currentDir * num3;
        }
        num2 += currentDir.Length();
        vector2 = this.path[index];
      }
      throw new Exception("Failed to get Path point");
    }

    private Vector2 GetXAxis()
    {
      Vector2 xaxis = this.end - this.start;
      xaxis.Normalize();
      return xaxis;
    }

    private Vector2 GetYAxis()
    {
      return this.r.NextDouble() < 0.5 ? new Vector2(this.xAxis.Y, -this.xAxis.X) : new Vector2(-this.xAxis.Y, this.xAxis.X);
    }

    private Vector2 TransformPoint(Vector2 point)
    {
      return this.start + this.xAxis * point.X + this.yAxis * point.Y;
    }

    private float NextRan()
    {
      this.Z = (float) Math.Floor(this.r.NextDouble() * 4294967296.0);
      this.Z = (float) ((1664525.0 * (double) this.Z + 1.0) % 4294967296.0);
      return this.Z / (float) uint.MaxValue;
    }

    private void GetPath()
    {
      this.path = new List<Vector2>();
      float num1 = (this.end - this.start).Length();
      this.xAxis = this.GetXAxis();
      this.yAxis = this.GetYAxis();
      float pa = this.NextRan();
      float pb = this.NextRan();
      int num2 = 10;
      float num3 = 1f;
      float num4 = 0.0f;
      float x = 0.0f;
      bool flag = false;
      while ((double) x < (double) num1)
      {
        float y;
        if ((double) num4 % (double) this.waveLength == 0.0)
        {
          pa = pb;
          pb = this.NextRan();
          y = pa * this.amplitude;
        }
        else
          y = Utility.Interpolate(pa, pb, num4 % (float) this.waveLength / (float) this.waveLength) * this.amplitude;
        if (flag)
        {
          this.path.Add(this.TransformPoint(new Vector2(x, y)));
          x += (float) num2;
        }
        else if ((double) y < (double) num3)
          flag = true;
        num4 += (float) num2;
      }
    }
  }
}
