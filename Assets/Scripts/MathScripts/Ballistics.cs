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
    
    public static (Quaternion, Quaternion) ShowCorrectTrajectory(BallisticsProfile bp, Transform startPoint, Vector3 endPoint, bool draw = false)
    {
        float pointCount = 120;
        float verticalDistance = endPoint.y - startPoint.position.y;
        
        
        Vector3 directionToTarget = (endPoint - startPoint.position).normalized;
        Vector3 neutralStart = new Vector3(startPoint.position.x, 0, startPoint.position.z);
        Vector3 neutralEnd = new Vector3(endPoint.x, 0, endPoint.z);
        float distToTarget = Vector3.Distance(neutralStart, neutralEnd);
        float totalDist = Mathf.Sqrt(distToTarget * distToTarget + verticalDistance * verticalDistance);
        
        (float angleLow, float angleHigh) launchAngles = GetLaunchAngles(totalDist, bp.Force, bp.Gravity);
        
        //Debug.DrawLine(startPoint.position,new Vector3(70, Mathf.Tan(12.5f * 3.14f / 180) * 70, 0), Color.cyan);
       // Debug.DrawRay(startPoint.position,new Vector3(70, Mathf.Tan(35 * 3.14f / 180) * 70, 0), Color.blue);
        float flatAim = Mathf.Sin(launchAngles.angleLow) * distToTarget;
        float verticalAim = flatAim + verticalDistance;
        float aimAngle = Mathf.Atan((1.2f * verticalDistance + 12.5f) / distToTarget);//launchAngles.angleLow * 180 / 3.14f; //Mathf.Atan(verticalAim / distToTarget);
        //aimAngle += verticalDistance;
        //aimAngle = aimAngle * 3.14f / 180;
        //aimAngle = -aimAngle * (180 / 3.14f);

        (float angleDegLow, float angleDegHigh) launchAngleDegs = (-launchAngles.angleLow * (180 / 3.14f),
            -launchAngles.angleHigh * (180 / 3.14f));

        (Vector3 forwardLow, Vector3 forwardHigh) directions = (Quaternion.Euler(0, launchAngleDegs.angleDegLow, 0) * directionToTarget, 
            Quaternion.Euler(0, launchAngleDegs.angleDegHigh, 0) * directionToTarget);
        
        
        (Quaternion rotationLow, Quaternion rotationHigh) rotations = (Quaternion.LookRotation(directions.forwardLow),
            Quaternion.LookRotation(directions.forwardHigh));
        if(draw)
        {
            List<Vector3> pointsDA = new List<Vector3>();
            List<Vector3> pointsDB = new List<Vector3>();
            List<Vector3> pointsVertical = new List<Vector3>();
            const float dt = 1f / 10f;
            for (int i = 0; i < pointCount; i++)
            {
                float time = i * dt;
                Vector2 pt2DA = GetPointInTrajectory(Vector3.zero, bp.Force, launchAngles.angleLow, time, bp.Gravity);
                Vector2 pt2DB = GetPointInTrajectory(Vector3.zero, bp.Force, launchAngles.angleHigh, time, bp.Gravity);
                Vector2 pt2vertical = GetPointInTrajectory(Vector3.zero, bp.Force, aimAngle, time, bp.Gravity);

                Vector3 GetWorldPoint(Vector2 point2D)
                {
                    Vector3 pt = directionToTarget * point2D.x; // lateral offset
                    pt.y = point2D.y; // vertical offset
                    return startPoint.position + pt;

                }
                pointsDA.Add(GetWorldPoint(pt2DA));
                pointsDB.Add(GetWorldPoint(pt2DB));
                pointsVertical.Add(GetWorldPoint(pt2vertical));
            }
            DrawTrajectory(Color.green, pointsDA, draw);
            DrawTrajectory(Color.green, pointsDB, draw);
            //DrawTrajectory(Color.blue, pointsVertical, draw);
        }
        return rotations;
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
    static Vector2 GetPointInTrajectory(Vector2 startPoint, float launchSpeed, float launchAngRad, float time, float gravity)
    {
        float xDisp = launchSpeed * time * Mathf.Cos(launchAngRad);
        float yDisp = launchSpeed * time * Mathf.Sin(launchAngRad) + .5f * gravity * time * time;
        return startPoint + new Vector2(xDisp, yDisp);
    }
    static (float, float) GetLaunchAngles(float dist, float speed, float gravity)
    {
        //float angle = Mathf.Asin((yDist * (2 * gravity)) / speed);
        float asinContent = Mathf.Clamp((dist * -gravity) / (speed * speed), -1, 1);
        return
            (Mathf.Asin(asinContent) / 2,
                (Mathf.Asin(-asinContent) + Mathf.PI) / 2);
    }
    static Vector2 GetPointInTrajectoryByAngle(Vector2 startPoint, float launchSpeed, float launchAngRad, float time, float gravity)
    {
        float xDisp = launchSpeed * time * Mathf.Cos(launchAngRad * Mathf.Deg2Rad);
        float yDisp = launchSpeed * time * Mathf.Sin(launchAngRad * Mathf.Deg2Rad) + .5f * gravity * time * time;
        return startPoint + new Vector2(xDisp, yDisp);
    }
}
