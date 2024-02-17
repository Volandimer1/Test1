using System.Collections.Generic;
using UnityEngine;

public class BonusBase : FieldObject, ISelectable
{
    private GameObject _particalSystem;

    public BonusBase() : base()
    {

    }

    public BonusBase(GameObject gameObject, int indexI, int indexJ, FieldObjectPooller objectPoller, GoalsManager goalsManager, Field field, AudioManager audioManager)
    {
        Constructor(gameObject, indexI, indexJ, objectPoller, goalsManager, field, audioManager);
    }

    public override void Constructor(GameObject gameObject, int indexI, int indexJ, FieldObjectPooller objectPoller, GoalsManager goalsManager, Field field, AudioManager audioManager)
    {
        base.Constructor(gameObject, indexI, indexJ, objectPoller, goalsManager, field, audioManager);
        _particalSystem = gameObject.transform.GetChild(1).gameObject;
        Movable = true;
    }

    public void AddToChain()
    {
        _particalSystem.SetActive(true);
    }

    public void DeleteFromChain()
    {
        _particalSystem.SetActive(false);
    }

    public override void TakeDamage()
    {

    }

    public override void Reset()
    {
        _particalSystem.SetActive(false);
    }
}
