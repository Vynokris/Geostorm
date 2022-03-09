using System.Numerics;
using static System.Math;

namespace MyMathLib
{
    // ---------- Arithmetic ---------- //

    public static class Arithmetic
    {
        // Rounds the given value to the nearest int.
        public static int RoundInt(float val)   { return (int)Round(val);  }

        // Rounds down the given value.
        public static int FloorInt(float val)   { return (int)Floor(val);  }

        // Rounds up the given value.
        public static int CeilInt(float val)    { return (int)Ceiling(val); }

        // Returns the sqare power of the given value.
        public static float SqPow(float val)    { return val * val;  }

        // Returns 1 if the given value is positive or null, and -1 if it is negative.
        public static int SignOf(float val)     { if (val == 0) return 1; return (int)val / Abs((int)val); }

        // Converts the given angle from degrees to radians.
        public static float Deg2Rad(float val)  { return val * ((float)PI / 180f); }

        // Converts the given angle from radians to degrees.
        public static float RadToDeg(float rad) { return rad * (180f / (float)PI); }

        // Clamps the given value to be inferior or equal to the maximum value.
        public static float ClampUnder(float val, float max)        { if (val > max) val = max; return val; }

        // Clamps the given value to be superior or equal to the minimum value.
        public static float ClampAbove(float val, float min)        { if (val < min) val = min; return val; }

        // Compute linear interpolation between start and end for the parameter val (if 0 <= val <= 1: start <= return <= end).
        public static float Lerp(float val, float start, float end) { return start + val* (end - start); }

        // Compute the linear interpolation factor that returns val when lerping between start and end.
        public static float GetLerp(float val, float start, float end)
        {
            if (end - start != 0) return (val - start) / (end - start);
            return 0;
        }

        // Linear interpolation between two given colors.
        public static Color ColorLerp(float val, Color start, Color end)
        {
            return new Color(start.R + val * (end.R - start.R),
                             start.G + val * (end.G - start.G),
                             start.B + val * (end.B - start.B),
                             start.A + val * (end.A - start.A));
        }

        // Remaps the given value from one range to another.
        public static float Remap(float val, float inputStart, float inputEnd, float outputStart, float outputEnd)
        {
            return outputStart + (val - inputStart) * (outputEnd - outputStart) / (inputEnd - inputStart);
        }

        // Returns true if the given number is a power of 2.
        public static bool IsPowerOf2(int val)      { return val == (int)Pow(2, (int)(Log(val) / Log(2))); }

        // Returns the closest power of 2 that is inferior or equal to val.
        public static int GetPowerOf2Under(int val) {  return (int)Pow(2, (int)(Log(val) / Log(2))); }

        // Returns the closest power of 2 that is superior or equal to val.
        public static int GetPowerOf2Above(int val)
        {
            if (IsPowerOf2(val)) return (int)Pow(2, (int)(Log(val) / Log(2)));
            else                 return (int)Pow(2, (int)(Log(val) / Log(2)) + 1);
        }

        // Blend between two HSV colors.
        public static HSV BlendHSV(HSV color0, HSV color1)
        {
            Vector2 totalVec = Geometry2D.Vector2FromAngle(color0.H, 1)
                             + Geometry2D.Vector2FromAngle(color1.H, 1);

            float avgHue = totalVec.GetAngle();
            float avgSat = (color0.S + color1.S) / 2;
            float avgVal = (color0.V + color1.V) / 2;

            return new HSV(avgHue, avgSat, avgVal);
        }
    }
}
