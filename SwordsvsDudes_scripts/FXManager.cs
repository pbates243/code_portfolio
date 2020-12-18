using UnityEngine;

public class FXManager : MonoBehaviour {

    public GameObject Sound3D;
    public GameObject Sound2D;

    public void Create3DSound (AudioClip sound, Vector3 position)
    {
        GameObject NewSound = Instantiate(Sound3D, position, Quaternion.identity, null);
        AudioSource aud = NewSound.GetComponent<AudioSource>();
        aud.clip = sound;
        aud.Play();
    }
    public void Create2DSound (AudioClip sound)
    {
        GameObject NewSound = Instantiate(Sound2D);
        AudioSource aud = NewSound.GetComponent<AudioSource>();
        aud.clip = sound;
        aud.Play();
    }
    public void Create2DSound (AudioClip sound, float volume)
    {
        GameObject NewSound = Instantiate(Sound2D);
        AudioSource aud = NewSound.GetComponent<AudioSource>();
        aud.clip = sound;
        aud.volume = volume;
        aud.Play();
    }
}
