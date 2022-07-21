using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game;

class ArmMaterial : Material
{

    [Uniform("armEndPos", UniformType.Vector2)]
    public Vector2 endPosition;

    [Uniform("armStartPos", UniformType.Vector2)]
    public Vector2 startPosition;

    public ArmMaterial(Sprite sprite) : base(sprite, "arm", @"Materials\arm.frag")
    {

    }
}
