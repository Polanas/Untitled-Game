#version 430 core
in vec2 TexCoords;
layout (location=0) out vec4 color;

uniform sampler2D text;
uniform vec3 textColor;
uniform float alpha;

void main()
{    
    vec2 texCoords = TexCoords;
   // texCoords.y = 1 - texCoords.y;
    vec4 sampled = vec4(1.0, 1.0, 1.0, texture(text, texCoords).r);
    color = vec4(textColor, alpha) * sampled;

   // color = vec4(1,0,0,1);
    //gl_FragDepth = 0;
}  