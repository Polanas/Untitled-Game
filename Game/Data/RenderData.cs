using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game;

class RenderData
{

    public Texture lightTexture;

    public Texture maskTexture;

    public Texture distortedShadowsTexture;

    public Texture shadowCastersTexture;

    public EcsSystems renderSystems;

    public readonly Graphics graphics;

    public readonly Dictionary<Layer, Matrix4> cameraLayerProjections;

    public readonly Dictionary<Layer, Matrix4> layerProjections;

    public readonly DebugDrawer debugDrawer;

    public readonly Dictionary<string, Layer> layers;

    public readonly List<Layer> layersList;

    public RenderData(DebugDrawer debugDrawer, Graphics graphics)
    {
        this.debugDrawer = debugDrawer;
        this.graphics = graphics;

        layers = new();
        layersList = new();
        cameraLayerProjections = new();
        layerProjections = new();
    }
}
