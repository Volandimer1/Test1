using UnityEngine;

public class InputController : MonoBehaviour
{
    public delegate void DragHandlerDelegate(Vector3 currentPosition);
    public delegate void PointerDownHandlerDelegate(Vector3 position);
    public delegate void PointerUpHandlerDelegate(Vector3 position);

    public event DragHandlerDelegate OnDragEvent;
    public event PointerDownHandlerDelegate OnPointerDownEvent;
    public event PointerUpHandlerDelegate OnPointerUpEvent;

    private Vector3 touchStartPos;
    private Vector3 lastPosition;
    private bool isDragging = false;

    private bool Locked = false;

    public void Lock()
    {
        Locked = true;
    }

    public void UnLock()
    {
        Locked = false;
    }

    public void CleanUpSubscriptions()
    {
        OnDragEvent = null;
        OnPointerDownEvent = null;
        OnPointerUpEvent = null;
    }

    private void Update()
    {
        if (Locked == true)
            return;

        if ((Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)) && (isDragging == false))
        {
            touchStartPos = GetInputPosition();
            OnPointerDownEvent?.Invoke(touchStartPos);
            isDragging = true;
            lastPosition = touchStartPos;
        }

        if (isDragging)
        {
            Vector3 currentPosition = GetInputPosition();

            if (Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended))
            {
                isDragging = false;
                OnPointerUpEvent?.Invoke(currentPosition);
                return;
            }

            if (currentPosition != lastPosition)
            {
                OnDragEvent?.Invoke(currentPosition);
                lastPosition = currentPosition;
            }
        }
    }

    private Vector3 GetInputPosition()
    {
        if (Input.touchCount > 0)
        {
            return Input.GetTouch(0).position;
        }
        else
        {
            return Input.mousePosition;
        }
    }
}