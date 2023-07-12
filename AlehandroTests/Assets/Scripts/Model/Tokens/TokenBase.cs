using UnityEngine;

public class TokenBase : FieldObject, ISelectable, IDamagable
{
    private GameObject _particalSystem;

    public TokenBase() : base()
    {
        _particalSystem = new();
    }

    public TokenBase(GameObject gameObject, int indexI, int indexJ) 
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

    public void TakeDamage()
    {

    }

    public override void Reset()
    {
        _particalSystem.SetActive(false);
    }
}