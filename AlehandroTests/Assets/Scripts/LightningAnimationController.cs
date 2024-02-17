using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LightningAnimationController : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private Texture[] _textures;
    [SerializeField] private float _fps = 30f;

    private int _animationFrame;
    private float _timeBetweenFrames;

    private void Update()
    {
        _timeBetweenFrames += Time.deltaTime;

        if (_timeBetweenFrames >= 1f/_fps)
        {
            _animationFrame++;
            if (_animationFrame >= _textures.Length)
                _animationFrame = 0;

            _lineRenderer.material.SetTexture("_MainTex", _textures[_animationFrame]);

            _timeBetweenFrames = 0;
        }
    }
}
