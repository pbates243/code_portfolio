using UnityEngine;
public class DestroyTimer : MonoBehaviour {
    public float time = 1f;
    private void FixedUpdate ()
    {
        time -= Time.deltaTime;
        if(time <= 0)
        {
            Destroy(gameObject);
        }
    }
}
