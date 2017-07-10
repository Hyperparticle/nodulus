using System;
using UnityEngine;

namespace Assets.Scripts.Utility
{
    [System.Serializable]
    public struct HsbColor
    {
        public float h;
        public float s;
        public float b;
        public float a;

        public HsbColor(float h, float s, float b, float a)
        {
            this.h = h;
            this.s = s;
            this.b = b;
            this.a = a;
        }

        public HsbColor(float h, float s, float b)
        {
            this.h = h;
            this.s = s;
            this.b = b;
            this.a = 1f;
        }

        public HsbColor(Color col)
        {
            var temp = FromColor(col);
            h = temp.h;
            s = temp.s;
            b = temp.b;
            a = temp.a;
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
                if (g == max)
                {
                    ret.h = (b - r) / dif * 60f + 120f;
                }
                else if (b == max)
                {
                    ret.h = (r - g) / dif * 60f + 240f;
                }
                else if (b > g)
                {
                    ret.h = (g - b) / dif * 60f + 360f;
                }
                else
                {
                    ret.h = (g - b) / dif * 60f;
                }
                if (ret.h < 0)
                {
                    ret.h = ret.h + 360f;
                }
            }
            else
            {
                ret.h = 0;
            }

            ret.h *= 1f / 360f;
            ret.s = (dif / max) * 1f;
            ret.b = max;

            return ret;
        }

        public static Color ToColor(HsbColor hsbColor)
        {
            var r = hsbColor.b;
            var g = hsbColor.b;
            var b = hsbColor.b;

            if (hsbColor.s == 0) return new Color(Mathf.Clamp01(r), Mathf.Clamp01(g), Mathf.Clamp01(b), hsbColor.a);

            var max = hsbColor.b;
            var dif = hsbColor.b * hsbColor.s;
            var min = hsbColor.b - dif;

            var h = hsbColor.h * 360f;

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

            return new Color(Mathf.Clamp01(r), Mathf.Clamp01(g), Mathf.Clamp01(b), hsbColor.a);
        }

        public Color ToColor()
        {
            return ToColor(this);
        }

        public override string ToString()
        {
            return "H:" + h + " S:" + s + " B:" + b;
        }

        private const float Tolerance = float.Epsilon * 10;
        public static HsbColor Lerp(HsbColor a, HsbColor b, float t)
        {
            float h, s;

            //check special case black (color.b==0): interpolate neither hue nor saturation!
            //check special case grey (color.s==0): don't interpolate hue!
            if (Math.Abs(a.b) < Tolerance)
            {
                h = b.h;
                s = b.s;
            }
            else if (Math.Abs(b.b) < Tolerance)
            {
                h = a.h;
                s = a.s;
            }
            else
            {
                if (Math.Abs(a.s) < Tolerance)
                {
                    h = b.h;
                }
                else if (Math.Abs(b.s) < Tolerance)
                {
                    h = a.h;
                }
                else
                {
                    // works around bug with LerpAngle
                    var angle = Mathf.LerpAngle(a.h * 360f, b.h * 360f, t);
                    while (angle < 0f)
                        angle += 360f;
                    while (angle > 360f)
                        angle -= 360f;
                    h = angle / 360f;
                }

                s = Mathf.Lerp(a.s, b.s, t);
            }

            return new HsbColor(h, s, Mathf.Lerp(a.b, b.b, t), Mathf.Lerp(a.a, b.a, t));
        }
    }
}