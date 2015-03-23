using UnityEngine;

public class LaserController : MonoBehaviour
{

    public GameObject startSpot;
    public GameObject endSpot;
    public GameObject ray;
    private float raySpriteWidth;
    private bool started;
    private float timer;
    private float duration;

    public void Draw(Vector3 startPosition, Vector3 endPosition, float duration)
    {
        this.duration = duration;
        var distanceVector = endPosition - startPosition;
        var scaleX = distanceVector.magnitude/raySpriteWidth * 2;
        ray.transform.localScale = new Vector3(scaleX, 1);
        ray.transform.position = (startPosition + endPosition) / 2f;
        var angle = Mathf.Atan2(distanceVector.y, distanceVector.x) * Mathf.Rad2Deg;
        ray.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        startSpot.transform.position = startPosition;
        endSpot.transform.position = endPosition;
        started = true;
        timer = 0f;
    }

	void Awake ()
	{
        raySpriteWidth = endSpot.renderer.bounds.size.x;
	}

    void Update()
    {
        if (!started)
        {
            return;
        }

        timer += Time.deltaTime;
        if (timer >= duration)
        {
            gameObject.SetActive(false);
            started = false;
        }

    }
}
