using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game;

class TestMaterial : Material
{

//    [UniformInfo("time", UniformType.Float)]
  //  public float time;

    [Uniform("testImage", UniformType.Texture)]
    private Texture _testTexture;

    public TestMaterial(Sprite sprite) : base(ResourceManager.GetShader("Mtest"), sprite)
    {
        _testTexture = ResourceManager.GetTexture("iceMaterial");
    }
}
