using UnityEngine;
using UnityEngine.UI;

public class PopupController : MonoBehaviour {
    private RectTransform _transform;
    private Text textComponent;
    private Animator animator;

	void Awake ()
	{
	    _transform = GetComponent<RectTransform>();
        textComponent = GetComponentInChildren<Text>();
	    animator = GetComponent<Animator>();
	}
	
	public void ShowPopup(Vector3 position, string text) {
        _transform.localPosition = position * 100;
        textComponent.text = text;
        animator.Play("PopupText");
	}

    public void OnAnimationCompleted()
    {
        gameObject.SetActive(false);
    }
}
