using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVisionField : MonoBehaviour
{
    public Action<Vector3> distractionSeen;

    #region Inner View
    [Space(5)]
    [Header("Inner View Settings")]
    [Space(5)]
    [SerializeField]
    private VisionField innerSight;
    public VisionField InnerSight { get { return innerSight; } }

    [SerializeField]
    [Min(0.1f)]
    private float innerVisionRadius = 5.0f;
    public float InnerVisionRadius { get {  return innerVisionRadius; } }

    [SerializeField]
    [Min(0.1f)]
    private float innerVisionRange = 10.0f;
    public float InnerVisionRange { get { return innerVisionRange; } }

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float innerMultiplier = 0.33f;
    public float InnerMultiplier { get { return innerMultiplier; } }

    public bool TargetInInnerSight() => innerSight.TargetSeenThisFrame();

    #endregion Inner View

    #region outer Sight
    [Space(5)]
    [Header("Outer View Settings")]
    [Space(5)]
    [SerializeField]
    private VisionField outerSight;
    public VisionField OuterSight { get { return outerSight; } }

    [SerializeField]
    [Min(0.1f)]
    private float outerVisionRadius = 8.0f;
    public float OuterVisionRadius { get { return outerVisionRadius; } }

    [SerializeField]
    [Min(0.1f)]
    private float outerVisionRange = 12.0f;
    public float OuterVisionRange { get { return outerVisionRange; } }

    [Min(0)]
    [SerializeField]
    private float distanceToStartSeeing = 2.5f;
    public float DistanceToStartSeeing { get {  return distanceToStartSeeing; } }

    [SerializeField]
    private bool shouldStartSeeingUseZPosition = true;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float outerMultiplier = 0.33f;
    public float OuterMultiplier { get { return outerMultiplier; } }

    public bool TargetInOuterSight() => outerSight.TargetSeenThisFrame();

    #endregion outer Sight

    #region Corner of the Eye
    [Space(5)]
    [Header("Corner of the Eye Settings")]
    [Space(5)]
    [SerializeField]
    private CornerOfTheEye cornerOfTheEye;
    [SerializeField]
    private float size = 12;
    public float Size { get { return size; } }

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float cornerMultiplier = 0.33f;
    public float CornerMultiplier { get { return cornerMultiplier; } }

    public bool TargetInCornerOfTheEyeSight() => cornerOfTheEye.TargetSeenThisFrame();

    #endregion


    [Space(5)]
    [Header("Unspecific Settings")]
    [Space(5)]

    [SerializeField]
    private LayerMask targetLayer;

    [SerializeField]
    private LayerMask allButRaycastLayer;

    public VisionFieldData visionData;

    public bool AnySightSeesTarget()
    {
        if (TargetInCornerOfTheEyeSight() || TargetInInnerSight() || TargetInOuterSight())
            return true;
        else return false;
    }

    private void Awake()
    {
        if (visionData == null) visionData = new VisionFieldData();
        outerSight.distractionSeen += DistractionSeen;
        innerSight.distractionSeen += DistractionSeen;
        cornerOfTheEye.distractionSeen += DistractionSeen;
        SetVisionFields();
    }
    
    private void OnDrawGizmos()
    {
        SetVisionFields();
    }
    

    private void SetVisionFields()
    {
        if (outerSight == null || innerSight == null) return;
        outerSight.visionRadius = outerVisionRadius;
        outerSight.visionRange = outerVisionRange;
        if (shouldStartSeeingUseZPosition)
        {
            outerSight.distanceToStartSeeing = -outerSight.transform.localPosition.z;
            distanceToStartSeeing = -outerSight.transform.localPosition.z;
        }
        else
        {
            outerSight.distanceToStartSeeing = distanceToStartSeeing;
        }

        innerSight.visionRange = innerVisionRange;
        innerSight.visionRadius = innerVisionRadius;

        outerSight.targetLayer = targetLayer;
        innerSight.targetLayer = targetLayer;
        cornerOfTheEye.targetLayer = targetLayer;

        outerSight.allButRaycastLayer = allButRaycastLayer;
        innerSight.allButRaycastLayer = allButRaycastLayer;
        cornerOfTheEye.allButRaycastLayer = allButRaycastLayer;

        cornerOfTheEye.size = size;
    }
    private void Update()
    {
        GrabVisionData();
        GetVisionFieldAlertnessMultiplier();
        SetVisionFields();
    }

    private void GrabVisionData()
    {
        if(!AnySightSeesTarget()) return;
        if (TargetInInnerSight())
        {
            visionData = innerSight.visionData;
            cornerOfTheEye.visionData = visionData;
            outerSight.visionData = visionData;
            return;
        }

        if (TargetInOuterSight())
        {
            visionData = outerSight.visionData;
            innerSight.visionData = visionData;
            cornerOfTheEye.visionData = visionData;
            return;
        }

        if(TargetInCornerOfTheEyeSight())
        {
            visionData = cornerOfTheEye.visionData;
            innerSight.visionData = visionData;
            outerSight.visionData = visionData;
            return;
        }
    }
    
    public void DistractionSeen(Vector3 distractionPosition)
    {
        distractionSeen.Invoke(distractionPosition);
    }

    public float GetVisionFieldAlertnessMultiplier()
    {
        if (!AnySightSeesTarget()) return 0f;

        if (TargetInInnerSight()) return innerMultiplier;
        if (TargetInOuterSight() && !TargetInCornerOfTheEyeSight()) return outerMultiplier;
        if (!TargetInOuterSight() && TargetInCornerOfTheEyeSight()) return cornerMultiplier;
        if (TargetInCornerOfTheEyeSight() && TargetInOuterSight()) return cornerMultiplier + outerMultiplier;

        return 0f;
        /* crazy calculations
        float totalDistance = 0.0f;
        float multiplierDistance = 0.0f;
        float multiplier = 0.0f;
        Debug.Log("GetDistanceMultiplier");
        if (TargetInCornerOfTheEyeSight())
        {
            float distance = Vector3.Distance(innerSight.transform.position, visionData.targetGameObject.transform.position);
            Vector3 innerConePoint = innerSight.GetConePointBasedOfTargetPosition(visionData.targetPositionFromLastDetection, distance, Color.cyan);

            Vector3 outerPoint = cornerOfTheEye.GetPositionOfTargetPosition(visionData.targetPositionFromLastDetection);

            totalDistance = Vector3.Distance(innerConePoint, outerPoint);
            multiplierDistance = Vector3.Distance(outerPoint, visionData.targetPositionFromLastDetection);
            multiplier = multiplierDistance / totalDistance;

            Debug.Log("Current Percentage: " + multiplier);
            return multiplier;
        }
        if (TargetInOuterSight())
        {
            float distance = Vector3.Distance(innerSight.transform.position, visionData.targetGameObject.transform.position);
            Vector3 innerConePoint;
            Vector3 outerConePoint;
            if(distance < innerSight.ActualVisionRange)
            {
                innerConePoint = innerSight.GetConePointBasedOfTargetPosition(visionData.targetPositionFromLastDetection, distance, Color.cyan);
            }
            else 
            { 
                innerConePoint = innerSight.GetMaxDistancePositionToTargetPosition(visionData.targetPositionFromLastDetection);
            }

            float angleToTarget = Vector3.Angle(innerSight.transform.forward, visionData.targetPositionFromLastDetection - innerSight.transform.position);
            Debug.Log("inner sight max angle: "+ innerSight.MaxAngle);
            Debug.Log("angle to target: " + angleToTarget);

            if (angleToTarget < innerSight.MaxAngle && distance > innerSight.ActualVisionRange)
            {
                outerConePoint = outerSight.GetMaxDistancePositionToTargetPosition(visionData.targetPositionFromLastDetection);
            }
            else 
            {
                outerConePoint = outerSight.GetConePointBasedOfTargetPosition(visionData.targetPositionFromLastDetection, distance, Color.magenta);
            }
            

            totalDistance = Vector3.Distance(innerConePoint, visionData.targetPositionFromLastDetection) + Vector3.Distance(outerConePoint, visionData.targetPositionFromLastDetection);
            multiplierDistance = Vector3.Distance(outerConePoint, visionData.targetPositionFromLastDetection);
            multiplier = multiplierDistance / totalDistance;

            Debug.Log("Current Percentage: " + multiplier);
            return multiplier;
        }
        return -1f;
        */
    }
}
