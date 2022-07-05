using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game;

class RenderData
{

    public Matrix4 UIProjection => _UIProjection;

    public Texture lightTexture;

    public Texture maskTexture;

    public Texture distortedShadowsTexture;

    public Texture shadowCastersTexture;

    public readonly DrawUtils drawUtils;

    public readonly Dictionary<Layer, Matrix4> cameraLayerProjections;

    public readonly Dictionary<Layer, Matrix4> layerProjections;

    public readonly DebugDrawer debugDrawer;

    public readonly Dictionary<string, Layer> layers;

    public readonly List<Layer> layersList;

    private Matrix4 _UIProjection;

    public RenderData(Matrix4 UIProjection, DebugDrawer debugDrawer, DrawUtils drawUtils)
    {
        _UIProjection = UIProjection;
        this.debugDrawer = debugDrawer;
        this.drawUtils = drawUtils;

        layers = new();
        layersList = new();
        cameraLayerProjections = new();
        layerProjections = new();
    }
}
