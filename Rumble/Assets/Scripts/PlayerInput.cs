using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    #region Public
    //Player movement speed
    public float moveSpeed;
    public float jumpHeight;
    public float timeToApex;
    public float accelerationTimeAir;
    public float accelerationTimeGround;
    #endregion

    #region Private
    //Name of this player's controller, based on controller number
    private string joystickName;
    private PlayerController controller;
    private Vector3 velocity;
    private float gravity;
    private float jumpVelocity;
    private float velocityZSmoothing;
    #endregion

    #region Serialized
    //If the player has a double jump remaining
    [SerializeField] private bool hasDoubleJump = true;
    #endregion

    private void Start ()
	{
        controller = GetComponent<PlayerController>();
        //Initialize the controller name for input detection
        joystickName = GetComponent<PlayerStats>().GetControllerName();
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToApex;
    }

    private void Update()
    {
        if (controller.collisions.above || controller.collisions.below)
            velocity.y = 0;

        //Check for a jump command
        if (Input.GetButtonDown(joystickName + "AButton"))
            //If the player can jump, allow them to jump
            if (CheckJump())
                Jump();

        if (!GetComponent<PlayerStats>().currentWeapon.isReloading)
        {
            //Under test
            if (Input.GetButtonDown(joystickName + "YButton"))
                controller.SwapWeapons();
        }

        //Under test
        if (Input.GetButtonDown(joystickName + "BButton"))
            controller.PickUpWeapon();

        float targetZVelocity = Input.GetAxisRaw(joystickName + "LeftHoriz") * moveSpeed;

        if (controller.collisions.below)
            velocity.z = Mathf.SmoothDamp(velocity.z, targetZVelocity, ref velocityZSmoothing, accelerationTimeGround);
        else
            velocity.z = Mathf.SmoothDamp(velocity.z, targetZVelocity, ref velocityZSmoothing, accelerationTimeAir);

        //Under test
        controller.Aim(Input.GetAxis(joystickName + "RightHoriz"), Input.GetAxis(joystickName + "RightVert"));
    }

    private void FixedUpdate ()
	{
        if (hasDoubleJump == false && controller.collisions.below)
            hasDoubleJump = true;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
	}

    //Adds jumping to velocity
    private void Jump()
    {
        //Begin the jump
        velocity.y = jumpVelocity;

        //If the player was in the air when they jumped
        if (!controller.collisions.below)
            hasDoubleJump = false;
    }

    //Checks to see if the player has a jump available
    private bool CheckJump()
    {
        //If the on the ground, the player can jump
        if (controller.collisions.below)
            return true;
        //If the player is in the air and still has a double jump, they can jump
        else if (hasDoubleJump)
            return true;
        //If the player is in the air and has no double jump, they cannot jump
        else
            return false;
    }
}
