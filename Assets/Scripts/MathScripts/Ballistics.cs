using System.Collections.Generic;
using UnityEngine;


public static class Ballistics
{
    
    public static bool IsSpotHittable(BallisticsProfile bp, Transform startPoint, Vector3 endPoint, Vector3 launchDirection, bool drawTrajectory = true)
    {
        if(bp == null)
        {
            Debug.LogError("Invalid ballistics profile!");
            return false;
        }
        if (startPoint == null)
        {
            Debug.LogError("Invalid startpoint!");
            return false;
        }
        //Variables for drawing the trajectory
        List<Vector3> trajectoryPoints = new List<Vector3>();
        Color trajectoryColor = Color.red;
        launchDirection = launchDirection.normalized;
        const float dt = 1 / 20f;
        #region Angle calculation
        float xDir = Mathf.Abs(launchDirection.x) + Mathf.Abs(launchDirection.z);
        Vector3 up = Vector3.up * launchDirection.y + Vector3.right * xDir;
        float launchAngle = Vector3.Angle(Vector3.right, up);
        if(launchDirection.y < 0)
        { launchAngle = -launchAngle; }
        #endregion
       
        
        for (int i = 0; i < bp.TrajectoryDetail; i++)
        {
            float time = i * dt;
            Vector2 point2D = GetPointInTrajectoryByAngle(Vector2.zero, bp.Force, launchAngle, time, bp.Gravity);
            Vector3 relativePoint = launchDirection * point2D.x;
            relativePoint.y = point2D.y;
            Vector3 worldPoint = startPoint.position + relativePoint;
            float distance = Vector3.Distance(worldPoint, endPoint);
            trajectoryPoints.Add(worldPoint);
            if (distance < bp.HitMarginal)
            {
                trajectoryColor = Color.green;
                DrawTrajectory(trajectoryColor, trajectoryPoints, drawTrajectory);
                return true;
            }
        }
        DrawTrajectory(trajectoryColor, trajectoryPoints, drawTrajectory);
        return false;
    }
    
    
    static void DrawTrajectory(Color trajectoryColor, List<Vector3> trajectoryPoints, bool draw)
    {
        if (!draw)
        { return; }
        for (int i = 0; i < trajectoryPoints.Count; i++)
        {
            if(i < trajectoryPoints.Count - 1)
            {
                Debug.DrawLine(trajectoryPoints[i], trajectoryPoints[i + 1], trajectoryColor);
            }
        }
    }
    static Vector2 GetPointInTrajectoryByAngle(Vector2 startPoint, float launchSpeed, float launchAngRad, float time, float gravity)
    {
        float xDisp = launchSpeed * time * Mathf.Cos(launchAngRad * Mathf.Deg2Rad);
        float yDisp = launchSpeed * time * Mathf.Sin(launchAngRad * Mathf.Deg2Rad) + .5f * gravity * time * time;
        return startPoint + new Vector2(xDisp, yDisp);
    }
}
