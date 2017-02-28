using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Weapon
{
    public GameObject shotgunLine;

    private GameObject[] lines;
    private float fireAngleDegrees = 15f;
    private int numPellets = 7;

    protected override void Start()
    {
        //Set the pistol variables
        ammoReserve = 32;
        maxTotalAmmo = 40;
        clipSize = 8;
        ammoInClip = 8;
        damage = 2;
        range = 10f;
        fireRate = 0.5f;
        reloadTime = 0.9f;
        timeSinceLastFire = fireRate;
        vfxTimer = 0.2f;
        isReloading = false;
        isPickedUp = false;
        lines = new GameObject[numPellets];

        for (int i = 0; i < numPellets; i++)
        {
            lines[i] = Instantiate(shotgunLine, Vector3.zero, Quaternion.identity) as GameObject;
            lines[i].SetActive(false);
        }
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

        for (int i = 0; i < numPellets; i++)
        {
            lines[i].transform.position = transform.position;
            lines[i].transform.eulerAngles = new Vector3(Random.Range(-fireAngleDegrees, fireAngleDegrees), 0, 0);
            lines[i].SetActive(true);
            lines[i].GetComponent<LineRenderer>().enabled = true;
            lines[i].GetComponent<ShotgunLine>().OnActive(fireRate * vfxTimer, shootRay, shootHit, range, collisionMask, damage);
        }
    }

    protected override void DisableEffects()
    {
        gunLight.enabled = false;
    }
}
