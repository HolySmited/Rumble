using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is an abstract class that serves as the base for all types of guns. It has
/// two child classes, the HitscanGun class and the ProjectileGun class. This class stores
/// all variables and methods common between both types of gun.
/// </summary>

[RequireComponent (typeof (ParticleSystem))]
[RequireComponent (typeof (AudioSource))]
[RequireComponent (typeof (Light))]
public abstract class Gun : MonoBehaviour
{
    //Effects
    public AudioClip gunshot;
    public AudioClip reload;
    public AudioClip outOfAmmo;
    protected float vfxLifetime;

    //Effect systems
    protected ParticleSystem shootParticles;
    protected AudioSource gunAudio;
    protected Light gunLight;

    //Collision information
    protected LayerMask collisionLayer;

    //Gun stats
    public int ammoInClip;
    public int ammoInReserve;
    public bool isReloading;
    protected int clipSize;
    protected int reserveSize;
    protected int damage;
    protected float range;
    protected float fireRate;
    protected float reloadTime;
    protected float timeSinceLastFire;
    protected float effectLifeTime;

    protected virtual void Awake()
    {
        //Get references to the effect systems
        shootParticles = GetComponent<ParticleSystem>();
        gunAudio = GetComponent<AudioSource>();
        gunLight = GetComponent<Light>();
    }

    //Fires the weapon
    protected virtual void Shoot()
    {
        return;
    }

    //Disables VFX created by the gun
    protected virtual void DisableEffects()
    {
        gunLight.enabled = false;
        shootParticles.Stop();
        return;
    }

    //Adds ammo to the weapon's reserve
    public void AddAmmotoReserve(int amount)
    {
        //Add the amount to the reserves
        ammoInReserve += amount;

        //If the reserve amount exceeds the maximum, reset it to the maximum
        if (ammoInReserve > reserveSize)
            ammoInReserve = reserveSize;
    }

    protected void Update()
    {
        //Increase the time since the last firing of the gun
        timeSinceLastFire += Time.deltaTime;

        //If the time since last firing the gun is greater than the lifetime of the VFX
        if (timeSinceLastFire >= vfxLifetime)
            DisableEffects();
    }

    //Used for allowing players to pick up the weapon if it is on the ground
    protected void OnTriggerEnter(Collider other)
    {
        //If the other object is a player
        if (other.tag == "Player")
        {
            //Get a reference to their equipment script and tell them they can pick it up
            PlayerEquipment playerEquipment = other.gameObject.GetComponent<PlayerEquipment>();
            playerEquipment.AddWeaponInRange(this);            
        }
    }

    //Used for telling the player they can no longer pick up this weapon
    protected void OnTriggerExit(Collider other)
    {
        //If the other object is a plyer
        if (other.tag == "Player")
        {
            //Get a reference to their equipment script and tell them they can no longer pick it up
            PlayerEquipment playerEquipment = other.gameObject.GetComponent<PlayerEquipment>();
            playerEquipment.RemoveWeaponInRange(this);
        }
    }

    //Reloads the gun
    protected IEnumerator Reload()
    {
        //If the gun has a full clip, it cannot be reloaded; if the player has no ammo to reload with,
        //it cannot be reloaded
        if (ammoInClip == clipSize || ammoInReserve == 0)
            yield break;

        //Play reload sound
        gunAudio.clip = reload;
        gunAudio.Play();

        //Set reloading to true so the player cannot fire while reloading
        isReloading = true;
        //Start the reload timer
        float timeSpentReloading = 0f;

        //While the player is still under the reload time
        while (timeSpentReloading < reloadTime)
        {
            //Increase the reload timer
            timeSpentReloading += Time.deltaTime;

            //If we have not reached the reload time, exit the coroutine
            if (timeSpentReloading < reloadTime)
                yield return null;
            //If we have reached the reload time
            else
            {
                //Get the amount of ammo missing from the clip
                int missingAmmo = clipSize - ammoInClip;

                //If the player has enough ammo to fully reload
                if (missingAmmo <= ammoInReserve)
                {
                    //Subtract the reload amount from the player's ammo reserve
                    ammoInReserve -= missingAmmo;
                    //Add the reload amount to the player's clip
                    ammoInClip += missingAmmo;
                }
                //If the player does not have enough ammo to fully reload
                else
                {
                    //Add all reserve ammo to the clip and set the reserve to 0
                    ammoInClip += ammoInReserve;
                    ammoInReserve = 0;
                }
            }
        }
    }
}
