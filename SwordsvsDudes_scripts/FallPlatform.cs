using UnityEngine;

public class FallPlatform : MonoBehaviour
{

    // How long after this platform has been touched that it should be removed from the game,
    // this is so that platforms that have fallen off and can't be seen anymore are
    // removed to avoid taking up unnecsesary cpu/gpu
    public static float TimeBeforeClearing = 20f;

    [Header("Adjustable")]
    public float timeBeforeFalling = 1f;

    [Header("References")]
    public Rigidbody2D rb;


    private bool hasBeenTouched = false;
    private float touchTimer; // How long this platform has been touched for

    private void Start()
    {

        if (rb == null)
        {
            Debug.LogError("Failed to assign rigidbody2d on platform for falling script", gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        // If this has already been touched, some some CPU by not calculating anything further
        if(hasBeenTouched)
        {
            return;
        }

        if(col.transform.tag == "Player")
        {

            hasBeenTouched = true;
            SpriteRenderer rend = GetComponent<SpriteRenderer>();
            rend.color = new Color(159,11,11,255);
        }
    }

    private void Update()
    {
        if(hasBeenTouched)
        {
            touchTimer += Time.deltaTime;
            if(rb.bodyType == RigidbodyType2D.Kinematic && touchTimer >= timeBeforeFalling)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
            }

            if(touchTimer > TimeBeforeClearing)
            {
                Destroy(gameObject);
            }
        }
    }

}
