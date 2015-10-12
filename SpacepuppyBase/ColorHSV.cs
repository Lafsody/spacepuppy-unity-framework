﻿using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using com.spacepuppy.Utils;

namespace com.spacepuppy
{

    /// <summary>
    /// Represents a colour in HSV colour space. 'Hue' is stored as a percentage, multiply by 360 to get the angle.
    /// </summary>
    public struct ColorHSV : System.IEquatable<ColorHSV>
    {

        #region Fields

        public float h;
        public float s;
        public float v;
        public float a;

        #endregion

        #region CONSTRUCTOR

        public ColorHSV(float h, float s, float v)
        {
            this.h = Mathf.Clamp01(h);
            this.s = Mathf.Clamp01(s);
            this.v = Mathf.Clamp01(v);
            this.a = 1f;
        }

        public ColorHSV(float h, float s, float v, float a)
        {
            this.h = Mathf.Clamp01(h);
            this.s = Mathf.Clamp01(s);
            this.v = Mathf.Clamp01(v);
            this.a = Mathf.Clamp01(a);
        }

        public ColorHSV(Color c)
        {
            float r = Mathf.Clamp01(c.r);
            float g = Mathf.Clamp01(c.g);
            float b = Mathf.Clamp01(c.b);
            var max = Mathf.Max(r, Mathf.Max(g, b));
            var min = Mathf.Min(r, Mathf.Min(g, b));
            var delta = max - min;

            if (Mathf.Abs(delta) < 0.0001f)
            {
                this.h = 0f;
            }
            if (r >= g && r >= b)
            {
                this.h = (((g - b) / delta) % 6f) / 6f;
            }
            else if (g >= b)
            {
                this.h = ((b - r) / delta + 2f) / 6f;
            }
            else
            {
                this.h = ((r - g) / delta + 4f) / 6f;
            }
            
            this.s = (max > 0f) ? delta / max : 0f;
            this.v = max;
            this.a = Mathf.Clamp01(c.a);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Pulls in each field so that it's in a logical range. Hue is wrapped around 0->1 (as it's an angle) where as 
        /// all other values are clamped to 0->1.
        /// </summary>
        public void Normalize()
        {
            this.h = this.h % 1f;
            if (this.h < 0f) this.h += 1f;
            this.s = Mathf.Clamp01(this.s);
            this.v = Mathf.Clamp01(this.v);
            this.a = Mathf.Clamp01(this.a);
        }

        public override string ToString()
        {
            return string.Format("H:{0:0.000},S:{1:0.000},V:{2:0.000},A:{3:0.000}", this.h, this.s, this.v, this.a);
        }

        #endregion

        #region IEquatable Interface

        public override bool Equals(object obj)
        {
            if (obj is ColorHSV)
            {
                return this == (ColorHSV)obj;
            }
            else if (obj is Color)
            {
                return this == (ColorHSV)((Color)obj);
            }
            else if (obj is Vector4)
            {
                return this == (ColorHSV)((Vector4)obj);
            }
            else
            {
                return false;
            }
        }

        public bool Equals(ColorHSV b)
        {
            return this == b;
        }

        public override int GetHashCode()
        {
            return this.h.GetHashCode() ^ this.s.GetHashCode() << 2 ^ this.v.GetHashCode() >> 2 ^ this.a.GetHashCode() >> 1;
        }

        #endregion

        #region Conversion

        public static implicit operator ColorHSV(Color c)
        {
            return new ColorHSV(c);
        }

        public static implicit operator Color(ColorHSV hsv)
        {
            return ToColor(hsv);
        }

        public static implicit operator ColorHSV(Vector4 c)
        {
            return new ColorHSV(c.x, c.y, c.z, c.w);
        }

        public static implicit operator Vector4(ColorHSV hsv)
        {
            return new Vector4(hsv.h, hsv.s, hsv.v, hsv.a);
        }

        #endregion

        #region Operators

        public static ColorHSV operator +(ColorHSV a, ColorHSV b)
        {
            var r = new ColorHSV();
            r.h = (a.h + b.h) % 1f;
            r.s = Mathf.Clamp01(a.s + b.s);
            r.v = Mathf.Clamp01(a.v + b.v);
            r.a = Mathf.Clamp01(a.a + b.a);
            return r;
        }

        public static ColorHSV operator -(ColorHSV a, ColorHSV b)
        {
            var r = new ColorHSV();
            r.h = a.h - b.h;
            if (r.h < 0f) r.h += 1f;
            r.s = Mathf.Clamp01(a.s - b.s);
            r.v = Mathf.Clamp01(a.v - b.v);
            r.a = Mathf.Clamp01(a.a - b.a);
            return r;
        }

        public static ColorHSV operator *(ColorHSV a, ColorHSV b)
        {
            var r = new ColorHSV();
            r.h = a.h * b.h;
            r.s = a.s * b.s;
            r.v = a.v * b.v;
            r.a = a.a * b.a;
            return r;
        }

        public static ColorHSV operator *(ColorHSV a, float b)
        {
            var r = new ColorHSV();
            r.h = (a.h * b) % 1f;
            if (r.h < 0f) r.h += 1f;
            r.s = Mathf.Clamp01(a.s * b);
            r.v = Mathf.Clamp01(a.v * b);
            r.a = Mathf.Clamp01(a.a * b);
            return r;
        }

        public static ColorHSV operator /(ColorHSV a, float b)
        {
            if (float.IsNaN(b)) return new ColorHSV();

            var r = new ColorHSV();
            r.h = (a.h / b) % 1f;
            if (r.h < 0f) r.h += 1f;
            r.s = Mathf.Clamp01(a.s / b);
            r.v = Mathf.Clamp01(a.v / b);
            r.a = Mathf.Clamp01(a.a / b);
            return r;
        }

        public static bool operator ==(ColorHSV a, ColorHSV b)
        {
            return a.h == b.h && a.s == b.s && a.v == b.v && a.a == b.a;
        }

        public static bool operator !=(ColorHSV a, ColorHSV b)
        {
            return a.h != b.h || a.s != b.s || a.v != b.v || a.a != b.a;
        }

        #endregion

        #region Static Utils

        public static Color ToColor(ColorHSV hsv)
        {
            hsv.Normalize();

            var hprime = (hsv.h % 1f) * 360f / 60f;
            if (hprime < 0f) hprime += 60f;
            var chroma = hsv.v * hsv.s;
            var x = chroma * (1f - Mathf.Abs(hprime % 2f - 1f));
            float r = 0f;
            float g = 0f;
            float b = 0f;

            switch (Mathf.FloorToInt(hprime))
            {
                case 0:
                    {
                        r = chroma;
                        g = x;
                    }
                    break;
                case 1:
                    {
                        r = x;
                        g = chroma;
                    }
                    break;
                case 2:
                    {
                        g = chroma;
                        b = x;
                    }
                    break;
                case 3:
                    {
                        g = x;
                        b = chroma;
                    }
                    break;
                case 4:
                    {
                        r = x;
                        b = chroma;
                    }
                    break;
                case 5:
                    {
                        r = chroma;
                        b = x;
                    }
                    break;
            }

            var m = hsv.v - chroma;
            return new Color(r + m, g + m, b + m, hsv.a);
        }

        public ColorHSV Lerp(ColorHSV start, ColorHSV end, float t)
        {
            ColorHSV r = new ColorHSV();

            if (start.v <= 0f)
            {
                r.h = end.h;
                r.s = end.s;
            }
            else if (end.v <= 0f)
            {
                r.h = start.h;
                r.s = start.s;
            }
            else
            {
                if (start.s <= 0f)
                {
                    r.h = end.h;
                }
                else if (end.s <= 0f)
                {
                    r.h = start.h;
                }
                else
                {
                    var delta = (end.h - start.h) % 1f;
                    r.h = start.h + delta * (t % 1f);
                    if (r.h < 0f)
                        r.h += 1f;
                }
                r.s = Mathf.Clamp01(start.s + (end.s - start.s) * t);
            }

            r.v = Mathf.Clamp01(start.v + (end.v - start.v) * t);
            r.a = Mathf.Clamp01(start.a + (end.a - start.a) * t);

            return r;
        }

        #endregion

    }
    
}
