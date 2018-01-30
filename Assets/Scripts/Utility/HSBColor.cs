using System;
using UnityEngine;

namespace Utility
{
    /// <summary>
    /// Utility class for HSB colors.
    /// </summary>
    [Serializable]
    public struct HsbColor
    {
        public float H;
        public float S;
        public float B;
        public float A;

        private const float Tolerance = float.Epsilon * 64;

        public HsbColor(float h, float s, float b, float a)
        {
            H = h;
            S = s;
            B = b;
            A = a;
        }

        public HsbColor(float h, float s, float b)
        {
            H = h;
            S = s;
            B = b;
            A = 1f;
        }

        public HsbColor(Color col)
        {
            var temp = FromColor(col);
            H = temp.H;
            S = temp.S;
            B = temp.B;
            A = temp.A;
        }

        public static HsbColor FromColor(Color color)
        {
            var ret = new HsbColor(0f, 0f, 0f, color.a);

            var r = color.r;
            var g = color.g;
            var b = color.b;

            var max = Mathf.Max(r, Mathf.Max(g, b));

            if (max <= 0)
            {
                return ret;
            }

            var min = Mathf.Min(r, Mathf.Min(g, b));
            var dif = max - min;

            if (max > min)
            {
                if (Math.Abs(g - max) < Tolerance)
                {
                    ret.H = (b - r) / dif * 60f + 120f;
                }
                else if (Math.Abs(b - max) < Tolerance)
                {
                    ret.H = (r - g) / dif * 60f + 240f;
                }
                else if (b > g)
                {
                    ret.H = (g - b) / dif * 60f + 360f;
                }
                else
                {
                    ret.H = (g - b) / dif * 60f;
                }
                if (ret.H < 0)
                {
                    ret.H = ret.H + 360f;
                }
            }
            else
            {
                ret.H = 0;
            }

            ret.H *= 1f / 360f;
            ret.S = dif / max * 1f;
            ret.B = max;

            return ret;
        }

        public static Color ToColor(HsbColor hsbColor)
        {
            var r = hsbColor.B;
            var g = hsbColor.B;
            var b = hsbColor.B;

            if (Math.Abs(hsbColor.S) < Tolerance) return new Color(Mathf.Clamp01(r), Mathf.Clamp01(g), Mathf.Clamp01(b), hsbColor.A);

            var max = hsbColor.B;
            var dif = hsbColor.B * hsbColor.S;
            var min = hsbColor.B - dif;

            var h = hsbColor.H * 360f;

            if (h < 60f)
            {
                r = max;
                g = h * dif / 60f + min;
                b = min;
            }
            else if (h < 120f)
            {
                r = -(h - 120f) * dif / 60f + min;
                g = max;
                b = min;
            }
            else if (h < 180f)
            {
                r = min;
                g = max;
                b = (h - 120f) * dif / 60f + min;
            }
            else if (h < 240f)
            {
                r = min;
                g = -(h - 240f) * dif / 60f + min;
                b = max;
            }
            else if (h < 300f)
            {
                r = (h - 240f) * dif / 60f + min;
                g = min;
                b = max;
            }
            else if (h <= 360f)
            {
                r = max;
                g = min;
                b = -(h - 360f) * dif / 60 + min;
            }
            else
            {
                r = 0;
                g = 0;
                b = 0;
            }

            return new Color(Mathf.Clamp01(r), Mathf.Clamp01(g), Mathf.Clamp01(b), hsbColor.A);
        }

        public Color ToColor()
        {
            return ToColor(this);
        }

        public override string ToString()
        {
            return "H:" + H + " S:" + S + " B:" + B;
        }

        public static HsbColor Lerp(HsbColor a, HsbColor b, float t)
        {
            float h, s;

            //check special case black (color.b==0): interpolate neither hue nor saturation!
            //check special case grey (color.s==0): don't interpolate hue!
            if (Math.Abs(a.B) < Tolerance)
            {
                h = b.H;
                s = b.S;
            }
            else if (Math.Abs(b.B) < Tolerance)
            {
                h = a.H;
                s = a.S;
            }
            else
            {
                if (Math.Abs(a.S) < Tolerance)
                {
                    h = b.H;
                }
                else if (Math.Abs(b.S) < Tolerance)
                {
                    h = a.H;
                }
                else
                {
                    // works around bug with LerpAngle
                    var angle = Mathf.LerpAngle(a.H * 360f, b.H * 360f, t);
                    while (angle < 0f)
                        angle += 360f;
                    while (angle > 360f)
                        angle -= 360f;
                    h = angle / 360f;
                }

                s = Mathf.Lerp(a.S, b.S, t);
            }

            return new HsbColor(h, s, Mathf.Lerp(a.B, b.B, t), Mathf.Lerp(a.A, b.A, t));
        }
    }
}