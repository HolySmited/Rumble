using UnityEngine;

/// <summary>
/// This class represents the GaussCannon weapon, and inherits from the HitscanGun class.
/// This class serves as an easy way to store values that differ between individual weapons,
/// as well as implements its own Shoot method to support charging the weapon before firing.
/// </summary>

public class GaussCannon : HitscanGun
{
    //Variables specific to the gauss cannon
    public float chargeTime;
    public float timeSpentCharging;

    //Uses its own shoot method for charging the weapon
    public override void Shoot(PlayerStats firingPlayer)
    {
        //If the gun has no ammo
        if (ammoInClip == 0)
        {
            //Reset the time since the last fire
            timeSinceLastFire = 0f;
            //Play the empty clip sound
            gunAudio.PlayOneShot(outOfAmmo);
        }
        //If the gun has ammo
        else
        {
            //Increase the time spent charging
            timeSpentCharging += Time.deltaTime;

            //If the gun is not reloading and is ready to fire again
            if (!isReloading && timeSinceLastFire >= fireRate && timeSpentCharging >= chargeTime)
            {
                //Construct the layer mask (invert the layers these bullets will ignore)
                collisionMask = ~((1 << ignoreBulletsLayer) | (1 << firingPlayer.teamLayer));
                //Reset the time since last firing the weapon
                timeSinceLastFire = 0f;
                //Reduce the ammo in the clip
                ammoInClip--;
                //Start the effects for firing the weapon
                gunAudio.Play();
                gunLight.enabled = true;
                //If the particles are still playing, stop the old particles and start new ones
                shootParticles.Stop();
                shootParticles.Play();
                bulletTracer.enabled = true;
                //Set the start position of the line renderer to the muzzle
                bulletTracer.SetPosition(0, transform.position);
                //Create the shootRay data
                shootRay.origin = transform.position;
                shootRay.direction = transform.forward;

                //If the shootRay hits something within range of the weapon on the shootable layer
                if (Physics.Raycast(shootRay, out shootHit, range, collisionMask))
                {
                    //Set the end position of the line renderer to whatever was hit
                    bulletTracer.SetPosition(1, shootHit.point);
                    //Find the object that was hit
                    GameObject objectHit = shootHit.collider.gameObject;
                    //If it was another player
                    if (objectHit.tag == "Player")
                    {
                        //Tell the player to take damage and register the firing player as the last one to damage them
                        PlayerStats enemy = objectHit.GetComponent<PlayerStats>();
                        enemy.TakeDamage(damage, firingPlayer);
                    }
                }
                //If the shootRay did not hit anything within range
                else
                    //Set the end position of the line renderer to the maximum range of the weapon
                    bulletTracer.SetPosition(1, shootRay.origin + shootRay.direction * range);
            }
        }
    }

    //Used for resetting the charge
    public void ResetCharge()
    {
        timeSpentCharging = 0f;
    }
}
