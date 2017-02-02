using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    #region Public
    public GameObject playerPrefab;
    public List<GameObject> spawnPoints = new List<GameObject>();
    #endregion

    #region Private

    #endregion

    #region Serialized
    [SerializeField] private string[] joystickNames;
    [SerializeField] private GameObject[] players;
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
            players[joystickNumber] = SpawnPlayer();
            //Initialize player
            players[joystickNumber].GetComponent<PlayerStats>().InitializePlayer(joystickNumber);
        }
    }

    private GameObject SpawnPlayer()
    {
        return Instantiate(playerPrefab, SelectSpawn().transform.position, Quaternion.identity) as GameObject;
    }

    private GameObject SelectSpawn()
    {
        int spawnIndex = (int)Random.Range(0, spawnPoints.Count - 0.1f);
        GameObject spawn = spawnPoints[spawnIndex];
        spawnPoints.Remove(spawn);
        return spawn;
    }
}
