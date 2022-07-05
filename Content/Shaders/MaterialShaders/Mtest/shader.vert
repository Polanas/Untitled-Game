#version 430 core

layout(std430, binding = 1) buffer texCoordsBuffer
{
    vec2[4] texCoords; 
};

out vec2 fTexCoords;
out vec2 fFramesAmount;

const vec2[] vertices = vec2[](
    vec2(0.0, 0.0), 
    vec2(1.0, 0.0),
    vec2(1.0, 1.0), 
    vec2(0.0, 1.0)
);

void main()
{
    fTexCoords = texCoords[gl_VertexID];
    gl_Position = vec4(vertices[gl_VertexID], 0, 1.0);
}