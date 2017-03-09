using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class functions as an object pool for all projectile types. It contains
/// a list of lists of GameObjects, with the inner lists representing each type
/// of projectile.
/// </summary>

public class ProjectilePool : MonoBehaviour
{
    //Stores all prjectile prefabs
    public List<GameObject> projectilePrefabs;
    //A list of all the object pools
    private List<List<GameObject>> projectileLists = new List<List<GameObject>>();

    private void Awake()
    {
        //Iterate through all prefabs
        for (int i = 0; i < projectilePrefabs.Count; i++)
        {
            //Add a new list for the prefab type
            projectileLists.Add(new List<GameObject>());
            //Instantiate one of the prefabs
            GameObject prefabClone = (GameObject)GameObject.Instantiate(projectilePrefabs[i]);
            //Add it to the list
            projectileLists[i].Add(prefabClone);
        }
    }

    //Gets a projectile from the appropriate object pool
    public List<GameObject> GetProjectiles(GameObject projectileRequested, int numRequested)
    {
        //Create a list of objects to return
        List<GameObject> projectiles = new List<GameObject>();

        //Get the index of the proper projectile type
        int indexOfProjectile = GetIndexOfProjectileType(projectileRequested);

        //If the projectile type was not found, log an error
        if (indexOfProjectile == -1)
        {
            Debug.LogError("Missing projectile prefab!");
            return null;
        }
        else
        {
            //If the list of projectiles is empty
            if (projectileLists[indexOfProjectile].Count == 0)
            {
                //Create the requested number of projectiles and add them to the list
                while (projectiles.Count < numRequested)
                {
                    GameObject newProjectile = (GameObject)GameObject.Instantiate(projectilePrefabs[indexOfProjectile]);
                    projectileLists[indexOfProjectile].Add(newProjectile);
                    projectiles.Add(newProjectile);
                }

                return projectiles;
            }
            //If the list has projectiles
            else
            {
                //Iterate through the projectiles
                for (int i = 0; i < projectileLists[indexOfProjectile].Count; i++)
                {
                    //If the current projectile is not active
                    if (!projectileLists[indexOfProjectile][i].activeInHierarchy)
                    {
                        //Add it to the list of requested items
                        projectiles.Add(projectileLists[indexOfProjectile][i]);

                        //If the requested number have been found, return them
                        if (projectiles.Count == numRequested)
                            return projectiles;
                    }
                }

                //After iterating, if more objects need to be created, create them
                while (projectiles.Count < numRequested)
                {
                    GameObject newProjectile = (GameObject)GameObject.Instantiate(projectilePrefabs[indexOfProjectile]);
                    projectileLists[indexOfProjectile].Add(newProjectile);
                    projectiles.Add(newProjectile);
                }

                return projectiles;
            }
        }
    }

    //Gets the index of a projectile within the lists of projectiles
    private int GetIndexOfProjectileType(GameObject projectile)
    {
        //Iterate through the lists
        for (int i = 0; i < projectileLists.Count; i++)
        {
            //If the correct type has been found, return its index
            if (projectileLists[i][0].GetComponent<ProjectileBehavior>().GetType() == projectile.GetComponent<ProjectileBehavior>().GetType())
                return i;
        }

        //If the type was not found, return an invalid index
        return -1;
    }
}
