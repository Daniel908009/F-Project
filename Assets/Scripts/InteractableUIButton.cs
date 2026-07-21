using UnityEngine;
using UnityEngine.UI;

public class InteractableUIButton : Interactable
{
    private Button button;
    private void Awake()
    {
        button = GetComponent<Button>();
    }
    public override void Interact()
    {
        if (button != null)
        {
            //Debug.Log("Button Clicked");
            button.onClick.Invoke();
        }
    }
    public override string GetInteractionPrompt()
    {
        return "";
    }
}