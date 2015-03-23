using UnityEngine;

internal class StandaloneInput : IInput
{
    public bool IsPointerDown()
    {
        return Input.GetMouseButtonDown(0);
    }

    public bool IsPointerUp()
    {
        return Input.GetMouseButtonUp(0);
    }

    public bool IsPointerHeld()
    {
        return Input.GetMouseButton(0);
    }
}
