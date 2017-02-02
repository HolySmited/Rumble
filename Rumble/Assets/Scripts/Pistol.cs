using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Weapon
{
    /// <summary>
    /// This class represents the Pistol weapon, and inherits from the Weapon class. The Weapon class
    /// includes functionality shared by all weapons, including firing and reloading. This class serves
    /// as an easy way to store values that differ between weapons.
    /// </summary>

    #region Public

    #endregion

    #region Private
    
    #endregion

    #region Serialized

    #endregion

    private void Start()
    {
        //Get the player's controller name
        controllerName = transform.parent.parent.parent.GetComponent<PlayerStats>().GetControllerName();
        //Set the pistol variables
        ammoReserve = 90;
        maxTotalAmmo = 99;
        clipSize = 9;
        ammoInClip = 9;
        damage = 10;
        range = 100f;
        fireRate = 0.25f;
        reloadTime = 0.75f;
        timeSinceLastFire = fireRate;
        vfxTimer = 0.2f;
        isReloading = false;
    }
}
