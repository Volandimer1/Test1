using UnityEngine;
using UnityEngine.EventSystems;

public class SelectLevel : MonoBehaviour, IPointerClickHandler
{
    private LevelContainerScrollView _container;

    public void Initialize(LevelContainerScrollView container)
    {
        _container = container;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _container.ItemClicked(gameObject);
    }
}
