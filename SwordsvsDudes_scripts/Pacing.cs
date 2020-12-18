using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pacing : MonoBehaviour {

        public float speed = 1.0f;
        public float origX;

        // Use this for initialization
        void Start()
        {
            //Vector3 origPosition = transform.position;
            origX = transform.position.x;
        }

        // Update is called once per frame
        void Update()
        {
            transform.Translate(speed * Time.deltaTime, 0, 0);

            if (Mathf.Abs(origX - transform.position.x) > 5.0f)
            {
                speed *= -1.0f; //change direction
            }
        }
    }

