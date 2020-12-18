using UnityEngine;
using System.Collections;

public class CheckPoint1 : MonoBehaviour
{
    private GameController1 gm;

    private void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameController1>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            gm.lastCheckPointPos = transform.position;
        }
    }



}

