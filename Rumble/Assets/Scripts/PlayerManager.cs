using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    #region Public
    public GameObject team1Prefab;
    public GameObject team2Prefab;
    public List<GameObject> spawnPoints = new List<GameObject>();
    public List<GameObject> initialSpawnPoints = new List<GameObject>();
    public GameObject[] players;
    #endregion

    #region Private
    List<GameObject> usedSpawnPoints = new List<GameObject>();
    #endregion

    #region Serialized
    [SerializeField] private string[] joystickNames; 
    #endregion

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

        foreach (GameObject spawnPoint in usedSpawnPoints)
        {
            spawnPoints.Add(spawnPoint);
        }

        usedSpawnPoints.Clear();
    }

    public GameObject SpawnPlayer(int playerNum)
    {
        if (playerNum < 2)
            return Instantiate(team1Prefab, InitialSpawn().transform.position, Quaternion.identity) as GameObject;
        else
            return Instantiate(team2Prefab, InitialSpawn().transform.position, Quaternion.identity) as GameObject;
    }

    private GameObject InitialSpawn()
    {
        GameObject spawn = initialSpawnPoints[0];
        usedSpawnPoints.Add(spawn);
        initialSpawnPoints.Remove(spawn);
        return spawn;
    }

    public GameObject SelectSpawn(PlayerStats spawningPlayer)
    {
        GameObject teammate;
        GameObject[] enemies = new GameObject[2];

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

        if (!CheckLife(teammate.GetComponent<PlayerStats>()))
            teammate = null;

        for (int i = 0; i < 2; i++)
        {
            if (!CheckLife(enemies[i].GetComponent<PlayerStats>()))
                enemies[i] = null;
        }


        if (!teammate && !(enemies[0] || enemies[1]))
            return spawnPoints[Random.Range(0, spawnPoints.Count)];
        else
        {
            GameObject bestSpawn = spawnPoints[0]; 

            foreach (GameObject spawnPoint in spawnPoints)
            {
                float shortestDistToTeammate = 1000f;
                float longestDistFromClosestEnemy = 0f;

                float dist = 1000f;
                float secondDist = 1000f;

                if (teammate)
                {
                    dist = DistanceToPlayer(spawnPoint, teammate);

                    if (dist < shortestDistToTeammate)
                    {
                        shortestDistToTeammate = dist;
                        bestSpawn = spawnPoint;
                    }

                    continue;
                }
                else if (enemies[0] || enemies[1])
                {
                    if (enemies[0])
                        dist = DistanceToPlayer(spawnPoint, enemies[0]);
                    if (enemies[1])
                        secondDist = DistanceToPlayer(spawnPoint, enemies[1]);
                    dist = dist > secondDist ? secondDist : dist;

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

    private float DistanceToPlayer(GameObject spawnPoint, GameObject player)
    {
        return Vector3.Distance(spawnPoint.transform.position, player.transform.position);
    }

    private bool CheckLife(PlayerStats player)
    {
        return player.health > 0 ? true : false;
    }
}
