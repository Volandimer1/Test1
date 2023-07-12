using UnityEngine;

public class BonusBase : FieldObject, ISelectable, IDamagable
{
    private GameObject _particalSystem;

    public BonusBase() : base()
    {
        _particalSystem = new();
    }

    public BonusBase(GameObject gameObject, int indexI, int indexJ)
    {
        Constructor(gameObject, indexI, indexJ);
    }

    public override void Constructor(GameObject gameObject, int indexI, int indexJ)
    {
        base.Constructor(gameObject, indexI, indexJ);
        _particalSystem = gameObject.transform.GetChild(0).gameObject;
        Moovable = true;
    }

    public void AddToChain()
    {
        _particalSystem.SetActive(true);
    }

    public void DeleteFromChain()
    {
        _particalSystem.SetActive(false);
    }

    public virtual void TakeDamage()
    {

    }

    public override void Reset()
    {
        _particalSystem.SetActive(false);
    }
}
