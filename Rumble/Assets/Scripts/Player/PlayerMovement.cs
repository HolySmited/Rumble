using UnityEngine;

/// <summary>
/// This class handles all movement and collisions for a player.
/// </summary>

public class PlayerMovement : MonoBehaviour
{
    //Player movement speed and jump stats
    public float targetZVelocity;
    public float moveSpeed;
    public float jumpHeight;
    public float timeToApex;
    public float accelerationTimeAir;
    public float accelerationTimeGround;
    private Vector3 velocity;
    private float gravity;
    private float jumpVelocity;
    private float velocityZSmoothing;
    private bool hasDoubleJump;

    //Player collision stats
    public LayerMask obstacleLayer;
    public int horizRayCount;
    public int vertRayCount;
    public CollisionInfo collisions;
    private CapsuleCollider playerCollider;
    private RaycastOrigins raycastOrigins;
    private const float SKIN_WIDTH = 0.015f;
    private float horizRaySpacing;
    private float vertRaySpacing;

    //Initialize variables
    private void Start()
    {
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToApex;
        hasDoubleJump = true;

        playerCollider = GetComponent<CapsuleCollider>();
        CalculateRaySpacing();
    }

    private void Update()
    {
        //If the player has landed from a double jump, give them the double jump back
        if (hasDoubleJump == false && collisions.below)
            hasDoubleJump = true;

        //Determine horizontal velocity based on whether the player is airborne or not
        if (collisions.below)
            velocity.z = Mathf.SmoothDamp(velocity.z, targetZVelocity, ref velocityZSmoothing, accelerationTimeGround);
        else
            velocity.z = Mathf.SmoothDamp(velocity.z, targetZVelocity, ref velocityZSmoothing, accelerationTimeAir);

        //Add gravity to our velocity and move based on the velocity
        velocity.y += gravity * Time.deltaTime;
        Move(velocity * Time.deltaTime);
    }

    //Sets targetZVelocity
    public void SetTargetZVelocity(float moveFactor)
    {
        targetZVelocity = moveFactor * moveSpeed;
    }

    public void CheckForVerticalCollisions()
    {
        //Check for vertical collisions
        if (collisions.above || collisions.below)
            velocity.y = 0;
    }

    //Adds jumping to velocity
    public void Jump()
    {
        if (CheckJump())
        {
            //Begin the jump
            velocity.y = jumpVelocity;

            //If the player was in the air when they jumped
            if (!collisions.below)
                hasDoubleJump = false;
        }
    }

    //Moves the player along a velocity
    public void Move(Vector3 velocity)
    {
        //Reset the collision data
        collisions.Reset();
        //Update the collision raycast origins
        UpdateRaycastOrigins();

        //Check for collisions
        if (velocity.z != 0)
            HorizontalCollisions(ref velocity);

        if (velocity.y != 0)
            VerticalCollisions(ref velocity);

        //Move with the velocity
        transform.Translate(velocity);
    }

    //Checks to see if the player has a jump available
    private bool CheckJump()
    {
        //If the on the ground, the player can jump
        if (collisions.below)
            return true;
        //If the player is in the air and still has a double jump, they can jump
        else if (hasDoubleJump)
            return true;
        //If the player is in the air and has no double jump, they cannot jump
        else
            return false;
    }

    //Check for horizontal collisions
    private void HorizontalCollisions(ref Vector3 velocity)
    {
        //Get the direction and magnitude of movement for this frame
        float directionZ = Mathf.Sign(velocity.z);
        float rayLength = Mathf.Abs(velocity.z) + SKIN_WIDTH;

        //Check all rays
        for (int i = 0; i < horizRayCount; i++)
        {
            //Set the raycast origin based on the direction of movement
            Vector3 rayOrigin;

            if (directionZ == -1)
                rayOrigin = raycastOrigins.bottomLeft;
            else
                rayOrigin = raycastOrigins.bottomRight;

            //Move the ray origin based on the number currently being checked
            rayOrigin += Vector3.up * (horizRaySpacing * i);

            //Check for a collision
            RaycastHit hit;
            if (Physics.Raycast(rayOrigin, Vector3.forward * directionZ, out hit, rayLength, obstacleLayer))
            {
                //If a collision happened, reset the velocity this frame to not go into the object
                velocity.z = (hit.distance - SKIN_WIDTH) * directionZ;
                //Set the ray length so we don't cast past the object on other rays
                rayLength = hit.distance;

                //Update the collision info
                collisions.left = (directionZ == -1);
                collisions.right = (directionZ == 1);
            }
        }
    }

    //This function works the same as HorizontalCollisions, but checks for vertical collisions
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
            if (Physics.Raycast(rayOrigin, Vector3.up * directionY, out hit, rayLength, obstacleLayer))
            {
                velocity.y = (hit.distance - SKIN_WIDTH) * directionY;
                rayLength = hit.distance;

                collisions.below = (directionY == -1);
                collisions.above = (directionY == 1);
            }
        }
    }

    //Updates the origins of raycasts after the player moves
    private void UpdateRaycastOrigins()
    {
        Bounds bounds = playerCollider.bounds;
        bounds.Expand(SKIN_WIDTH * -1);

        raycastOrigins.bottomLeft = new Vector3(bounds.center.x, bounds.min.y, bounds.min.z);
        raycastOrigins.bottomRight = new Vector3(bounds.center.x, bounds.min.y, bounds.max.z);
        raycastOrigins.topLeft = new Vector3(bounds.center.x, bounds.max.y, bounds.min.z);
        raycastOrigins.topRight = new Vector3(bounds.center.x, bounds.max.y, bounds.max.z);
    }

    //Calculates the spacing of rays based on the size of the player and the number of rays
    private void CalculateRaySpacing()
    {
        Bounds bounds = playerCollider.bounds;
        bounds.Expand(SKIN_WIDTH * -1);

        horizRayCount = Mathf.Clamp(horizRayCount, 2, int.MaxValue);
        vertRayCount = Mathf.Clamp(vertRayCount, 2, int.MaxValue);

        horizRaySpacing = bounds.size.y / (horizRayCount - 1);
        vertRaySpacing = bounds.size.z / (horizRayCount - 1);
    }

    //Stores info about where collisions occur
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

    //Stores all of the racyast origins
    private struct RaycastOrigins
    {
        public Vector3 topLeft;
        public Vector3 topRight;
        public Vector3 bottomLeft;
        public Vector3 bottomRight;
    }
}
