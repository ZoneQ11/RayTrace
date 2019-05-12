#version 330
in vec3 vPosition;
in vec3 Norm;
in vec3 vColor;
out vec3 position;
out vec3 norm;
out vec4 color;
uniform mat4 M;
void main()
{
 gl_Position = M * vec4( vPosition, 1.0 );
 position = vec3( vPosition );
 norm = vec3( Norm );
 color = vec4( vColor, 1.0 );
}