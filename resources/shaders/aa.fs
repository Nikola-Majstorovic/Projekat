#version 330 core

out vec4 FragColor;

in vec2 TexCoords;

uniform sampler2DMS screenTexture;
uniform int SCR_WIDTH, SCR_HEIGHT;

void main()
{
    ivec2 viewportDim = ivec2(SCR_WIDTH, SCR_HEIGHT);
    ivec2 coord = ivec2(viewportDim * TexCoords);
    vec3 sample0 = texelFetch(screenTexture, coord, 0).rgb;
    vec3 sample1 = texelFetch(screenTexture, coord, 1).rgb;
    vec3 sample2 = texelFetch(screenTexture, coord, 2).rgb;
    vec3 sample3 = texelFetch(screenTexture, coord, 3).rgb;

    vec3 col = 0.25 * (sample0 + sample1 + sample2 + sample3);

    FragColor = vec4(sample0, 1.0);
    //FragColor = vec4(1.0 , 0.2, 0.3, 1.0);
}