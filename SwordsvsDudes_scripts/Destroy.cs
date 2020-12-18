using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]

public class Destroy : MonoBehaviour {
    public TMP_Text tM;
    public float timer1 = 15;

    private void Start()
    {
        timer1 -= Time.deltaTime;
    }

    private void Update()
    {
        if (timer1 < 1)
        {
            Destroy(tM);
        }
    }


}
