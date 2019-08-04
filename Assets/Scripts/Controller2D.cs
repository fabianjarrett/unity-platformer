using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Controller2D : MonoBehaviour
{
    const float skinWidth = .015f;

    public LayerMask collisionMask;
    [Range(2, 32)] public int horizontalRayCount = 4;
    [Range(2, 32)] public int verticalRayCount = 4;

    float horizontalRaySpacing;
    float verticalRaySpacing;

    Bounds innerBounds;
    BoxCollider2D boxCollider;
    RaycastOrigins raycastOrigins;
    public CollisionData collisions;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        UpdateInnerBounds();
        CalcuateRaySpacing();
    }

    public void Move(Vector2 moveAmount)
    {
        UpdateInnerBounds();
        UpdateRaycastOrigins();
        collisions.Reset();

        if (moveAmount.x != 0)
        {
            HorizontalCollisions(ref moveAmount);
        }
        if(moveAmount.y != 0)
        {
            VerticalCollisions(ref moveAmount);
        }

        transform.Translate(moveAmount);
    }

    void HorizontalCollisions(ref Vector2 moveAmount)
    {
        float directionX = Mathf.Sign(moveAmount.x);
        float rayLength = Mathf.Abs(moveAmount.x) + skinWidth;

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = directionX < 0 ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * horizontalRaySpacing * i;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.blue);

            if (hit)
            {
                moveAmount.x = (hit.distance - skinWidth) * directionX;
                rayLength = hit.distance;

                collisions.left = directionX < 0;
                collisions.right = directionX > 0;
            }
        }
    }

    void VerticalCollisions(ref Vector2 moveAmount)
    {
        float directionY = Mathf.Sign(moveAmount.y);
        float rayLength = Mathf.Abs(moveAmount.y) + skinWidth;

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = directionY < 0 ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + moveAmount.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

            if(hit)
            {
                moveAmount.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance;

                collisions.below = directionY < 0;
                collisions.above = directionY > 0;
            }
        }
    }

    void UpdateInnerBounds()
    {
        innerBounds = boxCollider.bounds;
        innerBounds.Expand(skinWidth * -2);
    }

    void UpdateRaycastOrigins()
    {
        raycastOrigins.bottomLeft = new Vector2(innerBounds.min.x, innerBounds.min.y);
        raycastOrigins.bottomRight = new Vector2(innerBounds.max.x, innerBounds.min.y);
        raycastOrigins.topLeft = new Vector2(innerBounds.min.x, innerBounds.max.y);
        raycastOrigins.topRight = new Vector2(innerBounds.max.x, innerBounds.max.y);
    }

    void CalcuateRaySpacing()
    {
        horizontalRaySpacing = innerBounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = innerBounds.size.x / (verticalRayCount - 1);
    }

    struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

    public struct CollisionData
    {
        public bool above, below;
        public bool left, right;

        public void Reset()
        {
            above = below = false;
            left = right = false;
        }
    }
}
