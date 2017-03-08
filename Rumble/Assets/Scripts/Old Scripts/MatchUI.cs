using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchUI : MonoBehaviour
{
    public PlayerStats player1;
    public PlayerStats player2;
    public PlayerStats player3;
    public PlayerStats player4;

    public Text team1Score;
    public Text team2Score;
    public Text player1HUD;
    public Text player2HUD;
    public Text player3HUD;
    public Text player4HUD;

    private void Start()
    {
        PlayerManager playerMan = GameObject.FindObjectOfType<PlayerManager>();

        player1 = playerMan.players[0].GetComponent<PlayerStats>();
        player2 = playerMan.players[1].GetComponent<PlayerStats>();
        //player3 = playerMan.players[2].GetComponent<PlayerStats>();
        //player4 = playerMan.players[3].GetComponent<PlayerStats>();
    }

    private void Update ()
	{
        team1Score.text = "Blue team: " + (player1.kills + player2.kills);
        //team2Score.text = "Red team: " + (player3.kills + player4.kills);

        player1HUD.text = "Health: " + player1.health +
            "\nArmor: " + player1.armor +
            "\nAmmo: " + player1.currentWeapon.ammoInClip + "/" + player1.currentWeapon.ammoReserve;

        player2HUD.text = "Health: " + player2.health +
            "\nArmor: " + player2.armor +
            "\nAmmo: " + player2.currentWeapon.ammoInClip + "/" + player2.currentWeapon.ammoReserve;

        /*player3HUD.text = "Health: " + player3.health +
            "\nArmor: " + player3.armor +
            "\nAmmo: " + player3.currentWeapon.ammoInClip + "/" + player3.currentWeapon.ammoReserve;

        player4HUD.text = "Health: " + player4.health +
            "\nArmor: " + player4.armor +
            "\nAmmo: " + player4.currentWeapon.ammoInClip + "/" + player4.currentWeapon.ammoReserve;*/
    }
}
