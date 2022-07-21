using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game;

class Camera
{
    public readonly List<Matrix4> layerProjections = new(); 

    public Vector2 Size => (Vector2)MyGameWindow.ScreenSize / zoom;

    //public Vector2 TopLeft => _transform.position - Size / 2;

    public Vector2 position;

    public Vector2 offset;

    public float zoom;
}