using System;
using System.Collections.Generic;
using UnityEngine;

public class FieldObjectFactory
{
    private FieldObjectsPrefabsSO _fieldObjectsPrefabs;
    private GoalsManager _goalsManager;
    private Field _field;

    private Dictionary<Type, Func<FieldObject>> typeFactories = new Dictionary<Type, Func<FieldObject>>
    {
        { typeof(BlueToken), () => new BlueToken() },
        { typeof(GreenToken), () => new GreenToken() },
        { typeof(OrangeToken), () => new OrangeToken() },
        { typeof(RedToken), () => new RedToken() },
        { typeof(YelowToken), () => new YelowToken() },
        { typeof(ObstacleIce), () => new ObstacleIce() },
        { typeof(ObstacleRock), () => new ObstacleRock() },
        { typeof(BonusBomb), () => new BonusBomb() },
        { typeof(BonusSideRocket), () => new BonusSideRocket() }
    };

    public FieldObjectFactory(FieldObjectsPrefabsSO fieldObjectsPrefabs, GoalsManager goalsManager, Field field)
    {
        _fieldObjectsPrefabs = fieldObjectsPrefabs;
        _goalsManager = goalsManager;
        _field = field;
    }

    public T Get<T>(int indexI, int indexJ, Transform parentTransform, FieldObjectPooller objectPoller) where T : FieldObject, new()
    {
        if (!_fieldObjectsPrefabs.PrefabsDictionary.ContainsKey(typeof(T)))
        {
            Debug.LogError($"FieldObjectsPrefabs Scriptable Object's dictionary doesn't contains {typeof(T)} type key. Returning NULL from Get method.");
            return null;
        }

        GameObject prefabInstance = UnityEngine.Object.Instantiate(_fieldObjectsPrefabs.PrefabsDictionary[typeof(T)], parentTransform);

        T instanceOfT = new T();
        instanceOfT.Constructor(prefabInstance, indexI, indexJ, objectPoller, _goalsManager, _field);

        return instanceOfT;
    }

    public FieldObject GetObjectOfType(System.Type fieldObjectType, int indexI, int indexJ, Transform parentTransform, FieldObjectPooller objectPoller)
    {
        if (!_fieldObjectsPrefabs.PrefabsDictionary.ContainsKey(fieldObjectType))
        {
            Debug.LogError($"FieldObjectsPrefabs Scriptable Object's dictionary doesn't contains {fieldObjectType} type key. Returning NULL from Get method.");
            return null;
        }

        GameObject prefabInstance = GameObject.Instantiate(_fieldObjectsPrefabs.PrefabsDictionary[fieldObjectType], parentTransform);

        FieldObject instanceOfT = typeFactories[fieldObjectType].Invoke();
        instanceOfT.Constructor(prefabInstance, indexI, indexJ, objectPoller, _goalsManager, _field);

        return instanceOfT;
    }

    public FieldObject GetRandomToken(int indexI, int indexJ, Transform parentTransform, FieldObjectPooller objectPoller)
    {
        System.Type randomTokenType = FieldObjectsPrefabsSO.GetRandomTokenType();
        GameObject prefabInstance = UnityEngine.Object.Instantiate(_fieldObjectsPrefabs.PrefabsDictionary[randomTokenType], parentTransform);

        FieldObject instance = typeFactories[randomTokenType].Invoke();
        instance.Constructor(prefabInstance, indexI, indexJ, objectPoller, _goalsManager, _field);

        return instance;
    }
}