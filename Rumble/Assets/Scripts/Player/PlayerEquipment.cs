using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class stores all equipment a player is carrying with them, and all
/// functions necessary to interact with them in terms of management.
/// </summary>

public class PlayerEquipment : MonoBehaviour
{
    //List of guns
    public List<Gun> guns = new List<Gun>();
    //Max weapons
    public int maxWeapons;
    //Current wepon
    public Gun currentWeapon;
    //List of weapons in range; always added to the end, simulates a queue
    //while still supporting removal of specific items when they leave range
    private List<Gun> weaponsInRange = new List<Gun>();
    //Index of the current weapon    
    private int currentWeaponIndex = 0;

    //Initializes the player's inventory when spawning
    public void InitializeEquipment()
    {
        //Get the Gun script on the muzzle
        currentWeapon = transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Gun>();
        //Deactivate its collider
        currentWeapon.GetComponent<BoxCollider>().enabled = false;
        //Add it to the list of weapons
        guns.Add(currentWeapon);
    }

    //Swaps the current weapon to the next weapon
    public void CycleWeapons()
    {
        //If the player is not reloading and has a weapon to cycle to
        if (!currentWeapon.isReloading && guns.Count > 1)
        {
            //Increase the index
            currentWeaponIndex++;

            if (currentWeaponIndex >= guns.Count)
                currentWeaponIndex = 0;

            //Deactivate the old weapon and swap the current weapon
            currentWeapon.transform.parent.gameObject.SetActive(false);
            currentWeapon = guns[currentWeaponIndex];

            //Activate the new weapon
            currentWeapon.transform.parent.gameObject.SetActive(true);
        }
    }

    //Adds a weapon to the list of those in range or picks it up automatically if there is room
    public void AddWeaponInRange(Gun weapon)
    {
        if (guns.Count < maxWeapons)
            PickupWeapon(weapon);
        else
            weaponsInRange.Add(weapon);
    }

    //Removes a weapon from the list of those in range
    public void RemoveWeaponInRange(Gun weapon)
    {
        weaponsInRange.Remove(weapon);
    }

    //Sets the current weapon to a new weapon
    public void SwapWeapons()
    {
        //If there are weapons in range
        if (weaponsInRange.Count > 0)
        {
            //Iterate through them
            for (int i = 0; i < weaponsInRange.Count; i++)
            {
                //If a weapon is found that the player does not have
                if (!HasWeapon(weaponsInRange[i]))
                {
                    //Swap the weapons
                    Gun droppedWeapon = currentWeapon;
                    Gun newWeapon = weaponsInRange[i];

                    //Pickup the new weapon and enable it
                    currentWeapon = newWeapon;
                    currentWeapon.enabled = true;
                    //Disable its collider
                    currentWeapon.GetComponent<BoxCollider>().enabled = false;
                    guns[currentWeaponIndex] = newWeapon;
                    //Move the new weapon and child it to the player's aim pivot
                    newWeapon.transform.parent.SetParent(gameObject.transform.GetChild(0));
                    newWeapon.transform.parent.transform.localPosition = new Vector3(0, 0, 1);
                    //Remove the new weapon from the list of weapons in range
                    RemoveWeaponInRange(newWeapon);

                    //Drop the old weapon and set its parent to null, and disable it
                    droppedWeapon.transform.parent.SetParent(null);
                    droppedWeapon.enabled = false;
                    //Endable its box collider
                    droppedWeapon.GetComponent<BoxCollider>().enabled = true;
                    //Add the dropped weapon to the end of the list of available weapons
                    AddWeaponInRange(droppedWeapon);
                }
            } 
        }
    }

    //Automatically picks up a weapon
    private void PickupWeapon(Gun weapon)
    {
        //If a weapon is found that the player does not have
        if (!HasWeapon(weapon))
        {
            //Add the weapon and set its components to inactive
            guns.Add(weapon);
            weapon.enabled = true;
            weapon.GetComponent<BoxCollider>().enabled = false;
            //Move the new weapon and child it to the player's aim pivot
            weapon.transform.parent.SetParent(gameObject.transform.GetChild(0));
            weapon.transform.parent.transform.localPosition = new Vector3(0, 0, 1);

            //Set the weapon to inactive
            weapon.transform.parent.gameObject.SetActive(false);
        }
    }

    //Returns whether a player has a specific type of gun
    private bool HasWeapon(Gun type)
    {
        foreach (Gun gun in guns)
        {
            if (gun.Equals(type))
                return true;
        }

        return false;
    }
}
