using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leopotam.EcsLite.ExtendedSystems;
using Box2DX.Common;
using Box2DX.Dynamics;

namespace Game;

static class Extensions
{

    public static int AddEntity(this EcsWorld world) => world.NewEntity();

    public static ref T AddComponent<T>(this EcsWorld world, int entity) where T : struct =>
        ref world.GetPool<T>().Add(entity);

    public static void AddComponent<T>(this EcsWorld world, int entity, T component) where T : struct
    {
        ref var component1 = ref world.GetPool<T>().Add(entity);
        component1 = component;
    }

    public static ref T GetComponent<T>(this EcsWorld world, int entity) where T : struct =>
       ref world.GetPool<T>().Get(entity);

    public static bool HasComponent<T>(this EcsWorld world, int entity) where T : struct =>
        world.GetPool<T>().Has(entity);

    public static void RemoveComponent<T>(this EcsWorld world, int entity) where T : struct =>
       world.GetPool<T>().Del(entity);

    public static List<int> GetAllEntitiesWithTag(this EcsWorld world, string tag)
    {
        var pool = world.GetPool<Tag>();
        Tag tagS;
        List<int> entities = new();

        foreach (var e in world.Filter<Tag>().End())
        {
            tagS = pool.Get(e);

            if (tagS.tag == tag)
                entities.Add(e);
        }

        return entities;
    }

    public static int GetEntityWithTag(this EcsWorld world, string tag)
    {
        var pool = world.GetPool<Tag>();
        Tag tagS;

        foreach (var e in world.Filter<Tag>().End())
        {
            tagS = pool.Get(e);

            if (tagS.tag == tag)
                return e;
        }

        return -1;
    }

    public static int GetEntitiyWithComponent<T>(this EcsWorld world) where T : struct
    {
        foreach (var e in world.Filter<T>().End()) 
            return e;

        return -1;
    }
        
    

    public static EcsSystems AddGroup(this EcsSystems systems, string groupName, bool defaultState, string eventWorldName, MyGameWindow game, params MySystem[] nestedSystems)
    {
        game.AddGroupSystems(groupName, nestedSystems);

        for (int i = 0; i < nestedSystems.Length; i++)
            nestedSystems[i].GroupState = defaultState;

        return systems.Add(new EcsGroupSystem(groupName, defaultState, eventWorldName, nestedSystems));
    }

    public static T Find<T>(this EcsSystems systems) where T : IEcsSystem
    {
        IEcsSystem[] systemsArray = null;
        systems.GetAllSystems(ref systemsArray);

        for (int i = 0; i < systemsArray.Length; i++)
        {
            if (systemsArray[i] is T)
                return (T)systemsArray[i];  //a bit cheasy but WHO CARES
        }

        return default(T);
    }

    public static void SetPosition(this Body body, Vector2 position) => body.SetPosition(new Vec2(position.X*1/8f, position.Y*1/8f));

    public static Vector2 GetPixelatedPosition(this Body body)
    {
        Vec2 pos = body.GetPosition();
        return new Vector2(pos.X * 8f, pos.Y * 8f);
    }


    public static Vector2 LerpDist(this Vector2 a, Vector2 b, float amount)
    {
        float dist = Vector2.Distance(a, b);
        float test = dist == 0 ? 1 : amount / dist;
        return Vector2.Lerp(a, b, test);
    }

    public static Vector2 LerpDG(this Vector2 a, Vector2 b, float amount)
    {
        Vector2 c3norm = b - a;
        if (c3norm.Length < 0.0001f)
        {
            return a;
        }
        c3norm.Normalize();
        Vector2 ret = a + c3norm * amount;
        if (b.X > a.X && ret.X > b.X)
        {
            ret.X = b.X;
        }
        if (b.X < a.X && ret.X < b.X)
        {
            ret.X = b.X;
        }
        if (b.Y > a.Y && ret.Y > b.Y)
        {
            ret.Y = b.Y;
        }
        if (b.Y < a.Y && ret.Y < b.Y)
        {
            ret.Y = b.Y;
        }
        return ret;
    }

    public static EcsPackedEntity RepackEntity(this EcsWorld world, ref EcsPackedEntity packed)
    {
        packed.Gen = world.GetEntityGen(packed.Id);
        return packed;
    }

}