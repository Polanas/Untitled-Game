#version 430 core

layout(std430, binding = 0) buffer texCoordsBuffer
{
    vec2[10000] texCoords; 
};

layout (location = 1) in vec4 color;
layout (location = 2) in vec4 model1;
layout (location = 3) in vec2 model2;
layout (location = 4) in float depth;
//layout (location = 5) in int isShadowCaster;

out vec2 fTexCoords;
out vec4 fColor;
out float fDepth;
flat out int fIsShadowCaster;

uniform bool pixelated;

const vec2[] vertices = vec2[](
    vec2(0.0, 0.0), 
    vec2(1.0, 0.0),
    vec2(1.0, 1.0), 
    vec2(0.0, 1.0)
);

void main()
{
      mat4 projection = mat4(
    model1.x, model1.y, 0, model1.z,
    model1.w, model2.x, 0, model2.y,
    0,0,0,0,
    0,0,0,1
    );

    fTexCoords = texCoords[gl_InstanceID * 4 + gl_VertexID];
    fColor = color;
    fDepth = depth;
   // fIsShadowCaster = isShadowCaster;

    gl_Position = vec4(vertices[gl_VertexID], 0, 1.0) * projection;
}