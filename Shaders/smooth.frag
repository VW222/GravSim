// Fragment shader for smoothing
#version 330

in vec2 fragTexCoord;
in vec4 fragColor;

// Input uniform values
uniform sampler2D texture0;
uniform vec4 colDiffuse;

// Output fragment color
out vec4 finalColor;

void main() {
//    vec4 color = texture2D(texture0, fragTexCoord);
//    // Simple smoothing effect
//    vec4 smoothedColor = texture2D(texture0, fragTexCoord + vec2(-0.002, -0.002));
//    smoothedColor += texture2D(texture0, fragTexCoord + vec2(-0.002, 0.002));
//    smoothedColor += texture2D(texture0, fragTexCoord + vec2(0.002, -0.002));
//    smoothedColor += texture2D(texture0, fragTexCoord + vec2(0.002, 0.002));
//    smoothedColor /= 4.0;
//    finalColor = vec4(texture2D(texture0, fragTexCoord).xyz, 1.0);
    vec4 texelColor = texture(texture0, fragTexCoord)*colDiffuse*fragColor;
    finalColor = texelColor;
}