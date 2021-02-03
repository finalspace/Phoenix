using UnityEngine;
using System.Collections;

[RequireComponent (typeof (BoxCollider2D))]
public class PlayerCollision : MonoBehaviour {
	const float skinWidth = .15f;
	public int horizontalRayCount = 4;
	public int verticalRayCount = 4;

    private LayerMask oneSideCollision;
    private LayerMask hardCollision;
    private int layerOneSide;
    private int layerHard;
    private int layerCombined;

    float horizontalRaySpacing;
	float verticalRaySpacing;

	BoxCollider2D collider;
	RaycastOrigins raycastOrigins;
	public CollisionInfo collisions;


	void Start() {
		collider = GetComponent<BoxCollider2D> ();
		CalculateRaySpacing ();
        oneSideCollision = CollisionManager.Instance.OneSideGound;
        hardCollision = CollisionManager.Instance.HardBlock;
        layerOneSide = oneSideCollision.value;
        layerHard = hardCollision.value;
    }

	public void Move(Vector3 velocity) {
		
		UpdateRaycastOrigins ();
		collisions.Reset ();

		if (velocity.x != 0) {
			HorizontalCollisions (ref velocity);
		}
		if (velocity.y != 0) {
			VerticalCollisions (ref velocity);
		}

		transform.Translate (velocity);
	}

    public void MoveIgnoreCollision(Vector3 velocity)
    {
        transform.Translate(velocity);
    }

    public void MoveIgnoreCollision(Vector3 velocity, Collider2D ignoreColliders)
    {
        if (ignoreColliders == null)
        {
            Move(velocity);
        }
        else
        {
            ignoreColliders.enabled = false;
            Move(velocity);
            ignoreColliders.enabled = true;
        }
    }

    void HorizontalCollisions(ref Vector3 velocity) {
		float directionX = Mathf.Sign (velocity.x);
		float rayLength = Mathf.Abs (velocity.x) + skinWidth;
        bool left = (directionX == -1);

        //LayerMask collisionMask = hardCollision;
        int testLayer = layerHard;

        for (int i = 0; i < horizontalRayCount; i ++) {
			Vector2 rayOrigin = left ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, testLayer);


            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength,Color.red);

			if (hit) {
				velocity.x = (hit.distance - skinWidth) * directionX;
				rayLength = hit.distance;

				collisions.left = left;
				collisions.right = !left;

                if (left)
                    collisions.leftTransform = hit.transform;
                else collisions.rightTransform = hit.transform;
            }
		}
	}

	void VerticalCollisions(ref Vector3 velocity) {
		float directionY = Mathf.Sign (velocity.y);
		float rayLength = Mathf.Abs (velocity.y) + skinWidth;
        bool below = (directionY == -1);
        int testLayer = directionY > 0 ? layerHard : (layerOneSide | layerHard);

		for (int i = 0; i < verticalRayCount; i ++) {
			Vector2 rayOrigin = below ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, testLayer);

			Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength,Color.red);

			if (hit) {
				velocity.y = (hit.distance - skinWidth) * directionY;
                //if (hit.distance - skinWidth < 0)
                //    velocity.y *= 2;
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
    }

	void UpdateRaycastOrigins() {
		Bounds bounds = collider.bounds;
		bounds.Expand (skinWidth * -2);

		raycastOrigins.bottomLeft = new Vector2 (bounds.min.x, bounds.min.y);
		raycastOrigins.bottomRight = new Vector2 (bounds.max.x, bounds.min.y);
		raycastOrigins.topLeft = new Vector2 (bounds.min.x, bounds.max.y);
		raycastOrigins.topRight = new Vector2 (bounds.max.x, bounds.max.y);
	}

	void CalculateRaySpacing() {
		Bounds bounds = collider.bounds;
		bounds.Expand (skinWidth * -2);

		horizontalRayCount = Mathf.Clamp (horizontalRayCount, 2, int.MaxValue);
		verticalRayCount = Mathf.Clamp (verticalRayCount, 2, int.MaxValue);

		horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
		verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
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

	struct RaycastOrigins {
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	}

	public struct CollisionInfo {
		public bool above, below;
		public bool left, right;
        public float tiltAngle;

        public Transform aboveTransform, belowTransform, leftTransform, rightTransform;
        public Collider2D belowCollider;

		public void Reset() {
			above = below = false;
			left = right = false;
            aboveTransform = belowTransform = leftTransform = rightTransform = null;
            belowCollider = null;
            tiltAngle = 0;
        }
	}

}


