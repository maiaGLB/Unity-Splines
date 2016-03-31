using UnityEngine;
using System.Collections;

public class SplineFollower : MonoBehaviour
{
    [SerializeField] private BezierCurve _spineScoreBoard;
    [SerializeField] private float _duration;

    private CameraController _camera;
    private float _gameStartTime;

    protected void Start()
    {
        _gameStartTime = Time.time;
        _camera = this.GetComponent<CameraController>();
    }

    protected void Update()
    {
        float delta = Time.time - _gameStartTime;
        
        float t = delta / _duration;

        Vector3 v = _spineScoreBoard.GetPointAt(t);
        this.transform.position = v;
    }
}
