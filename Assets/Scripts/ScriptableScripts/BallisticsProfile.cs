using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Ballistics Profile", menuName = "ScriptableObjects/Ballistics Profile", order = 1)]
public class BallisticsProfile : ScriptableObject
{
    [SerializeField, Range(1, 100)] float force = 20;
    public float Force { get { return force; } }
    [SerializeField, Range(1, 10)] float gravity = 1;
    public float Gravity { get { return gravity * Physics.gravity.y; } }
    [SerializeField, Min(1)] float hitMarginal = 2;
    public float HitMarginal { get { return hitMarginal; } }
    [SerializeField, Range(32, 200)] int trajectoryDetail = 32;
    public int TrajectoryDetail { get { return trajectoryDetail; } }
}
