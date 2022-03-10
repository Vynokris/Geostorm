#version 100

precision mediump float;

// Input vertex attributes (from vertex shader).
varying vec2 fragTexCoord;
varying vec4 fragColor;

// Input uniform values.
uniform sampler2D texture0;
uniform vec4      colDiffuse;

// Variables.
const vec2 screenSize = vec2(1920, 1080);
const vec2 pixelSize  = vec2(1) / screenSize;
const int  blurRadius = 1;

void main()
{
    vec4  sum       = vec4(0);
    float coeffSum  = 0.0;
    
    // Do a wheighted average of the pixels surrounding the fragPixel.
    for (int x = -blurRadius; x <= blurRadius; x++) {
        for (int y = -blurRadius; y <= blurRadius; y++)
        {
            sum      += float(abs(x) + abs(y) + 1) * texture2D(texture0, fragTexCoord + vec2(x, y) * pixelSize);
            coeffSum += float(abs(x) + abs(y) + 1);
        }
    }
    sum /= coeffSum;

    // Calculate final fragment color.
    gl_FragColor = vec4(sum.rgb, 1.0);
}