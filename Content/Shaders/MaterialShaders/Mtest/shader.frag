#version 430 core

layout (location = 0) out vec4 color;

in vec2 fTexCoords;

uniform sampler2D image;
uniform vec2 framesAmount;
uniform sampler2D testImage;

float circle(vec2 p, float r) 
{
	return length(p) - r;
}

void main() 
{
   vec2 uv = fTexCoords;
   vec2 frameUV = uv;

   frameUV.x = mod(frameUV.x, 1./framesAmount.x) * framesAmount.x;
   frameUV.y = mod(frameUV.y, 1./framesAmount.y) * framesAmount.y;
	
//	vec3 colRainbow = 0.5 + 0.5*cos(time+frameUV.xyx+vec3(0,2,4));

	color = texture(image, uv) * vec4(texture(testImage, uv).xyz, 1);// * vec4(colRainbow, 1);
}