#version 330 core
in vec4 FragPos;

uniform vec3 lightPos;
uniform float far_plane;

float LinearizeDepth(float depth)
{
    float near = 1.0;
    float far = far_plane;
    float z = depth * 2.0 - 1.0; // back to NDC
    return (2.0 * near * far) / (far + near - z * (far - near));
}

void main()
{
    float lightDistance = length(FragPos.xyz - lightPos);

    lightDistance = lightDistance / far_plane;

    gl_FragDepth = lightDistance;
}