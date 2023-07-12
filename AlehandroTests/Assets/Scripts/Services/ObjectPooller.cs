using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooller
{
    private Dictionary<System.Type, object> poolDictionary = new Dictionary<System.Type, object>();
    Dictionary<System.Type, Func<object>> queueFactories = new Dictionary<System.Type, Func<object>>
    {
        { typeof(BlueToken), () => new Queue<BlueToken>() },
        { typeof(GreenToken), () => new Queue<GreenToken>() },
        { typeof(OrangeToken), () => new Queue<OrangeToken>() },
        { typeof(RedToken), () => new Queue<RedToken>() },
        { typeof(YelowToken), () => new Queue<YelowToken>() },
        { null, () => null },
    };
    private FieldObjectFactory _factory;

    public ObjectPooller(FieldObjectFactory factory)
    {
        _factory = factory;
    }

    public T GetObject<T>(int indexI, int indexJ, Transform parentTransform) where T : FieldObject, new()
    {
        Queue<T> queueToPoollFrom;

        if (!poolDictionary.ContainsKey(typeof(T)))
        {
            poolDictionary.Add(typeof(T), new Queue<T>());
        }

        queueToPoollFrom = (Queue<T>)poolDictionary[typeof(T)];

        T objectToPool;

        if (queueToPoollFrom.Count == 0)
        {
           return _factory.Get<T>(indexI, indexJ, parentTransform);
        }

        objectToPool = queueToPoollFrom.Dequeue();

        objectToPool.PrefabInstance.SetActive(true);
        objectToPool.ChangePosition(indexI, indexJ);

        return objectToPool;
    }

    public FieldObject GetRandomToken(int indexI, int indexJ, Transform parentTransform)
    {
        System.Type randomTokenType = FieldObjectsPrefabsSO.GetRandomTokenType();

        Queue<FieldObject> queueToPoollFrom;

        if (!poolDictionary.ContainsKey(randomTokenType))
        {
            poolDictionary.Add(randomTokenType, queueFactories[randomTokenType].Invoke());
        }

        queueToPoollFrom = (Queue<FieldObject>)poolDictionary[randomTokenType];

        FieldObject objectToPool;

        if (queueToPoollFrom.Count == 0)
        {
            return _factory.GetObjectOfType(randomTokenType, indexI, indexJ, parentTransform);
        }

        objectToPool = queueToPoollFrom.Dequeue();

        objectToPool.PrefabInstance.SetActive(true);
        objectToPool.ChangePosition(indexI, indexJ);

        return objectToPool;
    }

    public void ReturnObjectToPool<T>(T objectToReturn) where T :FieldObject
    {
        objectToReturn.Reset();
        objectToReturn.PrefabInstance.SetActive(false);

        Queue<T> queueReturnTo = (Queue<T>)poolDictionary[typeof(T)];

        queueReturnTo.Enqueue(objectToReturn);
    }

    public void ReturnObjectToPool(FieldObject objectToReturn)
    {
        objectToReturn.Reset();
        objectToReturn.PrefabInstance.SetActive(false);

        Type objectType = objectToReturn.GetType();
        Queue<FieldObject> queueReturnTo = (Queue<FieldObject>)poolDictionary[objectType];

        queueReturnTo.Enqueue(objectToReturn);
    }
}