using UnityEngine;

internal class MobileInput : IInput
{
    public bool IsPointerDown()
    {
        return Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
    }

    public bool IsPointerUp()
    {
        return Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended;
    }

    public bool IsPointerHeld()
    {
        return Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved;
    }
}