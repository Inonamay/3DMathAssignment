using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OrbitSolver
{
    public static Vector3 GetWorldPointInOrbit(float radius, Transform center, float angle)
    {
        return center.TransformPoint(GetLocalPointInOrbit(radius, angle));
    }
    public static Vector3 GetLocalPointInOrbit(float radius, float angle)
    {
        float x = radius * Mathf.Sin(angle);
        float z = radius * Mathf.Cos(angle);
        return new Vector3(x, 0, z);
    }
}
