using UnityEngine;

public interface IConstruct
{
    public void Constructor(GameObject gameObject, int indexI, int indexJ, FieldObjectPooller objectPoller, GoalsManager goalsManager, Field field);
}