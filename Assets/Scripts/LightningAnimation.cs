using UnityEngine;
using System.Collections;

public class LightningAnimation : MonoBehaviour
{

    public float framesPerSecond = 15f;
    public int framesInAnimation = 8;
    public int startFameOffset;
    private float secondsPerFrame;
    private int currentFrame; 

	void Start ()
	{
	    secondsPerFrame = 1f/framesPerSecond;
	}

    public void StartAnimation()
    {
        currentFrame = startFameOffset;
        StartCoroutine(Animation());
    }

    public void StopAnimation()
    {
        StopAllCoroutines();
    }

    IEnumerator Animation()
    {
        while (true)
        {
            renderer.material.SetTextureOffset("_MainTex", new Vector2(currentFrame * 0.125f, 0));
            yield return new WaitForSeconds(secondsPerFrame);
            currentFrame++;
            if (currentFrame == framesInAnimation)
            {
                currentFrame = 0;
            }
        }
    }
}
