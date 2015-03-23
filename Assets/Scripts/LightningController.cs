using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class LightningController : MonoBehaviour
{

    private float timer;
    private LightningAnimation[] animations;
    private AudioSource audioSource;
    private bool started;
    private float duration;

    void Awake ()
	{
	    animations = GetComponentsInChildren<LightningAnimation>();
	    audioSource = GetComponent<AudioSource>();
	}

    public void StartAnimation(float newDuration)
    {
        started = true;
        duration = newDuration;
        timer = 0;
        for (int i = 0; i < animations.Length; i++)
        {
            animations[i].StartAnimation();
        }
        audioSource.Play();
    }
	
	void Update ()
	{
	    if (!started)
	    {
	        return;
	    }

	    timer += Time.deltaTime;
	    if (timer >= duration)
	    {
            for (int i = 0; i < animations.Length; i++)
            {
                animations[i].StopAnimation();
            }
            gameObject.SetActive(false);
            audioSource.Stop();
	        started = false;
	    }
        
	}
}
