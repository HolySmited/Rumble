using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    #region Public

    #endregion

    #region Private
    private Text playerStatText; //Testing purposes only
    private GameObject primaryWeapon;
    private GameObject secondaryWeapon;
    #endregion

    #region Serialized
    [SerializeField] private string joystickName;
    [SerializeField] private int health = 50;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int armor = 0;
    [SerializeField] private int maxArmor = 100;
    #endregion

    private void Start()
    {
        playerStatText = GameObject.Find("Player Stat Text").GetComponent<Text>(); //Testing purposes only
    }

    private void Update()
    {
        playerStatText.text = "Health: " + health + "\nArmor: " + armor;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ammo")
        {
            Weapon weaponStats = primaryWeapon.transform.GetChild(0).GetComponent<Weapon>();

            if (weaponStats.ammoReserve + weaponStats.ammoInClip != weaponStats.maxTotalAmmo)
            {
                weaponStats.AddAmmo(50);
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

        primaryWeapon = transform.GetChild(0).GetChild(0).gameObject;
        secondaryWeapon = null;
    }

    //Returns the name of the controller
    public string GetControllerName()
    {
        return joystickName;
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
}
