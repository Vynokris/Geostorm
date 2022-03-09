using static System.Math;

namespace MyMathLib
{
    // ---------- Colors ---------- //

    public class HSV
    {
        public float H { get; set; }
        public float S { get; set; }
        public float V { get; set; }

        public HSV() { H = 0; S = 0; V = 0; }
        public HSV(float h, float s, float v) { H = h; S = s; V = v; }

        // Convert an HSV color to RGB.
        public Color ToRGB(float alpha)
        {
            Color color = new Color( 0, 0, 0, alpha );
            float k = 0, t = 0;

            // Red channel
            k = (H + 5) % 6; t = 4 - k;
            k = (t < k) ? t : k;
            k = (k < 1) ? k : 1;
            k = (k > 0) ? k : 0;
            color.R = V - V * S * k;

            // Green channel
            k = (H + 3) % 6;  t = 4 - k;
            k = (t < k) ? t : k;
            k = (k < 1) ? k : 1;
            k = (k > 0) ? k : 0;
            color.G = V - V * S * k;

            // Blue channel
            k = (H + 1) % 6;  t = 4 - k;
            k = (t < k) ? t : k;
            k = (k < 1) ? k : 1;
            k = (k > 0) ? k : 0;
            color.B = V - V * S * k;

            return color;
        }
    }

    public class Color
    {
        public float R { get; set; }
        public float G { get; set; }
        public float B { get; set; }
        public float A { get; set; }

        public Color()                                   { R = 0; G = 0; B = 0; A = 1; }
        public Color(float r, float g, float b, float a) { R = r; G = g; B = b; A = a; }

        // Convert an RGB color (0 <= rgba <= 1) to HSV.
        public HSV ToHSV()
        {
            HSV hsv = new HSV();

            float minV = Min(Min(R, G), B);
            float maxV = Max(Max(R, G), B);
            float diff = maxV - minV;

            float r = R, g = G, b = B;

            // Set Value.
            hsv.V = maxV;

            // If max and min are the same, return.
            if (diff< 0.00001f) return new HSV(0, 0, hsv.V);

            // Set Saturation.
            if (maxV > 0) hsv.S = diff / maxV;
            else          return new HSV(0, 0, hsv.V);

            // Set Hue.
            if      (r >= maxV) hsv.H = (g - b) / diff;
            else if (g >= maxV) hsv.H = 2.0f + (b - r) / diff;
            else if (b >= maxV) hsv.H = 4.0f + (r - g) / diff;

            // Keep Hue above 0.
            if (hsv.H < 0) hsv.H += 2 * (float)PI;

            return hsv;
        }

        // Shifts the hue of the given color.
        public Color Shift(float hue)
        {
            HSV hsv = this.ToHSV();
            hsv.H += hue;
            if (hsv.H >= 2 * (float)PI) hsv.H -= 2 * (float)PI;
            else if (hsv.H < 0)         hsv.H += 2 * (float)PI;
            Color result = hsv.ToRGB(A);

            return result;
        }
    }
}
