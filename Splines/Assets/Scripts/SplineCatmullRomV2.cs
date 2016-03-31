using UnityEngine;
using System.Collections.Generic;

public class SplineCatmullRomV2 : MonoBehaviour
{
    [SerializeField] private bool _isLooping = true;
    [SerializeField] private int _resolution = 100;
    [SerializeField] private List<Transform> _controlPointsList = new List<Transform>();

    private const float TOTAL = 1;
    private float _length;

    public float Length
    {
        get
        {
            _length = 0;
            for (int i = 0; i < _controlPointsList.Count - 3; i++)
            {
                _length += ApproximateLength(_controlPointsList[i].position, _controlPointsList[i + 1].position, _controlPointsList[i + 2].position, _controlPointsList[i + 3].position);
            }

            if (_isLooping) 
            { 
                _length += ApproximateLength(_controlPointsList[_controlPointsList.Count - 2].position, _controlPointsList[_controlPointsList.Count - 1].position, _controlPointsList[0].position, _controlPointsList[1].position);
            }

           // Debug.Log("lenth " + _length);
            return _length;
        }
    }

    public int CountPoints() 
    {
        return _controlPointsList.Count;    
    }

    public Vector3 MoveObject(float t, int index)
    {
        Vector3 pos = Vector3.zero;
        Vector3 p1 = _controlPointsList[index].position;
        Vector3 p2 = _controlPointsList[index + 1].position;
        Vector3 p3 = _controlPointsList[index + 2].position;
        Vector3 p4 = _controlPointsList[index + 3].position;
        
        if ( _controlPointsList.Count > 0 ) 
        { 
            pos = 0.5f * ((-p1 + 3 * p2 - 3 * p3 + p4) * t * t * t + (2 * p1 - 5 * p2 + 4 * p3 - p4) * t * t + (-p1 + p3) * t + 2 * p2);
        }

        return pos;
    }

    public float ApproximateSegmentLength(int index)
    {
        float total = 0;
        float t = 0;

        Vector3 p1 = _controlPointsList[index].position;
        Vector3 p2 = _controlPointsList[index + 1].position;
        Vector3 p3 = _controlPointsList[index + 2].position;
        Vector3 p4 = _controlPointsList[index + 3].position;
        
        Vector3 lastPosition = p2;
        Vector3 currentPosition;

        for (int i = 0; i < _resolution + 1; i++)
        {
            t = (TOTAL / _resolution) * i;
            //t = TOTAL;
            //t /= _resolution;
            //t *= i;

            currentPosition = 0.5f * ((-p1 + 3 * p2 - 3 * p3 + p4) * t * t * t + (2 * p1 - 5 * p2 + 4 * p3 - p4) * t * t + (-p1 + p3) * t + 2 * p2);
            total += (currentPosition - lastPosition).magnitude;
            lastPosition = currentPosition;
        }

        return total;
    }
    
    private float ApproximateLength(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
    {
        float total = 0;
        float t = 0;

        Vector3 lastPosition = p2;
        Vector3 currentPosition;

        for (int i = 0; i < _resolution + 1; i++)
        {
            t = (TOTAL / _resolution) * i;

            //t = TOTAL;
            //t /= _resolution;
            //t *= i;

            currentPosition = 0.5f * ((-p1 + 3 * p2 - 3 * p3 + p4) * t * t * t + (2 * p1 - 5 * p2 + 4 * p3 - p4) * t * t + (-p1 + p3) * t + 2 * p2);
            total += (currentPosition - lastPosition).magnitude;
            lastPosition = currentPosition;
        }

        return total;
    }


    //-----------------------------------------------

    //Display without having to press Play
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;

        //Draw a sphere at each control point
        for (int i = 0; i < _controlPointsList.Count; i++)
        {
            Gizmos.DrawWireSphere(_controlPointsList[i].position, 0.3f);
        }

        //Draw the Catmull-Rom lines between the points
        for (int i = 0; i < _controlPointsList.Count; i++)
        {
            //Cant draw between the endpoints
            //Neither do we need to draw from the second to the last endpoint
            //...if we are not making a looping line
            if ((i == 0 || i == _controlPointsList.Count - 2 || i == _controlPointsList.Count - 1) && !_isLooping)
            //if (( i == _controlPointsList.Count - 1) && !_isLooping) // not draw the start or end line -- 
            {
                continue;
            }

            DisplayCatmullRomSpline(i);
        }
    }
    
    //Returns a position between 4 Vector3 with Catmull-Rom Spline algorithm
    //http://www.iquilezles.org/www/articles/minispline/minispline.htm
    private Vector3 ReturnCatmullRom(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        Vector3 a = 0.5f * (2f * p1);
        Vector3 b = 0.5f * (p2 - p0);
        Vector3 c = 0.5f * (2f * p0 - 5f * p1 + 4f * p2 - p3);
        Vector3 d = 0.5f * (-p0 + 3f * p1 - 3f * p2 + p3);

        Vector3 pos = a + (b * t) + (c * t * t) + (d * t * t * t);

        return pos;
    }
    
    //Display a spline between 2 points derived with the Catmull-Rom spline algorithm
    private void DisplayCatmullRomSpline(int pos)
    {
        //Clamp to allow looping
        Vector3 p0 = _controlPointsList[ClampListPos(pos - 1)].position;
        Vector3 p1 = _controlPointsList[pos].position;
        Vector3 p2 = _controlPointsList[ClampListPos(pos + 1)].position;
        Vector3 p3 = _controlPointsList[ClampListPos(pos + 2)].position;
        
        //Just assign a tmp value to this
        Vector3 lastPos = Vector3.zero;

        //t is always between 0 and 1 and determines the resolution of the spline
        //0 is always at p1
        for (float t = 0; t < 1; t += 0.1f)
        {
            //Find the coordinates between the control points with a Catmull-Rom spline
            Vector3 newPos = ReturnCatmullRom(t, p0, p1, p2, p3);

            //Cant display anything the first iteration
            if (t == 0)
            {
                lastPos = newPos;
                continue;
            }

            Gizmos.DrawLine(lastPos, newPos);
            lastPos = newPos;
        }

        //Also draw the last line since it is always less than 1, so we will always miss it
        Gizmos.DrawLine(lastPos, p2);
    }


    //Clamp the list positions to allow looping
    //start over again when reaching the end or beginning
    private int ClampListPos(int pos)
    {
        if (pos < 0)
        {
            pos = _controlPointsList.Count - 1;
        }

        if (pos > _controlPointsList.Count)
        {
            pos = 1;
        }
        else if (pos > _controlPointsList.Count - 1)
        {
            pos = 0;
        }

        return pos;
    }
}
