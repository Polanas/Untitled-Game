using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leopotam.EcsLite.ExtendedSystems;

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
}