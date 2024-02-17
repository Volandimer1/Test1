using UnityEngine;

public class LightningController
{
    private LineRenderer _lineRenderer;

    public LightningController(LineRenderer lineRenderer)
    {
        _lineRenderer = lineRenderer;
    }

    public void AddPoint(Vector2 position)
    {
        _lineRenderer.positionCount++;
        _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, position);
    }

    public void RemovePoint()
    {
        _lineRenderer.positionCount--;
    }

    public void RemoveAll()
    {
        _lineRenderer.positionCount = 0;
    }
}
