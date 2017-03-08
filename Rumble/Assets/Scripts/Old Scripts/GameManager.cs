using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private const float MATCH_LENGTH = 60 * 5;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            if (Input.GetButtonDown("J1AButton"))
                SceneManager.LoadScene(1);

            if (Input.GetButtonDown("J1BButton"))
                Application.Quit();
        }
        else if (SceneManager.GetActiveScene().name == "2v2")
        {
            if (Time.timeSinceLevelLoad >= MATCH_LENGTH)
                SceneManager.LoadScene(0);
        }
    }
}
