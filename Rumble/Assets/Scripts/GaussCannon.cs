using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaussCannon : Weapon
{
    private float chargeTime;
    private float timeSpentCharging;
       
    protected override void Start()
    {
        ammoReserve = 20;
        maxTotalAmmo = 24;
        clipSize = 4;
        ammoInClip = 4;
        damage = 8;
        range = 100f;
        fireRate = 1.5f;
        chargeTime = 0.5f;
        timeSpentCharging = 0f;
        reloadTime = 1f;
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
                    if (ammoInClip < 0)
                    {
                        timeSinceLastFire = 0f;
                        gunAudio.clip = emptyShot;
                        gunAudio.Play();
                    }
                    else
                    {
                        timeSpentCharging += Time.deltaTime;

                        //If the player has ammo in the clip, fire
                        if (ammoInClip > 0 && timeSpentCharging >= chargeTime)
                            Shoot();
                    }
                }
            }
            else if (Input.GetAxisRaw(controllerName + "RT") < 0.1)
                timeSpentCharging = 0f;

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
