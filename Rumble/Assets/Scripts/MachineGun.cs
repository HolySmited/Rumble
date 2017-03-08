using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGun : Weapon
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

    protected override void Start()
    {
        //Set the pistol variables
        ammoReserve = 64;
        maxTotalAmmo = 80;
        clipSize = 16;
        ammoInClip = 16;
        damage = 4;
        range = 100f;
        fireRate = 0.15f;
        reloadTime = 0.9f;
        timeSinceLastFire = fireRate;
        vfxTimer = 0.2f;
        isReloading = false;
        isPickedUp = false;

        gunLine = GetComponent<LineRenderer>();
    }

    protected override void Update()
    {
        if (isPickedUp)
        {
            //Increase the time since the last firing of the gun
            timeSinceLastFire += Time.deltaTime;

            //If the player holds down right trigger
            if (Input.GetAxisRaw(controllerName + "RT") > 0.5)
            {
                //If the player  is not reloading and is ready to fire again
                if (!isReloading && timeSinceLastFire >= fireRate)
                {
                    //If the player has ammo in the clip, fire
                    if (ammoInClip > 0)
                        Shoot();
                    //If the player has no ammo in the clip, reset the timeSinceLastFire and play an empty clip sound
                    else
                    {
                        timeSinceLastFire = 0f;
                        gunAudio.clip = emptyShot;
                        gunAudio.Play();
                    }
                }
            }

            //If the player presses the X button
            if (Input.GetButtonDown(controllerName + "XButton"))
            {
                //If they are missing ammo in their clip and have ammo left
                if (ammoInClip != clipSize && ammoReserve != 0)
                    StartCoroutine("Reload");
            }

            //If the time since last firing the gun is greater than the lifetime of the VFX
            if (timeSinceLastFire >= fireRate * vfxTimer)
                DisableEffects();
        }
    }

    //Fires the weapon and creates visual and audio effects
    protected override void Shoot()
    {
        //Reset the time since last firing the weapon
        timeSinceLastFire = 0f;
        //Reduce the ammo in the clip
        ammoInClip--;
        //Start the effects for firing the weapon
        gunAudio.clip = gunshot;
        gunAudio.Play();
        gunLight.enabled = true;
        //If the particles are still playing, stop the old particles and start new ones
        gunParticles.Stop();
        gunParticles.Play();
        gunLine.enabled = true;
        //Set the start position of the line renderer to the muzzle
        gunLine.SetPosition(0, transform.position);
        //Create the shootRay data
        shootRay.origin = transform.position;
        shootRay.direction = transform.forward;

        //If the shootRay hits something within range of the weapon on the shootable layer
        if (Physics.Raycast(shootRay, out shootHit, range, collisionMask))
        {
            //Set the end position of the line renderer to whatever was hit
            gunLine.SetPosition(1, shootHit.point);
            //Find the object that was hit
            GameObject objectHit = shootHit.collider.gameObject;
            //If it was another player
            if (objectHit.tag == "Player")
            {
                //Tell the player to take damage
                PlayerStats enemy = objectHit.GetComponent<PlayerStats>();
                enemy.lastPlayerToDamageMe = playerScript;
                enemy.TakeDamage(damage);
            }
        }
        //If the shootRay did not hit anything within range
        else
            //Set the end position of the line renderer to the maximum range of the weapon
            gunLine.SetPosition(1, shootRay.origin + shootRay.direction * range);
    }

    protected override void DisableEffects()
    {
        gunLine.enabled = false;
        gunLight.enabled = false;
    }
}
