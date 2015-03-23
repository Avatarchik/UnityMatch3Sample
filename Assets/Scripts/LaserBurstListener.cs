using UnityEngine;

public class LaserBurstListener : MonoBehaviour {

    private AudioSource audioSource;

	void Start ()
	{
	    audioSource = GetComponent<AudioSource>();
	}

    void OnEnable()
    {
        BoardRenderer.LaserShot += OnLaserShot;
    }

    void OnDisable()
    {
        BoardRenderer.LaserShot -= OnLaserShot;
    }

    private void OnLaserShot()
    {
        audioSource.Play();
    }
}
