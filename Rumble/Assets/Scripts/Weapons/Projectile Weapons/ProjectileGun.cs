using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is an abstract class that serves as the base for all projectile guns. It inherits
/// from Gun. This class stores all variables and methods common between projectile guns.
/// </summary>

public class ProjectileGun : Gun
{
    //Projectile shoot info
    public int numberOfProjectiles;
    public GameObject projectilePrefab;
    protected ProjectilePool projectilePool;

    protected override void Awake()
    {
        base.Awake();
        projectilePool = FindObjectOfType<ProjectilePool>();
    }

    //Fires the projectile, or projectiles, ignoring collision with the team of the player who fired it
    public override void Shoot(PlayerStats firingPlayer)
    {
        //If the gun is ready to fire again
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

                    //Get projectiles from the object pool
                    List<GameObject> projectiles = projectilePool.GetProjectiles(projectilePrefab, numberOfProjectiles);
                    //Iterate through the projectiles
                    foreach (GameObject projectile in projectiles)
                    {
                        //Move them
                        projectile.transform.position = transform.position;
                        //Rotate the object to face forwards
                        projectile.transform.rotation = Quaternion.LookRotation(transform.forward);
                        //Activate the projectile
                        projectile.GetComponent<ProjectileBehavior>().Activate(collisionMask, firingPlayer);
                    }
                }
            }
        }
    }
}
