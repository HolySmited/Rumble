using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Public
    public int horizRayCount;
    public int vertRayCount;
    public CollisionInfo collisions;
    #endregion

    #region Private
    private CapsuleCollider playerCollider;
    private RaycastOrigins raycastOrigins;
    private const float SKIN_WIDTH = 0.015f;
    private float horizRaySpacing;
    private float vertRaySpacing;
    private int collisionMask;
    private const int GROUND_LAYER = 8;
    #endregion

    #region Serialized
    
    #endregion

    private void Start()
    {
        playerCollider = GetComponent<CapsuleCollider>();
        CalculateRaySpacing();

        collisionMask = 1 << GROUND_LAYER;
    }

    public void Move(Vector3 velocity)
    {
        collisions.Reset();
        UpdateRaycastOrigins();

        if (velocity.z != 0)
            HorizontalCollisions(ref velocity);

        if (velocity.y != 0)
            VerticalCollisions(ref velocity);
        
        transform.Translate(velocity);
    }

    private void HorizontalCollisions(ref Vector3 velocity)
    {
        float directionZ = Mathf.Sign(velocity.z);
        float rayLength = Mathf.Abs(velocity.z) + SKIN_WIDTH;

        for (int i = 0; i < horizRayCount; i++)
        {
            Vector3 rayOrigin;

            if (directionZ == -1)
                rayOrigin = raycastOrigins.bottomLeft;
            else
                rayOrigin = raycastOrigins.bottomRight;

            rayOrigin += Vector3.up * (horizRaySpacing * i);

            RaycastHit hit;
            if (Physics.Raycast(rayOrigin, Vector3.forward * directionZ, out hit, rayLength, collisionMask))
            {
                velocity.z = (hit.distance - SKIN_WIDTH) * directionZ;
                rayLength = hit.distance;

                collisions.left = (directionZ == -1);
                collisions.right = (directionZ == 1);
            }

            Debug.DrawRay(rayOrigin, Vector3.forward * directionZ * rayLength, Color.red);
        }
    }

    private void VerticalCollisions(ref Vector3 velocity)
    {
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + SKIN_WIDTH;

        for (int i = 0; i < vertRayCount; i++)
        {
            Vector3 rayOrigin;

            if (directionY == -1)
                rayOrigin = raycastOrigins.bottomLeft;
            else
                rayOrigin = raycastOrigins.topLeft;

            rayOrigin += Vector3.forward * (vertRaySpacing * i + velocity.z);

            RaycastHit hit;
            if (Physics.Raycast(rayOrigin, Vector3.up * directionY, out hit, rayLength, collisionMask))
            {
                velocity.y = (hit.distance - SKIN_WIDTH) * directionY;
                rayLength = hit.distance;

                collisions.below = (directionY == -1);
                collisions.above = (directionY == 1);
            }

            Debug.DrawRay(rayOrigin, Vector3.up * directionY * rayLength, Color.red);
        }
    }

    private void UpdateRaycastOrigins()
    {
        Bounds bounds = playerCollider.bounds;
        bounds.Expand(SKIN_WIDTH * -1);

        raycastOrigins.bottomLeft = new Vector3(bounds.center.x, bounds.min.y, bounds.min.z);
        raycastOrigins.bottomRight = new Vector3(bounds.center.x, bounds.min.y, bounds.max.z);
        raycastOrigins.topLeft = new Vector3(bounds.center.x, bounds.max.y, bounds.min.z);
        raycastOrigins.topRight = new Vector3(bounds.center.x, bounds.max.y, bounds.max.z);
    }

    private void CalculateRaySpacing()
    {
        Bounds bounds = playerCollider.bounds;
        bounds.Expand(SKIN_WIDTH * -1);

        horizRayCount = Mathf.Clamp(horizRayCount, 2, int.MaxValue);
        vertRayCount = Mathf.Clamp(vertRayCount, 2, int.MaxValue);

        horizRaySpacing = bounds.size.y / (horizRayCount - 1);
        vertRaySpacing = bounds.size.z / (horizRayCount - 1);
    }

    public struct CollisionInfo
    {
        public bool above;
        public bool below;
        public bool left;
        public bool right;

        public void Reset()
        {
            above = false;
            below = false;
            left = false;
            right = false;
        }
    }

    private struct RaycastOrigins
    {
        public Vector3 topLeft;
        public Vector3 topRight;
        public Vector3 bottomLeft;
        public Vector3 bottomRight;
    }
}
