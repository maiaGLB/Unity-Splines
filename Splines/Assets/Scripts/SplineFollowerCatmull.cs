using UnityEngine;
using System.Collections;

public class SplineFollowerCatmull : MonoBehaviour
{
    [SerializeField]
    private SplineCatmullRomV2 _spline;
    [SerializeField]
    private float _duration;

    private CameraController _camera;
    private int _indexSpline = 0;
    private float deltaT = 0;

    protected void Start()
    {
        deltaT = 0;
        _camera = this.GetComponent<CameraController>();
    }

    protected void FixedUpdate()
    {
        if ((_indexSpline + 3) < _spline.CountPoints())
        {
            float proportionalDuration = _duration * (_spline.ApproximateSegmentLength(_indexSpline) / _spline.Length);
            deltaT += Time.fixedDeltaTime / proportionalDuration;

            if (deltaT >= 0 && deltaT <= 1)
            {
                transform.position = _spline.MoveObject(deltaT, _indexSpline);
            }

            if (deltaT >= 1)
            {
                _indexSpline += 1;
                deltaT -= 1;
            }
        }
    }
}
