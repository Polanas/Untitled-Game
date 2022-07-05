#version 430 core

layout (location = 0) out vec4 color;
//layout (location = 1) out vec4 shaderCastersColor;

in vec2 fTexCoords;
in vec4 fColor;
in float fDepth;
//flat in int fIsShadowCaster;

uniform sampler2D image;

//#define Resolution const vec2(1920,1080)

//vec4 myTexture(vec2 uv) 
//{
//    vec2 res = textureSize(image,0);
//    uv = uv*res + 0.5;
//    
//    vec2 fl = floor(uv);
//    vec2 fr = fract(uv);
//    vec2 aa = fwidth(uv)*0.75;
//    fr = smoothstep( vec2(0.5)-aa, vec2(0.5)+aa, fr);
//    
//    uv = (fl+fr-0.5) / res;
//    return texture(image, uv);
//}

void main() 
{
   vec2 uv = fTexCoords;

   color = fColor * texture(image,uv);
  // shaderCastersColor = color.a != 0 && fIsShadowCaster > 0 ? vec4(1) : vec4(0);
  // shaderCastersColor = vec4(fIsShadowCaster, 0,0, 1);
   gl_FragDepth = color.a != 0 ? (100 - fDepth)/100 : 1;
}