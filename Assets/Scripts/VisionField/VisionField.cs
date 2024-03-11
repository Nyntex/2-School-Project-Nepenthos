using System;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

[RequireComponent(typeof(BoxCollider))]
public class VisionField : MonoBehaviour, IDistractable
{
    public Action<Vector3> distractionSeen;

    [Min(0.1f)]
    public float visionRadius;

    [Min(0.1f)]
    public float visionRange;
    private float actualVisionRange = 0.0f;
    public float ActualVisionRange { get { return actualVisionRange; } }
    
    public float distanceToStartSeeing = 0.0f;

    [HideInInspector]
    public bool useZAxisToDistanceStartSeeing = false;

    [SerializeField]
    private Color gizmoColor = Color.red;

    [SerializeField]
    private bool enableGizmos = true;

    public LayerMask targetLayer;
   
    public LayerMask allButRaycastLayer;

    //private const int numTriangles = 64;
    private float maxAngle = 0.0f;
    public float MaxAngle { get { return Vector3.Angle(Vector3.forward, new Vector3(0, visionRadius, visionRange)); } }
    public VisionFieldData visionData;
    private bool targetSeenThisFrame;
    public bool TargetSeenThisFrame() => targetSeenThisFrame;

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(MathF.Pow(2, other.gameObject.layer));
        if (visionData == null) visionData = new VisionFieldData();
        if (MathF.Pow(2, other.gameObject.layer) == targetLayer)
        {

            //Debug.Log("On Trigger Enter");
            visionData.targetGameObject = other.gameObject;
        }

    }

    private void OnTriggerStay(Collider other)
    {
        maxAngle = Vector3.Angle(Vector3.forward, new Vector3(0, visionRadius, visionRange));
        if(MathF.Pow(2,other.gameObject.layer) == targetLayer)
        {
            if (maxAngle > Vector3.Angle(transform.forward, other.transform.position - transform.position))
            {

                CheckTargetLayerWithRaycast(other);
            }
            else
                targetSeenThisFrame = false;
            
        }

        if (other.gameObject.GetComponent<DistractionObject>())
        {
            CheckDistractionObjects(other);
        }
    }

    private void Awake()
    {
        visionData = new VisionFieldData();
        //UpdateCollisionBox();
    }

    private void CheckTargetLayerWithRaycast(Collider other)
    {
        if (Physics.Raycast(transform.position, other.transform.position - transform.position, out RaycastHit hit, actualVisionRange, allButRaycastLayer, QueryTriggerInteraction.Ignore))
        {
            if (MathF.Pow(2, hit.collider.gameObject.layer) == targetLayer)
            {
                PlayerController player = other.GetComponent<PlayerController>();
                if (player != null) 
                {
                    targetSeenThisFrame = !player.IsHidden() && !player.dead;
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
        if(other.GetComponent<DistractionObject>().isDistracting && !targetSeenThisFrame)
        {
            other.GetComponent<DistractionObject>().isDistracting = false;
            distractionSeen.Invoke(other.transform.position);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (MathF.Pow(2, other.gameObject.layer) == targetLayer)
        {
            targetSeenThisFrame = false;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (MathF.Pow(2, other.gameObject.layer) == targetLayer)
        {
            targetSeenThisFrame = false;
        }
    }

    private void Update()
    {
        
        if (!targetSeenThisFrame)
        {
            visionData.timeSinceLastSeen += Time.deltaTime;
        }
        else
        {
            visionData.timeSinceLastSeen = 0.0f;
        }
        visionData.currentDistanceToTargetPosition = Vector3.Distance(transform.position ,visionData.targetPositionFromLastDetection);

        UpdateCollisionBox();
    }

#if UNITY_EDITOR
    //Draw Cone and Resize BoxTrigger
    private void OnDrawGizmos()
    {
        UpdateCollisionBox();

        if (enableGizmos)
        {
            #region Draw Cone

            // Set Positions
            Vector3 forwardPosition = transform.position + (transform.forward * visionRange);
            Vector3 pointRight = forwardPosition + (transform.right * visionRadius);
            Vector3 pointLeft = forwardPosition + (-transform.right * visionRadius);
            Vector3 pointTop = forwardPosition + (transform.up * visionRadius);
            Vector3 pointBottom = forwardPosition + (-transform.up * visionRadius);
            actualVisionRange = Vector3.Distance(transform.position, pointRight);
            Vector3 actualForwardPoint = transform.position + (transform.forward * actualVisionRange);

            Vector3 unseenForwardPosition = transform.position + (transform.forward * distanceToStartSeeing);
            float percentofTheWay = distanceToStartSeeing / visionRange;
            Vector3 unseenPointRight = unseenForwardPosition + (transform.right * visionRadius * percentofTheWay);
            Vector3 unseenPointLeft = unseenForwardPosition + (-transform.right * visionRadius * percentofTheWay);
            Vector3 unseenPointTop = unseenForwardPosition + (transform.up * visionRadius * percentofTheWay);
            Vector3 unseenPointBottom = unseenForwardPosition + (-transform.up * visionRadius * percentofTheWay);

            //draw lines between points
            Gizmos.color = gizmoColor;
            Gizmos.DrawLine(transform.position, actualForwardPoint);
            Gizmos.DrawLine(unseenPointRight, pointRight);
            Gizmos.DrawLine(unseenPointLeft, pointLeft);
            Gizmos.DrawLine(unseenPointTop, pointTop);
            Gizmos.DrawLine(unseenPointBottom, pointBottom);

            if (distanceToStartSeeing > 0)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, unseenPointRight);
                Gizmos.DrawLine(transform.position, unseenPointLeft);
                Gizmos.DrawLine(transform.position, unseenPointTop);
                Gizmos.DrawLine(transform.position, unseenPointBottom);
            }
            //Draw Disc
            UnityEditor.Handles.color = gizmoColor;
            UnityEditor.Handles.DrawWireDisc(forwardPosition, transform.forward, visionRadius);

            //Set Positions for extra vision
            float distance = visionRange + ((actualVisionRange - visionRange) * 0.666f);

            Vector3 halfwayForward = transform.position + (transform.forward * distance);
            Vector3 halfwayTopPoint = halfwayForward + transform.up * visionRadius * 0.5f;
            Vector3 halfwayBotPoint = halfwayForward + -transform.up * visionRadius * 0.5f;
            Vector3 halfwayRightPoint = halfwayForward + transform.right * visionRadius * 0.5f;
            Vector3 halfwayLeftPoint = halfwayForward + -transform.right * visionRadius * 0.5f;

            //Draw extra vision
            Gizmos.color = gizmoColor;
            Gizmos.DrawLine(actualForwardPoint, halfwayLeftPoint);
            Gizmos.DrawLine(actualForwardPoint, halfwayRightPoint);
            Gizmos.DrawLine(actualForwardPoint, halfwayTopPoint);
            Gizmos.DrawLine(actualForwardPoint, halfwayBotPoint);

            Gizmos.DrawLine(halfwayBotPoint, pointBottom);
            Gizmos.DrawLine(halfwayTopPoint, pointTop);
            Gizmos.DrawLine(halfwayRightPoint, pointRight);
            Gizmos.DrawLine(halfwayLeftPoint, pointLeft);

            #endregion Draw Cone
        }


    }
#endif

    private void UpdateCollisionBox()
    {
        actualVisionRange = Vector3.Distance(transform.position, (transform.position + (transform.forward * visionRange)) + (-transform.up * visionRadius));
        float extraDistanceToStartSeeing = distanceToStartSeeing;
        //Debug.Log(useZAxisToDistanceStartSeeing);
        if (useZAxisToDistanceStartSeeing) //for this work properly z has to be negative
        {
            if (transform.localPosition.z >= 0)
            {
                Debug.Log("Z Position should be negative!");
                useZAxisToDistanceStartSeeing = false;
            }
            else
            { 
                extraDistanceToStartSeeing = -transform.localPosition.z;
            }
        }
        GetComponent<BoxCollider>().center = new Vector3(0, 0, (extraDistanceToStartSeeing + actualVisionRange) / 2.0f);
        GetComponent<BoxCollider>().size = new Vector3(visionRadius * 2.0f, visionRadius * 2.0f, actualVisionRange - extraDistanceToStartSeeing);
        GetComponent<BoxCollider>().isTrigger = true;
    }

    #region GenerateConeCollider
    /*
    private void GenerateMeshCollider()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        Vector3 lastCorner = new Vector3(visionRadius * MathF.Cos(0), visionRadius * MathF.Sin(0), visionRange);
        for(int i = 1;  i <= numTriangles; ++i) 
        { 
            float angle = -i* 360 / numTriangles * Mathf.Deg2Rad;
            Vector3 corner = new Vector3(visionRadius * MathF.Cos(angle), visionRadius * MathF.Sin(angle), visionRange);

            vertices.Add(new Vector3(0,0,0));
            vertices.Add(lastCorner);
            vertices.Add(corner);
            vertices.Add(new Vector3(0,0,visionRange));

            lastCorner = corner;
        }

        for(int i = 0; i < vertices.Count; i+=4) 
        {
            triangles.Add(i);
            triangles.Add(i+2);
            triangles.Add(i+1);
            triangles.Add(i+3);
            triangles.Add(i+1);
            triangles.Add(i+2);

        }

        var newMesh = new Mesh();
        newMesh.MarkDynamic();

        newMesh.vertices = vertices.ToArray();
        newMesh.triangles = triangles.ToArray();

        GetComponent<MeshCollider>().sharedMesh = newMesh;

        
        foreach(var vertice in vertices)
        {
            Debug.Log(vertice);
        }
        foreach(var triangle in triangles)
        {
            Debug.Log(triangle);
        }    
    }
    */
    #endregion GenerateConeCollider

    public void Distract(Vector3 sourcePosition)
    {
        if(transform.parent.GetComponent<IDistractable>() != null) 
        { 
            transform.parent.GetComponent<IDistractable>().Distract(sourcePosition);
        }
    }

    public Vector3 GetConePointBasedOfTargetPosition(Vector3 targetPosition, float distance, Color color)
    {
        Vector3 directionVectorToTarget = (targetPosition - transform.position);

        // Getting target direction vector projected onto the line / getting the closest point on the Line, 
        // receiving a Vector from target position to closest point product on the forward vector
        Vector3 projectedVector = Vector3.Project(directionVectorToTarget, transform.forward);

        //If this distance is bigger than the vision Range I return the max distance position
        distance = Vector3.Distance(transform.position, projectedVector + transform.position);
        if (distance > visionRange) return GetMaxDistancePositionToTargetPosition(targetPosition);

        //otherwise I look how far this is on the actual vision range and calculate the distance to the cone point
        float percentOfTheWay = distance / visionRange;
        Vector3 conePoint = transform.position + ((targetPosition - (transform.position + projectedVector)).normalized * visionRadius * percentOfTheWay) 
                            + (transform.forward * visionRange * percentOfTheWay);
        

        //Draw Line to Player
        Debug.DrawLine(projectedVector + transform.position, conePoint, color);  
        Debug.DrawLine(transform.position, conePoint, color);

        return conePoint;
    }

    public Vector3 GetMaxDistancePositionToTargetPosition(Vector3 targetPosition)
    {
        Vector3 distancePosition = transform.position + (targetPosition - transform.position).normalized * actualVisionRange;
        Debug.DrawLine(transform.position, distancePosition,gizmoColor);
        return distancePosition;
    }
}

public class VisionFieldData
{
    public float currentDistanceToTargetPosition = 0.0f;
    public float angleToTarget = 0.0f;
    public Vector3 targetPositionFromLastDetection = Vector3.zero;
    public Vector3 positionFromLastDetection = Vector3.zero;
    public float timeSinceLastSeen = 0.0f;
    public GameObject targetGameObject = null;
}