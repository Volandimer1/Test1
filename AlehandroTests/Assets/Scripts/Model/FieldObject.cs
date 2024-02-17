using System;
using UnityEngine;

public abstract class FieldObject : IConstruct
{
    public Indexes Indexes;
    public GameObject PrefabInstance;

    protected Field _field;
    protected FieldObjectPooller _objectPoller;
    protected GoalsManager _goalsManager;

    public bool Movable { get; protected set; }

    private Vector2 StartingPosition, TargetPosition;

    public FieldObject()
    {
        Movable = false;
    }

    public FieldObject(GameObject gameObject, int indexI, int indexJ, FieldObjectPooller objectPoller, GoalsManager goalsManager, Field field)
    {
        Constructor(gameObject, indexI, indexJ, objectPoller, goalsManager, field);
    }

    public virtual void Constructor(GameObject gameObject, int indexI, int indexJ, FieldObjectPooller objectPoller, GoalsManager goalsManager, Field field)
    {
        _field = field;
        PrefabInstance = gameObject;
        ChangePosition(indexI, indexJ);

        _objectPoller = objectPoller;
        _goalsManager = goalsManager;
    }

    public virtual void TakeDamage()
    {

    }

    public void ChangePosition(int indexI, int indexJ)
    {
        Indexes.Row = indexI;
        Indexes.Column = indexJ;
        PrefabInstance.transform.localPosition = IndexesToLocalPosition(Indexes);
    }

    public void SetTarget(Vector2 targetPosition)
    {
        if (Movable == false) return;

        StartingPosition = PrefabInstance.transform.localPosition;
        TargetPosition = targetPosition;
    }

    public void SetTarget(Indexes indexes)
    {
        if (Movable == false) return;

        StartingPosition = PrefabInstance.transform.localPosition;
        TargetPosition = IndexesToLocalPosition(indexes);
    }

    public void MoveToInterpolated(float interpolateValue)
    {
        if (Movable == false) return;

        PrefabInstance.transform.localPosition =
            Vector2.Lerp(StartingPosition, TargetPosition, interpolateValue);
    }

    public static Vector2 IndexesToLocalPosition(int indexI, int indexJ)
    {
        return new Vector2(-640 + (indexJ * 320), 2720 - (indexI * 320));
    }

    public static Vector2 IndexesToLocalPosition(Indexes indexes)
    {
        return new Vector2(-640 + (indexes.Column * 320), 2720 - (indexes.Row * 320));
    }

    public static Indexes LocalPositionToIndexes(Vector2 position)
    {
        Indexes result = new Indexes();

        result.Column = (int)Math.Floor((position.x + 640 + 160) / 320.0);
        result.Row = (int)Math.Floor((position.y - 2720 - 160) / -320.0);

        return result;
    }

    public abstract void Reset();
}