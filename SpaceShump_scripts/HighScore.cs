using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(TMP_Text))]

public class HighScore : MonoBehaviour
{
    static public int score = 0;
    TMP_Text tmPro1;

    void Awake()
    {
        // if the player prefs high score already exists, read it 
        if (PlayerPrefs.HasKey("HighScore"))
        {
            score = PlayerPrefs.GetInt("HighScore");
        }

        //assign the high score to highscore
        PlayerPrefs.SetInt("HighScore", score);
    }

   


    // Update is called once per frame
    void Update()
    {
        tmPro1 = GetComponent<TMP_Text>();
        tmPro1.text = "High Score: " + score;

        // update the playerPrefs HighScore if necessary
        if (score > PlayerPrefs.GetInt("HighScore"))
        {
            PlayerPrefs.SetInt("HighScore", score);
        }
    }
}
