using System;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(BoxCollider))]
public class CornerOfTheEye : MonoBehaviour
{
    public Action<Vector3> distractionSeen;

    [SerializeField]
    private VisionField outerSight;

    public float size = 1.0f;

    [SerializeField]
    private Color gizmoColor = Color.red;

    public LayerMask targetLayer;
    
    public LayerMask allButRaycastLayer;

    public float farthestDistanceInside = 0;

    [SerializeField]
    private bool enableGizmos = true;

    [Space(20)]
    [SerializeField]
    private bool debugLogs = false;
    public VisionFieldData visionData;
    private bool targetSeenThisFrame;
    public bool TargetSeenThisFrame() => targetSeenThisFrame;

    private void Awake()
    {
        visionData = new VisionFieldData();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (debugLogs) Debug.Log(MathF.Pow(2, other.gameObject.layer));

        if(visionData == null) visionData = new VisionFieldData();
        if (MathF.Pow(2, other.gameObject.layer) == targetLayer)
        {
            if (debugLogs) Debug.Log("On Trigger Enter");
            visionData.targetGameObject = other.gameObject;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (MathF.Pow(2, other.gameObject.layer) == targetLayer)
        {
            targetSeenThisFrame = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (MathF.Pow(2, other.gameObject.layer) == targetLayer)
        {
             CheckTargetLayerWithRaycast(other);
        }

        if (other.gameObject.GetComponent<DistractionObject>())
        {
            CheckDistractionObjects(other);
        }
    }

    private void CheckTargetLayerWithRaycast(Collider other)
    {
        Vector3 directionVectorToTarget = other.transform.position - transform.position;
        Vector3 projectedVector = Vector3.Project(directionVectorToTarget, transform.forward);
        Vector3 raycastStartPosition = projectedVector + transform.position;
        RecalculateFarthestDistanceInside(GetComponent<BoxCollider>());

        if (Physics.Raycast(raycastStartPosition, other.transform.position - raycastStartPosition, out RaycastHit hit, farthestDistanceInside, allButRaycastLayer, QueryTriggerInteraction.Ignore))
        {
            //Debug.Log("Raycast from Corner");
            Debug.DrawLine(raycastStartPosition, hit.point, Color.black);
            if (MathF.Pow(2, hit.collider.gameObject.layer) == targetLayer)
            {
                if (other.GetComponent<PlayerController>())
                {
                    targetSeenThisFrame = !other.GetComponent<PlayerController>().IsHidden() && !other.GetComponent<PlayerController>().dead;
                }

                visionData.positionFromLastDetection = transform.position;
                visionData.angleToTarget = Vector3.Angle(transform.forward, other.transform.position - transform.position);
                visionData.targetPositionFromLastDetection = other.transform.position;
                visionData.targetGameObject = other.gameObject;
                //Debug.DrawLine(transform.position, other.transform.position, gizmoColor, 0.25f);
            }
            else
            {
                targetSeenThisFrame = false;
            }
        }
        else
        {
            targetSeenThisFrame = false;
        }

    }

    private void CheckDistractionObjects(Collider other)
    {
        if (other.GetComponent<DistractionObject>().isDistracting && !targetSeenThisFrame)
        {
            other.GetComponent<DistractionObject>().isDistracting = false;
            distractionSeen.Invoke(other.transform.position);
        }
    }

    //Draw for Designers :)
    private void OnDrawGizmos()
    {
        if (outerSight == null) return;

        BoxCollider box = GetComponent<BoxCollider>();
        ResizeBoxCollider(box);

        box.isTrigger = true;
        RecalculateFarthestDistanceInside(box);


        if (!enableGizmos) return;


        //Calculate Lines to Draw them
        Gizmos.color = Color.white;
        Vector3 point = transform.position + (transform.forward * box.center.z) + (transform.right * box.size.x * 0.5f);
        Gizmos.DrawLine(transform.position + (transform.forward * box.center.z), point);

        Vector3 bottomFrontLeft = ((transform.right * -box.size.x) + (transform.up * -box.size.y) + (transform.forward * box.size.z)) / 2.0f + (box.center.z * transform.forward) + transform.position;
        Vector3 bottomFrontRight = ((transform.right * box.size.x) + (transform.up * -box.size.y) + (transform.forward * box.size.z)) / 2.0f + (box.center.z * transform.forward) + transform.position;
        Vector3 topFrontLeft = ((transform.right * -box.size.x) + (transform.up * box.size.y) + (transform.forward * box.size.z)) / 2.0f + (box.center.z * transform.forward) + transform.position; ;
        Vector3 topFrontRight = ((transform.right * box.size.x) + (transform.up * box.size.y) + (transform.forward * box.size.z)) / 2.0f + (box.center.z * transform.forward) + transform.position;

        Vector3 bottomBackLeft = ((transform.right * -box.size.x) + (transform.up * -box.size.y) + (transform.forward * -box.size.z)) / 2.0f + (box.center.z * transform.forward) + transform.position;
        Vector3 bottomBackRight = ((transform.right * box.size.x) + (transform.up * -box.size.y) + (transform.forward * -box.size.z)) / 2.0f + (box.center.z * transform.forward) + transform.position;
        Vector3 topBackLeft = ((transform.right * -box.size.x) + (transform.up * box.size.y) + (transform.forward * -box.size.z)) / 2.0f + (box.center.z * transform.forward) + transform.position;
        Vector3 topBackRight = ((transform.right * box.size.x) + (transform.up * box.size.y) + (transform.forward * -box.size.z)) / 2.0f + (box.center.z * transform.forward) + transform.position; ;

        Gizmos.color = gizmoColor;
        Gizmos.DrawLine(outerSight.transform.position, transform.position);
        Gizmos.DrawLine(bottomFrontLeft, bottomFrontRight);
        Gizmos.DrawLine(bottomFrontLeft, topFrontLeft);
        Gizmos.DrawLine(bottomFrontLeft, bottomBackLeft);
        Gizmos.DrawLine(topFrontLeft, topFrontRight);
        Gizmos.DrawLine(bottomFrontRight, topFrontRight);
        Gizmos.DrawLine(bottomFrontRight, bottomBackRight);

        Gizmos.DrawLine(topBackRight, topFrontRight);
        Gizmos.DrawLine(topBackRight, topBackLeft);
        Gizmos.DrawLine(topBackRight, bottomBackRight);
        Gizmos.DrawLine(bottomBackRight, bottomBackLeft);
        Gizmos.DrawLine(topBackLeft, topFrontLeft);
        Gizmos.DrawLine(topBackLeft, bottomBackLeft);

    }

    private void RecalculateFarthestDistanceInside(BoxCollider box)
    {
        farthestDistanceInside = Vector3.Distance(transform.position + (transform.forward * box.center.z), 
                                 transform.position + (transform.forward * box.center.z) + (transform.right * box.size.x * 0.5f));
    }

    private void ResizeBoxCollider(BoxCollider box)
    {
        box.center = new Vector3(0.0f, 0.0f, size * 0.5f);
        float percentOnTheWayForCollider = (Vector3.Distance(transform.position, outerSight.transform.position) + size) / outerSight.visionRange;
        if (debugLogs) Debug.Log(percentOnTheWayForCollider);

        float zPoint = size;
        Vector3 positiveRightVector = outerSight.transform.right;
        if (positiveRightVector.x < 0)
            positiveRightVector.x *= -1.0f;
        float pointRight = ((outerSight.visionRadius) * percentOnTheWayForCollider);

        Vector3 sizeVector = new Vector3(pointRight * 2.0f, pointRight * 2.0f, zPoint);

        box.size = sizeVector;
    }

    public Vector3 GetMaxDistancePositionToTargetPosition(Vector3 targetPosition)
    {
        Vector3 distancePosition = transform.position + (targetPosition - transform.position).normalized * farthestDistanceInside;
        Debug.DrawLine(transform.position, distancePosition, gizmoColor);
        return distancePosition;
    }

    public Vector3 GetPositionOfTargetPosition(Vector3 targetPosition)
    {
        RecalculateFarthestDistanceInside(GetComponent<BoxCollider>());
        Vector3 directionVectorToTarget = targetPosition - transform.position;
        Vector3 projectedVector = Vector3.Project(directionVectorToTarget, transform.forward);
        Vector3 projectedVectorPoint = projectedVector + transform.position;
        Vector3 cubePosition = projectedVectorPoint + ((targetPosition - projectedVectorPoint).normalized * farthestDistanceInside);

        return cubePosition;
    }
}
