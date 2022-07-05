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

    public static void AddComponents<T>(this EcsWorld world, int entity, params T[] components) where T : struct
    {
        for (int i = 0; i < components.Length; i++)
            world.GetPool<T>().Add(entity);
    }

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

    public static EcsSystems AddGroup(this EcsSystems systems, string groupName, bool defaultState, string eventWorldName, MyGameWindow game, params MySystem[] nestedSystems)
    {
        game.AddGroupSystems(groupName, nestedSystems);

        for (int i = 0; i < nestedSystems.Length; i++)
            nestedSystems[i].GroupState = defaultState;

        return systems.Add(new EcsGroupSystem(groupName, defaultState, eventWorldName, nestedSystems));
    }
}