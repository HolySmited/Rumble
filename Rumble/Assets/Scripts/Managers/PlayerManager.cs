using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class handles management of all players in a game as well as spawn logic.
/// </summary>

public class PlayerManager : MonoBehaviour
{
    //Player information
    public GameObject team1Prefab;
    public GameObject team2Prefab;
    public GameObject[] players;
    private string[] joystickNames;

    //Spawn information
    public List<GameObject> spawnPoints = new List<GameObject>();
    public List<GameObject> initialSpawnPoints = new List<GameObject>();
    
    private void Awake()
    {
        joystickNames = null;
        players = null;
        //Get the names of all controllers connected
        joystickNames = Input.GetJoystickNames();
        //Create an array to store the player prefabs
        players = new GameObject[4];

        //Iterate through the controllers, creating and intiializing a player for each one
        for (int joystickNumber = 0; joystickNumber < joystickNames.Length; joystickNumber++)
        {
            //Create player
            players[joystickNumber] = SpawnPlayer(joystickNumber);
            //Initialize player
            players[joystickNumber].GetComponent<PlayerStats>().InitializePlayer(joystickNumber);
        }
    }

    //Spawns a player at the beginning of the game
    public GameObject SpawnPlayer(int playerNum)
    {
        //Spawns the correct team based on the player's number
        if (playerNum < 2)
            return Instantiate(team1Prefab, InitialSpawn().transform.position, Quaternion.identity) as GameObject;
        else
            return Instantiate(team2Prefab, InitialSpawn().transform.position, Quaternion.identity) as GameObject;
    }

    //Respawns a player after death
    public void Respawn(PlayerStats respawningPlayer)
    {
        GameObject newSpawnPoint = SelectSpawn(respawningPlayer);

        respawningPlayer.transform.position = newSpawnPoint.transform.position;
        respawningPlayer.GetComponent<MeshRenderer>().enabled = true;
    }

    //Selects a spawn for a player based on ally and enemy proximity. prioritizing proximity to an ally over
    //distance from enemies
    public GameObject SelectSpawn(PlayerStats spawningPlayer)
    {
        //Stores the player's ally and enemies
        GameObject teammate;
        GameObject[] enemies = new GameObject[2];

        //Determines which player is what relation to the spawning player
        if (players[0] == spawningPlayer.gameObject)
        {
            teammate = players[1];
            enemies[0] = players[2];
            enemies[1] = players[3];
        }
        else if (players[1] == spawningPlayer.gameObject)
        {
            teammate = players[0];
            enemies[0] = players[2];
            enemies[1] = players[3];
        }
        else if (players[2] == spawningPlayer.gameObject)
        {
            teammate = players[3];
            enemies[0] = players[0];
            enemies[1] = players[1];
        }
        else
        {
            teammate = players[2];
            enemies[0] = players[0];
            enemies[1] = players[1];
        }

        //Checks to see if the player's ally is alive
        if (!CheckLife(teammate.GetComponent<PlayerStats>()))
            teammate = null;

        //Checks to see if the player's enemies are alive
        for (int i = 0; i < 2; i++)
        {
            if (!CheckLife(enemies[i].GetComponent<PlayerStats>()))
                enemies[i] = null;
        }

        //If the player's ally and enemies are dead, return a random spawn 
        if (!teammate && !(enemies[0] || enemies[1]))
            return spawnPoints[Random.Range(0, spawnPoints.Count)];
        else
        {
            //Assume the first spawn is the best spawn
            GameObject bestSpawn = spawnPoints[0];

            //Iterate through all spawn points
            foreach (GameObject spawnPoint in spawnPoints)
            {
                float shortestDistToTeammate = 1000f;
                float longestDistFromClosestEnemy = 0f;

                //Used for temporary storage of distance variables
                float dist = 1000f;
                float secondDist = 1000f;

                //If the player's ally is alive
                if (teammate)
                {
                    //Get the distance to the ally from the current spawn point
                    dist = DistanceToPlayer(spawnPoint, teammate);

                    //If this point is the closest to the ally so far, store it
                    if (dist < shortestDistToTeammate)
                    {
                        shortestDistToTeammate = dist;
                        bestSpawn = spawnPoint;
                    }

                    //Move on to the next point
                    continue;
                }
                //If the player's ally is dead and either enemy is alive
                else
                {
                    //If the first enemy is alive, get the distance from this spawn to them
                    if (enemies[0])
                        dist = DistanceToPlayer(spawnPoint, enemies[0]);
                    //Do the same for the second enemy if they are alive
                    if (enemies[1])
                        secondDist = DistanceToPlayer(spawnPoint, enemies[1]);
                    //Store the distance to the closest enemy
                    dist = dist > secondDist ? secondDist : dist;

                    //If that distance is farther away than any other point, store it
                    if (dist > longestDistFromClosestEnemy)
                    {
                        longestDistFromClosestEnemy = dist;
                        bestSpawn = spawnPoint;
                    }
                }
            }

            return bestSpawn;
        }
    }

    //Returns the first available initial spawn points (teams are spawned in order)
    private GameObject InitialSpawn()
    {
        //Get the first spawn and add it to the list of all spawn points and remove it from the intial spawns
        GameObject spawn = initialSpawnPoints[0];
        spawnPoints.Add(spawn);
        initialSpawnPoints.Remove(spawn);
        return spawn;
    }

    //Gets a distance to a player from a spawn point
    private float DistanceToPlayer(GameObject spawnPoint, GameObject player)
    {
        return Vector3.Distance(spawnPoint.transform.position, player.transform.position);
    }

    //Checks to see if a player is alive
    private bool CheckLife(PlayerStats player)
    {
        return player.currentHealth > 0 ? true : false;
    }
}
