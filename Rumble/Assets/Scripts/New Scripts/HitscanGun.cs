using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponenet (typeof (LineRenderer))]
public class HitscanGun : Gun
{
    //Hitscan shoot info
    protected Ray shootRay;
    protected RaycastHit shootHit;

    //Hitscan effect system info
    protected LineRenderer bulletTracer;

    protected override void Awake()
    {
        bulletTracer = GetComponent<LineRenderer>();
    }

    //Fires the weapon for the specified team
    protected override void Shoot(LayerMask firingTeam)
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
        shootParticles.Stop();
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
}
