using System.Collections.Generic;
using UnityEngine;

public abstract class FieldObject : IConstruct
{
    public Indexes Indexes;
    public GameObject PrefabInstance;

    protected FieldObject[,] _fieldObjects;
    protected List<Indexes> _emptyCellsIndexes;
    protected List<int> _indexOfARowForSortInEmptyCells;
    protected ObjectPooller _objectPoller;
    protected GoalsManager _goalsManager;

    public bool Movable { get; protected set; }

    private Vector2 StartingPosition, TargetPosition;

    public FieldObject()
    {
        Movable = false;
    }

    public FieldObject(GameObject gameObject, int indexI, int indexJ, ObjectPooller objectPoller, GoalsManager goalsManager)
    {
        Constructor(gameObject, indexI, indexJ, objectPoller, goalsManager);
    }

    public virtual void Constructor(GameObject gameObject, int indexI, int indexJ, ObjectPooller objectPoller, GoalsManager goalsManager)
    {
        PrefabInstance = gameObject;
        ChangePosition(indexI, indexJ);

        _objectPoller = objectPoller;
        _goalsManager = goalsManager;
    }

    public virtual void TakeDamage(ref FieldObject[,] fieldObjects, ref List<Indexes> emptyCellsIndexes, ref List<int> indexOfARowForSortInEmptyCells)
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

        result.Column = (int)((position.x + 640 + 160) / 320);
        result.Row = (int)((position.y - 2720 - 160) / (-320));

        return result;
    }

    public abstract void Reset();
}