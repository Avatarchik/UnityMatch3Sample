using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class ExplosionController : MonoBehaviour {

    private AudioSource _audio;
    private GameObject _gameObject;

	void Awake ()
	{
	    _audio = audio;
	    _gameObject = gameObject;
	}
	
    public void OnAnimationStart()
    {
        _audio.Play();
    }

    public void OnAnimationFinish()
    {
        _gameObject.SetActive(false);
    }
}
