using UnityEngine;
using UnityEngine.Splines;

// SCOPE: allows enemies to find the position they have to move to based on relative progress along spline
public class Spline : MonoBehaviour
{
    SplineContainer splineContainer;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        splineContainer = GetComponent<SplineContainer>();
    }
    
    public Vector3 GetPositionOnSpline(float progress) {
        Vector3 position = splineContainer.Spline.EvaluatePosition(progress);
        Vector3 worldposition = splineContainer.transform.TransformPoint(position);
        return worldposition;
    }
}
