using UnityEngine;

/// <summary>
/// This class is an abstract class that serves as the base for all hitscan guns. It inherits
/// from Gun. This class stores all variables and methods common between hitscan guns.
/// </summary>

[RequireComponent (typeof (LineRenderer))]
public abstract class HitscanGun : Gun
{
    //Hitscan shoot info
    public float range;
    protected Ray shootRay;
    protected RaycastHit shootHit;

    //Hitscan effect system info
    protected LineRenderer bulletTracer;

    //Gets references to hitscan specific effect systems
    protected override void Awake()
    {
        bulletTracer = GetComponent<LineRenderer>();
        base.Awake();
    }

    //Disables VFX specific to hitscan weapons
    protected override void DisableEffects()
    {
        bulletTracer.enabled = false;
        base.DisableEffects();
    }

    //Fires the weapon, ignoring collision with the team of the player who fired
    public override void Shoot(PlayerStats firingPlayer)
    {
        //If the gun is ready to fire
        if (timeSinceLastFire >= fireRate)
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
                //If the gun is not reloading
                if (!isReloading)
                {
                    //Construct the layer mask (invert the layers these bullets will ignore)
                    collisionMask = ~(ignoreBulletsLayer | firingPlayer.teamLayer);
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
    }
}
