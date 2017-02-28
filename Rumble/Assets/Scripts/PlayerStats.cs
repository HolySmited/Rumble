using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    #region Public
    public Weapon currentWeapon;
    public Weapon secondaryWeapon;
    public int numWeapons;
    public int team;
    public int teamMask;
    public PlayerStats lastPlayerToDamageMe;
    public int kills;
    public int health = 50;
    public int armor = 0;
    #endregion

    #region Private
    private PlayerManager playerMan;
    private float respawnTime = 5f;
    #endregion

    #region Serialized
    [SerializeField] private string joystickName;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int maxArmor = 100;
    #endregion

    private void Start()
    {
        playerMan = FindObjectOfType<PlayerManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ammo")
        {
            if (secondaryWeapon != null)
            {
                if (secondaryWeapon.ammoReserve + secondaryWeapon.ammoInClip != secondaryWeapon.maxTotalAmmo)
                    secondaryWeapon.AddAmmo(50);
            }

            if (currentWeapon.ammoReserve + currentWeapon.ammoInClip != currentWeapon.maxTotalAmmo)
            {
                currentWeapon.AddAmmo(50);
                Destroy(other.gameObject);
                return;
            }
            else
                return;
        }
        else if (other.tag == "Health")
        { 
            if (health != maxHealth)
            {
                AddHealth(50);
                Destroy(other.gameObject);
                return;
            }
            else
                return;
        }
        else if (other.tag == "Armor")
            if (armor != maxArmor)
            {
                AddArmor(50);
                Destroy(other.gameObject);
                return;
            }
            else
                return;
    }

    //Spawns the player and populates their stats
    public void InitializePlayer(int _controllerID)
    {
        //Set the controller based on the controller ID assigned by the player manager
        switch (_controllerID)
        {
            case 0:
                joystickName = "J1";
                team = 1;
                teamMask = 10;
                break;
            case 1:
                joystickName = "J2";
                team = 1;
                teamMask = 10;
                break;
            case 2:
                joystickName = "J3";
                team = 2;
                teamMask = 9;
                break;
            case 3:
                joystickName = "J4";
                team = 2;
                teamMask = 9;
                break;
            default:
                break;
        }

        currentWeapon = transform.GetChild(0).GetChild(0).GetChild(0).gameObject.GetComponent<Weapon>();
        currentWeapon.controllerName = joystickName;
        currentWeapon.collisionMask = (1 << teamMask) | 1 << 8;
        currentWeapon.isPickedUp = true;
        secondaryWeapon = null;
        numWeapons = 1;
    }

    //Returns the name of the controller
    public string GetControllerName()
    {
        return joystickName;
    }

    //Under test
    public void TakeDamage(int damageAmount, int armorDamage = default(int))
    {
        //Add armor values
        health -= damageAmount;

        if (health <= 0)
            Die();
    }

    private void AddHealth(int amount)
    {
        health += amount;

        if (health > maxHealth)
            health = maxHealth;
    }

    private void AddArmor(int amount)
    {
        armor += amount;

        if (armor > maxArmor)
            armor = maxArmor;
    }

    //Under test
    private void Die()
    {
        if (lastPlayerToDamageMe != null)
            lastPlayerToDamageMe.kills++;

        //Drop weapons and grenades

        gameObject.GetComponent<MeshRenderer>().enabled = false;

        StartCoroutine("Respawn");
    }

    //Under test
    private IEnumerator Respawn()
    {
        float timeSpentDead = 0f;

        while (timeSpentDead < respawnTime)
        {
            timeSpentDead += Time.deltaTime;

            yield return null;
        }

        ResetStats();

        GameObject spawnPoint = playerMan.SelectSpawn(this);

        gameObject.transform.position = spawnPoint.transform.position;
        gameObject.GetComponent<MeshRenderer>().enabled = true;
    }

    private void ResetStats()
    {
        health = 50;
        armor = 0;
        lastPlayerToDamageMe = null;
    }
}
