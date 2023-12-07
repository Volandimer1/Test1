using System.Collections.Generic;
using UnityEngine;

public interface IConstruct
{
    public void Constructor(GameObject gameObject, int indexI, int indexJ, ObjectPooller objectPoller, GoalsManager goalsManager);
}