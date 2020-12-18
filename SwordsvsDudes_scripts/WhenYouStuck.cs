using UnityEngine;
using UnityEngine.SceneManagement;

public class WhenYouStuck : MonoBehaviour
{
    public GameObject player;
    public Transform respawnPoint;
    public int currentLevelBuildIndex = 1;

    public void RespawnPlayer()
    {
        // When a player needs to respawn we should reset the scene
        SceneManager.LoadScene(currentLevelBuildIndex);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            currentLevelBuildIndex += 1;}
    }

}

