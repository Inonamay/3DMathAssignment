using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    [SerializeField] Transform orbitPoint = null;
    [SerializeField] bool lookAtOrbit = false;
    [SerializeField] bool drawOrbit = true;
    [SerializeField] float sphereSize = 1;
    [SerializeField, Range(0, 360)] float angleDegrees = 0;
    float angle = 0;
    [SerializeField, Range(1, 50)] float radius = 10;
    [SerializeField, Range(-1, 1)] int direction = 1;
    [SerializeField, Range(0, 10)] float speed = 1;
    public float Angle
    {
        get
        {
            return angle;
        }
        set
        {
            if (value > 6.28f)
            {
                angle = 0;
                angleDegrees = 0;
                return;
            }
            if (value < 0)
            {
                angle = 6.28f;
                angleDegrees = 360;
                return;
            }
            angleDegrees = value * 57.3f;
            angle = value;
        }
    }
    float counter = 0;
    private void Awake()
    {
        Angle = angleDegrees / 57.3f;
    }
    void Update()
    {
        Angle += speed * direction * 0.005f;
        if (lookAtOrbit)
        {
            Vector3 forward = transform.position - OrbitSolver.GetWorldPointInOrbit(radius, orbitPoint, angle);
            forward.y = -forward.y;
            transform.rotation = Quaternion.LookRotation(forward);
        }
        else
        {
            transform.position = OrbitSolver.GetWorldPointInOrbit(radius, orbitPoint, angle);
        }
        
    }
    private void OnDrawGizmos()
    {
        if(!drawOrbit || !orbitPoint)
        {
            return;
        }
        float detail = radius * 2;
        for (int i = 0; i < detail; i++)
        {
            Gizmos.DrawSphere(OrbitSolver.GetWorldPointInOrbit(radius, orbitPoint, i), radius * 0.1f * sphereSize);
        }
    }
}
