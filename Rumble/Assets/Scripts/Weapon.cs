using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

abstract public class Weapon : MonoBehaviour
{
    /// <summary>
    /// The Weapon class is the superclass for all weapons in the game. It serves to store all
    /// shared functionality between weapons such as firing and reloading.
    /// </summary>
   
    #region Public
    //Gun audio clips
    public AudioClip gunshot;
    public AudioClip reload;
    public AudioClip emptyShot;
    public int ammoInClip;
    public int ammoReserve;
    public int maxTotalAmmo;
    #endregion

    #region Private
    //The string name of the player's controller
    protected string controllerName;
    //The ray that tests for a hit when shooting
    protected Ray shootRay;
    //The hit info from shootRay
    protected RaycastHit shootHit;
    //References to the gun's visual  and audio effect systems
    protected ParticleSystem gunParticles;
    protected LineRenderer gunLine;
    protected AudioSource gunAudio;
    protected Light gunLight;
    protected Text ammoText; //Testing purposes only
    //Indicates anything that is shootable (enemies, walls, ground, etc.)
    public const int PLAYER_LAYER = 9;
    public const int GROUND_LAYER = 8;
    //Layers that bullets cannot penetrate
    protected int playerMask;
    protected int groundMask;
    protected int collisionMask;
    #endregion

    #region Serialized
    //Various stats about the gun
    [SerializeField] protected int clipSize;
    [SerializeField] protected int damage;
    [SerializeField] protected float range;
    [SerializeField] protected float fireRate;
    [SerializeField] protected float reloadTime;
    [SerializeField] protected float timeSinceLastFire;
    [SerializeField] protected float vfxTimer;
    [SerializeField] protected bool isReloading;
    [SerializeField] protected bool isPickedUp;
    #endregion

    protected void Awake()
    {
        //Get a reference to all necessary components
        gunParticles = GetComponent<ParticleSystem>();
        gunLine = GetComponent<LineRenderer>();
        gunAudio = GetComponent<AudioSource>();
        gunLight = GetComponent<Light>();
        ammoText = GameObject.Find("Ammo Counter").GetComponent<Text>(); //Testing purposes only
        playerMask = 1 << PLAYER_LAYER;
        groundMask = 1 << GROUND_LAYER;
        collisionMask = playerMask | groundMask;

    }

    protected void Update()
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

        ammoText.text = "Ammo: " + ammoInClip + "/" + clipSize + "\nReserve: " + ammoReserve; //Testing purposes only
    }

    public void AddAmmo(int amount)
    {
        ammoReserve += amount;

        if (ammoReserve + ammoInClip > maxTotalAmmo)
            ammoReserve = maxTotalAmmo - ammoInClip;
    }

    //Fires the weapon and creates visual and audio effects
    protected void Shoot()
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
            //If it was an enemy
            if (objectHit.tag == "Enemy") //Will change after testing
            {
                //Tell the enemy to take damage
                EnemyTest script = objectHit.GetComponent<EnemyTest>(); //Will change after testing
                script.TakeDamage(damage); //Will change after testing
            }
        }
        //If the shootRay did not hit anything within range
        else
            //Set the end position of the line renderer to the maximum range of the weapon
            gunLine.SetPosition(1, shootRay.origin + shootRay.direction * range);
    }

    //A coroutine that reloads the weapon
    protected IEnumerator Reload()
    {
        //Play reload sound
        gunAudio.clip = reload;
        gunAudio.Play();
        //Set reloading to true so the player cannot fire while reloading
        isReloading = true;
        //Start the reload timer
        float timePassed = 0f;

        //While we are still under the reload time
        while (timePassed < reloadTime)
        {
            //Increase the reload timer
            timePassed += Time.deltaTime;

            //If we have reached the reload time
            if (timePassed >= reloadTime)
            {
                //Get the amount of ammo missing from the clip
                int missingAmmo = clipSize - ammoInClip;

                //If the player has enough ammo to reload
                if (missingAmmo <= ammoReserve)
                {
                    //Subtract the reload amount from the player's ammo reserve
                    ammoReserve -= missingAmmo;
                    //Add the reload amount to the player's clip
                    ammoInClip += missingAmmo;
                }
                //If the player does not have enough ammo to fully reload
                else
                {
                    //Add all reserve ammo to the clip and set the reserve to 0
                    ammoInClip += ammoReserve;
                    ammoReserve = 0;
                }
            }

            //Return
            yield return null;
        }

        //The player has reloaded
        isReloading = false;
    }

    //Disables persistent effects from firing the weapon
    protected void DisableEffects()
    {
        gunLine.enabled = false;
        gunLight.enabled = false;
    }
}
