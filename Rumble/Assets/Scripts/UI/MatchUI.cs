using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class handles populating the game UI with all of the necessary information.
/// </summary>

public class MatchUI : MonoBehaviour
{
    //Text objects
    public Text team1Score;
    public Text team2Score;
    public Text player1HUD;
    public Text player2HUD;
    public Text player3HUD;
    public Text player4HUD;

    //Player stats
    private PlayerStats player1Stats;
    private PlayerStats player2Stats;
    private PlayerStats player3Stats;
    private PlayerStats player4Stats;

    //Player equipment
    private PlayerEquipment player1Equipment;
    private PlayerEquipment player2Equipment;
    private PlayerEquipment player3Equipment;
    private PlayerEquipment player4Equipment;

    private void Start()
    {
        PlayerManager playerMan = GameObject.FindObjectOfType<PlayerManager>();

        player1Stats = playerMan.players[0].GetComponent<PlayerStats>();
        //player2Stats = playerMan.players[1].GetComponent<PlayerStats>();
        //player3Stats = playerMan.players[2].GetComponent<PlayerStats>();
        //player4Stats = playerMan.players[3].GetComponent<PlayerStats>();

        player1Equipment = playerMan.players[0].GetComponent<PlayerEquipment>();
        //player2Equipment = playerMan.players[0].GetComponent<PlayerEquipment>();
        //player3Equipment = playerMan.players[0].GetComponent<PlayerEquipment>();
        //player4Equipment = playerMan.players[0].GetComponent<PlayerEquipment>();
    }

    private void Update ()
	{
        //team1Score.text = "Blue team: " + (player1Stats.kills + player2Stats.kills);
        //team2Score.text = "Red team: " + (player3Stats.kills + player4Stats.kills);

        player1HUD.text = "Health: " + player1Stats.currentHealth +
            "\nArmor: " + player1Stats.currentArmor +
            "\nAmmo: " + player1Equipment.currentWeapon.ammoInClip + "/" + player1Equipment.currentWeapon.ammoInReserve;

        /*player2HUD.text = "Health: " + player2Stats.currentHealth +
            "\nArmor: " + player2Stats.currentArmor +
            "\nAmmo: " + player2Equipment.currentWeapon.ammoInClip + "/" + player2Equipment.currentWeapon.ammoInReserve;*/

        /*player3HUD.text = "Health: " + player3Stats.curentHealth +
            "\nArmor: " + player03Stats.currentArmor +
            "\nAmmo: " + player3Equipment.currentWeapon.ammoInClip + "/" + player3Equipment.currentWeapon.ammoInReserve;*/

        /*player4HUD.text = "Health: " + player4Stats.currentHealth +
            "\nArmor: " + player4Stats.currentArmor +
            "\nAmmo: " + player4Equipment.currentWeapon.ammoInClip + "/" + player4Equipment.currentWeapon.ammoInReserve;*/
    }
}
