using System;
using System.Collections.Generic;
using UnityEngine;

public class FieldObjectPooller
{
    private Dictionary<System.Type, Queue<FieldObject>> poolDictionary = new Dictionary<System.Type, Queue<FieldObject>>();
    private FieldObjectFactory _factory;
    private Transform _parentTransform;

    public FieldObjectPooller(FieldObjectFactory factory, Transform parentTransform)
    {
        _factory = factory;
        _parentTransform = parentTransform;
    }

    public FieldObject GetObjectOfType(Type objectType, int indexI, int indexJ)
    {
        Queue<FieldObject> queueToPoollFrom;

        if (!poolDictionary.ContainsKey(objectType))
        {
            poolDictionary.Add(objectType, new Queue<FieldObject>());
        }

        queueToPoollFrom = poolDictionary[objectType];

        FieldObject objectToPool;

        if (queueToPoollFrom.Count == 0)
        {
            return _factory.GetObjectOfType(objectType, indexI, indexJ, _parentTransform, this);
        }

        objectToPool = queueToPoollFrom.Dequeue();

        objectToPool.PrefabInstance.SetActive(true);
        objectToPool.ChangePosition(indexI, indexJ);

        return objectToPool;
    }

    public FieldObject GetRandomToken(int indexI, int indexJ)
    {
        Type randomTokenType = FieldObjectsPrefabsSO.GetRandomTokenType();

        return GetObjectOfType(randomTokenType, indexI, indexJ);
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