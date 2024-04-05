#version 330 core
out vec4 FragColor;

struct PointLight {
    vec3 position;

    vec3 specular;
    vec3 diffuse;
    vec3 ambient;

    float constant;
    float linear;
    float quadratic;
};

struct Material {
    sampler2D texture_diffuse1;
    sampler2D texture_specular1;

    float shininess;
};

in VS_OUT {
    vec3 FragPos;
    vec3 Normal;
    vec2 TexCoords;
} fs_in;


uniform Material material;

uniform int num_of_lights;
uniform PointLight pointLights[10];
uniform vec3 viewPosition;

uniform samplerCube depthMaps[10];
//uniform samplerCubeArray depthMaps;
uniform float far_plane;
uniform bool shadows;

vec3 gridSamplingDisk[20] = vec3[]
(
   vec3(1, 1,  1), vec3( 1, -1,  1), vec3(-1, -1,  1), vec3(-1, 1,  1),
   vec3(1, 1, -1), vec3( 1, -1, -1), vec3(-1, -1, -1), vec3(-1, 1, -1),
   vec3(1, 1,  0), vec3( 1, -1,  0), vec3(-1, -1,  0), vec3(-1, 1,  0),
   vec3(1, 0,  1), vec3(-1,  0,  1), vec3( 1,  0, -1), vec3(-1, 0, -1),
   vec3(0, 1,  1), vec3( 0, -1,  1), vec3( 0, -1, -1), vec3( 0, 1, -1)
);


float ShadowCalculation(vec3 fragPos, int light_index)
{
    vec3 fragToLight = fragPos - pointLights[light_index].position;
    //float closestDepth = texture(depthMap, fragToLight).r;
    //closestDepth *= far_plane;
    float currentDepth = length(fragToLight);
    float shadow = 0.0;
    float bias = 0.25;
    int samples = 20;
    float viewDistance = length(viewPosition - fragPos);
    float diskRadius = (1.0 + (viewDistance / far_plane)) / 25.0;
    for(int i = 0; i < samples; ++i)
    {
        //float closestDepth = texture(depthMaps, vec4(fragToLight + gridSamplingDisk[i] * diskRadius, 0)).r;
        //float closestDepth = texture(depthMaps[0], fragToLight + gridSamplingDisk[i] * diskRadius).r;
        //FragColor = vec4(texture(depthMaps[1], fragToLight).r, vec3(0.0));
        float closestDepth = 0.0;
        if(light_index == 0)
        {
            closestDepth = texture(depthMaps[0], fragToLight + gridSamplingDisk[i] * diskRadius).r;
        }
        else if(light_index == 1)
        {
            closestDepth = texture(depthMaps[1], fragToLight + gridSamplingDisk[i] * diskRadius).r;
        }
        else if(light_index == 2)
        {
            closestDepth = texture(depthMaps[1], fragToLight + gridSamplingDisk[i] * diskRadius).r;
        }
        else if(light_index == 3)
        {
            closestDepth = texture(depthMaps[1], fragToLight + gridSamplingDisk[i] * diskRadius).r;
        }
        else if(light_index == 4)
        {
            closestDepth = texture(depthMaps[1], fragToLight + gridSamplingDisk[i] * diskRadius).r;
        }
        else if(light_index == 5)
        {
            closestDepth = texture(depthMaps[1], fragToLight + gridSamplingDisk[i] * diskRadius).r;
        }
        else
        {
        }

        closestDepth *= far_plane;
        if(currentDepth - bias > closestDepth)
        {
            shadow += 1.0;
        }
    }

   // FragColor = vec4(vec3(closestDepth / far_plane), 1.0);
    //FragColor = vec4(vec3(texture(depthMaps[1], fragToLight).r), 1.0);
    shadow /= float(samples);
    return shadow;
}

// calculates the color when using a point light.
vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir, int light_index)
{
    vec3 color = texture(material.texture_diffuse1, fs_in.TexCoords).rgb;
    vec3 lightDir = normalize(light.position - fs_in.FragPos);
    // diffuse shading
    float diff = max(dot(lightDir, normal), 0.0);
    // phong specular shading
    //vec3 reflectDir = reflect(-lightDir, normal);
    //float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    // bling-phong specular shading
    vec3 halfwayDir = normalize(lightDir + viewDir);
    float spec = pow(max(dot(normal, halfwayDir), 0.0), 32.0);

    // attenuation
    float distance = length(light.position - fragPos);
    float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));
    // combine results
    vec3 ambient = light.ambient * color;
    vec3 diffuse = light.diffuse * diff * color;
    vec3 specular = light.specular * spec * vec3(texture(material.texture_specular1, fs_in.TexCoords).xxx);
    ambient *= attenuation;
    diffuse *= attenuation;
    specular *= attenuation;
    float shadow = ShadowCalculation(fs_in.FragPos, light_index);
    return (ambient + ((1 - shadow) * (diffuse + specular)));
   // return (ambient + diffuse + specular);
   //return color;
}

void main()
{
    vec3 normal = normalize(fs_in.Normal);
    vec3 viewDir = normalize(viewPosition - fs_in.FragPos);
    vec3 result = vec3(0.0);
    //result += CalcPointLight(pointLights[0], normal, fs_in.FragPos, viewDir, 0);
    for(int i = 0; i < num_of_lights; i++)
    {
        if(i == 0)
        {
            result += CalcPointLight(pointLights[0], normal, fs_in.FragPos, viewDir, 0);
        }
        if(i == 1)
        {
            result += CalcPointLight(pointLights[1], normal, fs_in.FragPos, viewDir, 1);
        }
        else
        {
        }
    }
    FragColor = vec4(result, 1.0);
}