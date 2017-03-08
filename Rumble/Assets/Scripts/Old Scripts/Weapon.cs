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
    public bool isPickedUp;
    //The string name of the player's controller
    public string controllerName;
    public int collisionMask;
    public bool isReloading;
    #endregion

    #region Private
    //The ray that tests for a hit when shooting
    protected Ray shootRay;
    //The hit info from shootRay
    protected RaycastHit shootHit;
    //References to the gun's visual  and audio effect systems
    protected ParticleSystem gunParticles;
    protected LineRenderer gunLine;
    protected AudioSource gunAudio;
    protected Light gunLight;
    public const int GROUND_LAYER = 8;
    //Layers that bullets cannot penetrate
    protected int groundMask;
    protected PlayerStats playerScript;
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
    #endregion

    protected void Awake()
    {
        //Get a reference to all necessary components
        gunParticles = GetComponent<ParticleSystem>();
        gunAudio = GetComponent<AudioSource>();
        gunLight = GetComponent<Light>();
        groundMask = 1 << GROUND_LAYER;
    }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {

    }

    protected void OnTriggerEnter(Collider other)
    {
        if (!isPickedUp)
        {
            if (other.tag == "Player")
            {
                playerScript = other.gameObject.GetComponent<PlayerStats>();

                if (playerScript.numWeapons == 0)
                {
                    playerScript.currentWeapon = this;
                    collisionMask = (1 << playerScript.teamMask) | groundMask;
                    controllerName = playerScript.GetControllerName();
                    isPickedUp = true;
                    gameObject.transform.parent.parent = other.gameObject.transform.GetChild(0);
                }
                else if (playerScript.numWeapons == 1)
                {
                    if (playerScript.currentWeapon.GetType() != this.GetType())
                    {
                        playerScript.secondaryWeapon = this;
                        collisionMask = (1 << playerScript.teamMask) | groundMask;
                        controllerName = playerScript.GetControllerName();
                        isPickedUp = true;
                        gameObject.transform.parent.transform.parent = other.gameObject.transform.GetChild(0);
                        gameObject.transform.parent.localPosition = new Vector3(0, 0, 1);
                        gameObject.transform.parent.gameObject.SetActive(false);
                        playerScript.numWeapons++;
                    }
                }
                else
                {
                    if (playerScript.currentWeapon.GetType() != this.GetType() && playerScript.secondaryWeapon.GetType() != this.GetType())
                    {
                        PlayerController playerCont = other.gameObject.GetComponent<PlayerController>();

                        playerCont.weaponAvailable = true;
                        playerCont.availableWeapon = gameObject.transform.parent.gameObject;
                    }
                }
            }
        }
    }

    protected void OnTriggerExit(Collider other)
    {
        if (!isPickedUp)
        {
            if (other.tag == "Player")
            {
                playerScript = other.gameObject.GetComponent<PlayerStats>();

                if (playerScript.currentWeapon.GetType() != this.GetType() && playerScript.secondaryWeapon.GetType() != this.GetType())
                {
                    PlayerController playerCont = other.gameObject.GetComponent<PlayerController>();

                    playerCont.weaponAvailable = false;
                    playerCont.availableWeapon = gameObject;
                }
            }
        }
    }

    public void AddAmmo(int amount)
    {
        ammoReserve += amount;

        if (ammoReserve + ammoInClip > maxTotalAmmo)
            ammoReserve = maxTotalAmmo - ammoInClip;
    }

    protected virtual void Shoot()
    {

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
    protected virtual void DisableEffects()
    {

    }
}
