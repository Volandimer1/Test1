using UnityEngine;

public abstract class FieldObject : IConstruct
{
    public indexes Indexes;
    public GameObject PrefabInstance;
    public bool Moovable { get; protected set; }

    private Vector2 StartingPosition, TargetPosition;

    public FieldObject()
    {
        Indexes = new();
        PrefabInstance = new();
        Moovable = false;
    }

    public FieldObject(GameObject gameObject, int indexI, int indexJ)
    {
        Constructor(gameObject, indexI, indexJ);
    }

    public virtual void Constructor(GameObject gameObject, int indexI, int indexJ)
    {
        PrefabInstance = gameObject;
        ChangePosition(indexI, indexJ);
    }

    public void ChangePosition(int indexI, int indexJ)
    {
        Indexes.i = indexI;
        Indexes.j = indexJ;
        PrefabInstance.transform.localPosition = IndexesToLocalPosition(Indexes);
    }

    public void SetTarget(Vector2 targetPosition)
    {
        if (Moovable == false) return;

        StartingPosition = PrefabInstance.transform.localPosition;
        TargetPosition = targetPosition;
    }

    public void SetTarget(indexes indexes)
    {
        if (Moovable == false) return;

        StartingPosition = PrefabInstance.transform.localPosition;
        TargetPosition = IndexesToLocalPosition(indexes);
    }

    public void MoveToInterpolated(float interpolateValue)
    {
        if (Moovable == false) return;

        PrefabInstance.transform.localPosition =
            Vector2.Lerp(StartingPosition, TargetPosition, interpolateValue);
    }

    public static Vector2 IndexesToLocalPosition(int indexI, int indexJ)
    {
        return new Vector2(-640 + (indexJ * 320), 2720 - (indexI * 320));
    }

    public static Vector2 IndexesToLocalPosition(indexes indexes)
    {
        return new Vector2(-640 + (indexes.j * 320), 2720 - (indexes.i * 320));
    }

    public static indexes LocalPositionToIndexes(Vector2 position)
    {
        indexes result = new indexes();

        result.j = (int)((position.x + 640 + 160) / 320);
        result.i = (int)((position.y - 2720 - 160) / (-320));

        return result;
    }

    public abstract void Reset();
}