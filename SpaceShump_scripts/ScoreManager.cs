using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class ScoreManager : MonoBehaviour
{
    public static int SCORE = 0;
    TMP_Text tmPro;

    // Use this for initialization
    void Start()
    {
        tmPro = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        tmPro.text = SCORE.ToString("N0");
    }
}

