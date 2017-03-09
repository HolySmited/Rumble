using UnityEngine;

/// <summary>
/// Class that handles player functionality not categorized by any other
/// player script.
/// </summary>

public class PlayerController : MonoBehaviour
{
    //Aims the player's currently equipped gun in an 8-point system:
    //Left, right, up, down, and the four diagonals in between
    public void Aim(float xInput, float yInput)
    {
        //Use a buffer zone around the up and down directions
        if (xInput < 0.15f && xInput > -0.15f)
        {
            //Down direction
            if (yInput < 0)
                gameObject.transform.GetChild(0).transform.eulerAngles = new Vector3(270, 0, 0);
            //Up direction
            else if (yInput > 0)
                gameObject.transform.GetChild(0).transform.eulerAngles = new Vector3(90, 0, 0);
        }
        //Either down-left, left, or up-left
        else if (xInput < 0)
        {
            //Use a buffer zone around the left direction
            if (yInput < 0.15f && yInput > -0.15f)
                gameObject.transform.GetChild(0).transform.eulerAngles = new Vector3(180, 0, 0);
            //Down-left
            else if (yInput < 0)
                gameObject.transform.GetChild(0).transform.eulerAngles = new Vector3(225, 0, 0);
            //Up-left
            else if (yInput > 0)
                gameObject.transform.GetChild(0).transform.eulerAngles = new Vector3(135, 0, 0);
        }
        //Either down-right, right, or up-right
        else if (xInput > 0)
        {
            //Use a buffer zone around the right direction
            if (yInput < 0.15f && yInput > -0.15f)
                gameObject.transform.GetChild(0).transform.eulerAngles = new Vector3(0, 0, 0);
            //Down-right
            else if (yInput < 0)
                gameObject.transform.GetChild(0).transform.eulerAngles = new Vector3(315, 0, 0);
            //Up-right
            else if (yInput > 0)
                gameObject.transform.GetChild(0).transform.eulerAngles = new Vector3(45, 0, 0);
        }
    }
}
