#version 330
in vec3 position;
in vec3 norm;
in vec4 color;
out vec4 outputColor;
void main()
{
 vec3 lightPosition = vec3(0,0,50);
 vec3 lightVector = normalize(lightPosition - position);
 float brightness = dot(lightVector, norm);
 outputColor = color * brightness * 2.0f;
}