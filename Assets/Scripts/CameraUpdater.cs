using UnityEngine;

[ExecuteInEditMode]
public class CameraUpdater : MonoBehaviour {

    private ScreenOrientation prevOrientation = ScreenOrientation.Unknown;
    public float desiredOrtographicSizeForSmallerDimension = 5;
    private Camera _camera;

    void Start ()
	{
        _camera = GetComponent<Camera>();
	}
	
	void Update () {
	    if (Screen.orientation != prevOrientation)
	    {
	        prevOrientation = Screen.orientation;
            if (Screen.width > Screen.height)
            {
                camera.orthographicSize = desiredOrtographicSizeForSmallerDimension;
            }
            else
            {
                camera.orthographicSize = desiredOrtographicSizeForSmallerDimension / Screen.width * Screen.height;
            }
	    }
	}
}
