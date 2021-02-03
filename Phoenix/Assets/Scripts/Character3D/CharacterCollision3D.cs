using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class CharacterCollision3D : MonoBehaviour
{
    const float skinWidth = .15f;

    public Transform root;

    private LayerMask oneSideCollision;
    private LayerMask hardCollision;
    private int layerOneSide;
    private int layerHard;
    private int layerCombined;

    const float verticalRaySpacing = 0.25f;

    BoxCollider collider;
    RaycastOrigins raycastOrigins;
    public CollisionInfo collisions;

    Vector3 raycastFloorPos;
    Vector3 floorMovement;
    Vector3 gravity;
    Vector3 CombinedRaycast;

    //
    public bool debugCollision;
    Rigidbody rb;


    void Start()
    {
        collider = GetComponent<BoxCollider>();
        layerOneSide = oneSideCollision.value;
        layerHard = hardCollision.value;

        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        debugCollision = collisions.below;
    }

    public void Move(Vector3 deltaMovement)
    {
        //UpdateRaycastOrigins();
        collisions.Reset();

        if (deltaMovement.y != 0)
        {
            VerticalCollisions(ref deltaMovement);
        }

        transform.Translate(deltaMovement, Space.World);
    }

    public void MoveIgnoreCollision(Vector3 deltaMovement)
    {
        transform.Translate(deltaMovement);
    }

    public void MoveIgnoreCollision(Vector3 deltaMovement, Collider2D ignoreColliders)
    {
        if (ignoreColliders == null)
        {
            Move(deltaMovement);
        }
        else
        {
            ignoreColliders.enabled = false;
            Move(deltaMovement);
            ignoreColliders.enabled = true;
        }
    }

    void VerticalCollisions(ref Vector3 deltaMovement)
    {
        float directionY = Mathf.Sign(deltaMovement.y);
        float rayLength = Mathf.Abs(deltaMovement.y) + skinWidth;
        bool below = (directionY == -1);
        //int testLayer = directionY > 0 ? layerHard : (layerOneSide | layerHard);
        int layer = layerHard;

        float hitDistance = 0;
        int hitCount = 0;
        for (int i = 0; i < 5; i++)
        {
            RaycastHit hit;
            Vector3 rayOrigin = (i == 0) ? root.TransformPoint(0, -skinWidth * directionY, 0) 
                : root.TransformPoint(Mathf.Sin(i * Mathf.PI / 2), -skinWidth * directionY, Mathf.Cos(i * Mathf.PI / 2));
            Debug.DrawRay(rayOrigin, Vector3.down, Color.magenta);
            if (Physics.Raycast(rayOrigin, Vector3.down, out hit, rayLength))
            {
                hitCount++;
                hitDistance += hit.distance;
            }
        }

        if (hitCount > 0)
        {
            collisions.below = below;
            deltaMovement.y = ((hitDistance / hitCount) - skinWidth) * directionY;
        }



        /*
        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = below ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + deltaMovement.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, layer);

            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

            if (hit)
            {
                deltaMovement.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance;

                collisions.below = below;
                collisions.above = !below;

                if (below)
                {
                    collisions.belowTransform = hit.transform;
                    collisions.belowCollider = hit.collider;
                }
                else collisions.aboveTransform = hit.transform;
            }
        }
        */
    }

    /// <summary>
    /// shrink first to make sure raycast doesn't go through
    /// </summary>
    void UpdateRaycastOrigins()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    private bool CheckGrounded()
    {
        return (FloorRaycasts(0, 0, 1.001f) != Vector3.zero);
    }

    Vector3 FindGround()
    {
        // width of raycasts around the centre of your character
        float raycastWidth = 0.25f;
        // check floor on 5 raycasts   , get the average when not Vector3.zero  
        int floorAverage = 1;

        CombinedRaycast = FloorRaycasts(0, 0, 2.0f);
        floorAverage += (getFloorAverage(raycastWidth, 0) + getFloorAverage(-raycastWidth, 0) + getFloorAverage(0, raycastWidth) + getFloorAverage(0, -raycastWidth));

        return CombinedRaycast / floorAverage;
    }

    // only add to average floor position if its not Vector3.zero
    int getFloorAverage(float offsetx, float offsetz)
    {

        if (FloorRaycasts(offsetx, offsetz, 1.6f) != Vector3.zero)
        {
            CombinedRaycast += FloorRaycasts(offsetx, offsetz, 1.6f);
            return 1;
        }
        else { return 0; }
    }


    Vector3 FloorRaycasts(float offsetx, float offsetz, float raycastLength)
    {
        RaycastHit hit;
        // move raycast
        raycastFloorPos = root.TransformPoint(0 + offsetx, 0 + skinWidth, 0 + offsetz);

        Debug.DrawRay(raycastFloorPos, Vector3.down, Color.magenta);
        if (Physics.Raycast(raycastFloorPos, -Vector3.up, out hit, raycastLength))
        {
            return hit.point;
        }
        else return Vector3.zero;
    }

    /// <summary>
    /// calculate tilt angle of bottom surface
    /// </summary>
    public float CalculateTiltAngle()
    {
        float angle = 0;

        int testLayer = (layerOneSide | layerHard);
        float dist0 = -1, dist1 = -1;
        float l = 0, r = 0;
        int verticalRayCount = 5;
        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = raycastOrigins.bottomLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, 1, testLayer);

            if (hit)
            {
                if (dist0 < 0)
                {
                    l = i;
                    dist0 = hit.distance;
                }
                else
                {
                    r = i;
                    dist1 = hit.distance;
                }
            }
        }
        if (l < r)
        {
            angle = Mathf.Atan2(dist1 - dist0, verticalRaySpacing * (r - l));
        }
        collisions.tiltAngle = angle;
        return angle;
    }

    struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

    public struct CollisionInfo
    {
        public bool above, below;
        public float tiltAngle;

        public Transform aboveTransform, belowTransform, leftTransform, rightTransform;
        public Collider2D belowCollider;

        public void Reset()
        {
            above = below = false;
            aboveTransform = belowTransform = leftTransform = rightTransform = null;
            belowCollider = null;
            tiltAngle = 0;
        }
    }

}


