using System.Collections;
using UnityEngine;

/// <summary>
/// This class holds all of the stats for a given player, as well as all of the
/// methods necessary to interact with those stats.
/// </summary>

public class PlayerStats : MonoBehaviour
{
    public PlayerStats lastPlayerToDamageMe;
    public int kills;
    public float respawnTime;
    public int maxHealth;
    public int maxArmor;
    public int currentHealth;
    public int currentArmor;
    public LayerMask teamLayer;
    private PlayerEquipment playerEquipment;
    private PlayerManager playerManager;
    private string joystickName;


    private void Awake()
    {
        playerEquipment = GetComponent<PlayerEquipment>();
        playerManager = FindObjectOfType<PlayerManager>();
        currentHealth = maxHealth;
        currentArmor = 0;
    }

    //Attempts to use whatever pickup was encountered
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ammo")
        {
            foreach (Gun gun in playerEquipment.guns)
            {
                if (gun.ammoInReserve != gun.reserveSize)
                    gun.AddAmmotoReserve(other.GetComponent<Pickup>().Use());
            }
        }
        else if (other.tag == "Health")
        {
            if (currentHealth != maxHealth)
                AddHealth(other.GetComponent<Pickup>().Use());
        }
        else if (other.tag == "Armor")
            if (currentArmor != maxArmor)
                AddArmor(other.GetComponent<Pickup>().Use());
    }

    //Populates their stats
    public void InitializePlayer(int _controllerID)
    {
        //Set the controller based on the controller ID assigned by the player manager
        switch (_controllerID)
        {
            case 0:
                joystickName = "J1";
                break;
            case 1:
                joystickName = "J2";
                break;
            case 2:
                joystickName = "J3";
                break;
            case 3:
                joystickName = "J4";
                break;
            default:
                break;
        }

        playerEquipment.InitializeEquipment();
    }

    //Returns the name of the controller
    public string GetControllerName()
    {
        return joystickName;
    }

    //Takes an indicated amount of damage, factoring in armor; has an optional paramter for damage
    //that goes straight to armor; also sets the last player to shoot this player
    public void TakeDamage(int damageAmount, PlayerStats shotByPlayer, int armorDamage = default(int))
    {
        currentHealth -= damageAmount;
        lastPlayerToDamageMe = shotByPlayer;

        if (currentHealth <= 0)
            Die();
    }

    private void AddHealth(int amount)
    {
        currentHealth += amount;

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }

    private void AddArmor(int amount)
    {
        currentArmor += amount;

        if (currentArmor > maxArmor)
            currentArmor = maxArmor;
    }

    //Called when the player dies
    private void Die()
    {
        //Add a kill to the player that killed this player
        if (lastPlayerToDamageMe != null)
            lastPlayerToDamageMe.kills++;

        //Disable the player
        gameObject.SetActive(false);
        //Begin the respawn process
        StartCoroutine("Respawn");
    }

    //Respawns the player
    private IEnumerator Respawn()
    {
        float timeSpentDead = 0f;

        //Wait the requisite amount of time
        while (timeSpentDead < respawnTime)
        {
            timeSpentDead += Time.deltaTime;

            yield return null;
        }

        //Reste all stats
        ResetStats();

        //Respawn the player
        playerManager.Respawn(this);
    }

    //Reset stats to their starting values
    private void ResetStats()
    {
        currentHealth = maxHealth;
        currentArmor = 0;
        lastPlayerToDamageMe = null;
    }
}
