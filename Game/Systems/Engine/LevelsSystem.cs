using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ldtk;
using Leopotam.EcsLite.Di;

namespace Game;

class LevelsSystem : MySystem
{

    private EcsPoolInject<Transform> _transforms;

    private EcsPoolInject<Renderable> _renderables;

    public override void Init(EcsSystems systems)
    {
        base.Init(systems);

        LoadLevels();
        SetLevel(sharedData.gameData.levels["level1"]);
    }

    private void SetLevel(Level level)
    {
        ClearEntities();

        foreach (var layer in level.LdrkLevel.Sublevels[0].LayerInstances)
        {
            switch (layer.Type)
            {
                case "IntGrid":

                    foreach (var tile in layer.AutoLayerTiles)
                    {
                        var renderable = new Renderable();
                        var sprite = new Sprite(Path.GetFileNameWithoutExtension(layer.TilesetRelPath), new Vector2i(8));
                        sprite.Frame = (int)tile.T;

                        renderable.sprite = sprite;
                        sprite.layer = sharedData.renderData.layers["background1"];
                        sprite.position = new Vector2(tile.Px[0], tile.Px[1]);
                        sprite.depth = 5;
                        sprite.flippedHorizontally = tile.F == 1 || tile.F == 3;
                        sprite.flippedVertically = tile.F == 2 || tile.F == 3;
                        sprite.isShadowCaster = true;

                        var entity = world.AddEntity();

                        ref var tr = ref _transforms.Value.Add(entity);
                        ref var rend = ref _renderables.Value.Add(entity);

                        tr = new Transform(sprite.position, 0, sprite.size);
                        rend = renderable;

                        sharedData.physicsData.physicsFactory.AddSaticBody(tr);
                    }
                    break;
                case "Entities":

                    foreach (var ldtkEntity in layer.EntityInstances)
                    {
                        var sprite = new Sprite(Path.GetFileNameWithoutExtension(ldtkEntity.Tags[0]));
                        var renderable = new Renderable();

                        renderable.sprite = sprite;
                        sprite.position = new Vector2(ldtkEntity.Px[0] + sprite.TexWidth / 2 - 4, ldtkEntity.Px[1] + sprite.TexHeight / 2 - 4);
                        sprite.layer = sharedData.renderData.layers["background1"];
                        sprite.depth = 6;
                        sprite.isShadowCaster = true;

                        var entity = world.AddEntity();

                        ref var tr = ref _transforms.Value.Add(entity);
                        ref var rend = ref _renderables.Value.Add(entity);

                        tr = new Transform(sprite.position, 0, sprite.size);
                        rend = renderable;
                    }
                    break;
                case "Tiles":


                    break;

                default:

                    continue;
            }

            if (layer.Type != "IntGrid") //TODO: Add other layers support
                continue;
        }
    }

    private void ClearEntities()
    {
        var filter = world.Filter<Transform>().End();

        foreach (var entity in filter)
            world.DelEntity(entity);
    }

    private void LoadLevels()
    {
        DirectoryInfo directoryInfo = new(@"Content\Levels");

        foreach (var file in directoryInfo.GetFiles())
        {
            if (file.Extension != ".json")
                continue;

            var ldtkJson = LdtkJson.FromJson(File.ReadAllText(file.FullName));
            var level = new Level(ldtkJson);

            sharedData.gameData.levelsList.Add(level);
            sharedData.gameData.levels[Path.GetFileNameWithoutExtension(file.FullName)] = level;
        }
    }
}
