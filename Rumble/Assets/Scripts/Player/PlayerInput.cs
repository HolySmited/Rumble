using UnityEngine;

/// <summary>
/// This class checks for input and delegates function between the other player scripts.
/// </summary>

[RequireComponent (typeof (PlayerController))]
[RequireComponent (typeof (PlayerStats))]
[RequireComponent (typeof (PlayerMovement))]
[RequireComponent (typeof (PlayerEquipment))]
public class PlayerInput : MonoBehaviour
{
    private string joystickName;
    private PlayerController playerController;
    private PlayerStats playerStats;
    private PlayerMovement playerMovement;
    private PlayerEquipment playerEquipment;

    private void Start ()
	{
        playerController = GetComponent<PlayerController>();
        playerStats = GetComponent<PlayerStats>();
        playerMovement = GetComponent<PlayerMovement>();
        playerEquipment = GetComponent<PlayerEquipment>();

        joystickName = playerStats.GetControllerName();
    }

    private void Update()
    {
        //Set the movement information
        playerMovement.SetTargetZVelocity(Input.GetAxisRaw(joystickName + "LeftHoriz"));

        //Check for veritcal collisions
        playerMovement.CheckForVerticalCollisions();

        //Check for a jump command
        if (Input.GetButtonDown(joystickName + "AButton"))
            //Jump
            playerMovement.Jump();

        //Aim the current weapon
        playerController.Aim(Input.GetAxis(joystickName + "RightHoriz"), Input.GetAxis(joystickName + "RightVert"));

        //Check for a reload command
        if (Input.GetButtonDown(joystickName + "XButton"))
            //Reload
            playerEquipment.currentWeapon.ReloadCurrentWeapon();

        //Check for a cycle command
        if (Input.GetButtonDown(joystickName + "YButton"))
            //Cycle
            playerEquipment.CycleWeapons();

        //Check for a swap command
        if (Input.GetButtonDown(joystickName + "BButton"))
            //Swap weapons
            playerEquipment.SwapWeapons();

        //Check for a shoot command
        if (Input.GetAxisRaw(joystickName + "RT") > 0.5f)
            playerEquipment.currentWeapon.Shoot(playerStats);
        //Do a special check for GaussCannon charge time reset
        else
        {
            if (playerEquipment.currentWeapon is GaussCannon)
                playerEquipment.currentWeapon.GetComponent<GaussCannon>().timeSpentCharging = 0f;
        }
    }
}
