using System;
using System.Collections.Generic;
using UnityEngine;

public class FieldObjectFactory
{
    private FieldObjectsPrefabsSO _fieldObjectsPrefabs;

    private Dictionary<Type, Func<FieldObject>> typeFactories = new Dictionary<Type, Func<FieldObject>>
    {
        { typeof(BlueToken), () => new BlueToken() },
        { typeof(GreenToken), () => new GreenToken() },
        { typeof(OrangeToken), () => new OrangeToken() },
        { typeof(RedToken), () => new RedToken() },
        { typeof(YelowToken), () => new YelowToken() },
        { null, () => null },
    };

    public FieldObjectFactory(FieldObjectsPrefabsSO fieldObjectsPrefabs)
    {
        _fieldObjectsPrefabs = fieldObjectsPrefabs;
    }

    public T Get<T>(int indexI, int indexJ, Transform parentTransform) where T : FieldObject, new()
    {
        if (!_fieldObjectsPrefabs.PrefabsDictionary.ContainsKey(typeof(T)))
        {
            Debug.LogError($"FieldObjectsPrefabs Scriptable Object's dictionary doesn't contains {typeof(T)} type key. Returning NULL from Get method.");
            return null;
        }

        GameObject prefabInstance = UnityEngine.Object.Instantiate(_fieldObjectsPrefabs.PrefabsDictionary[typeof(T)], parentTransform);

        T instanceOfT = new T();
        instanceOfT.Constructor(prefabInstance, indexI, indexJ);

        return instanceOfT;
    }

    public FieldObject GetObjectOfType(System.Type fieldObjectType, int indexI, int indexJ, Transform parentTransform)
    {
        if (!_fieldObjectsPrefabs.PrefabsDictionary.ContainsKey(fieldObjectType))
        {
            Debug.LogError($"FieldObjectsPrefabs Scriptable Object's dictionary doesn't contains {fieldObjectType} type key. Returning NULL from Get method.");
            return null;
        }

        GameObject prefabInstance = UnityEngine.Object.Instantiate(_fieldObjectsPrefabs.PrefabsDictionary[fieldObjectType], parentTransform);

        FieldObject instanceOfT = typeFactories[fieldObjectType].Invoke();
        instanceOfT.Constructor(prefabInstance, indexI, indexJ);

        return instanceOfT;
    }

    public FieldObject GetRandomToken(int indexI, int indexJ, Transform parentTransform)
    {
        System.Type randomTokenType = FieldObjectsPrefabsSO.GetRandomTokenType();
        GameObject prefabInstance = UnityEngine.Object.Instantiate(_fieldObjectsPrefabs.PrefabsDictionary[randomTokenType], parentTransform);

        FieldObject instance = typeFactories[randomTokenType].Invoke();
        instance.Constructor(prefabInstance, indexI, indexJ);

        return instance;
    }
}