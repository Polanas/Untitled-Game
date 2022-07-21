#version 430 core

#define Pi 3.14159265359

layout (location = 0) out vec4 fragColor;

in vec2 fTexCoords;

uniform sampler2D image;

uniform vec2 armEndPos;
uniform vec2 armStartPos;

float circle(vec2 p, float r)
{
    return length (p) - r;
}

float sdSegment(in vec2 p, in vec2 a, in vec2 b )
{
    vec2 pa = p-a, ba = b-a;
  float h = clamp( dot(pa,ba)/dot(ba,ba), 0.0, 1.0 );
  return length( pa - ba*h );
}

float sdBox( in vec2 p, in vec2 b )
{
    vec2 d = abs(p)-b;
    return length(max(d,0.0)) + min(max(d.x,d.y),0.0);
}

bool lineSegment(vec2 p, vec2 a, vec2 b, float thickness)
{
    vec2 pa = p - a;
    vec2 ba = b - a;
    float len = length(ba);

    vec2 dir = ba / len;
    float t = dot(pa, dir);
    vec2 n = vec2(-dir.y, dir.x);
    float factor = max(abs(n.x), abs(n.y));
    float distThreshold = (thickness - 1.0 + factor)*0.5;
    float proj = dot(n, pa);

    return (t > 0.0) && (t < len) && (proj <= distThreshold) && (proj > -distThreshold);
}

void main(void)
{
    vec2 uv = gl_FragCoord.xy;

    vec2 pos1 = armStartPos;
    vec2 pos2 = armEndPos;

  bool d2 = lineSegment(
      uv, pos1, pos2, 1.);
  bool d2Left = lineSegment(
      uv+vec2(1,0), pos1, pos2, 1.);
  bool d2Right = lineSegment(
      uv+vec2(-1,0), pos1, pos2, 1.);
   bool d2Up = lineSegment(
      uv+vec2(0,1), pos1, pos2, 1.);
   bool d2Down = lineSegment(
      uv+vec2(0,-1), pos1, pos2, 1.);

    vec4 col = d2 ? vec4(vec3(97,127,248)/255, 1) : vec4(0);
    col = d2Left && !d2 ? vec4(0,0,0,1) : col;
    col = d2Right     && !d2 ? vec4(0,0,0,1) : col;
    col = d2Up     && !d2 ? vec4(0,0,0,1) : col;
    col = d2Down     && !d2 ? vec4(0,0,0,1) : col;

    fragColor = col;
}
