using UnityEngine;
using TMPro;

public class InteractableUIInputSelect : Interactable
{
    private TMP_InputField select;
    private void Awake()
    {
        select = GetComponent<TMP_InputField>();
        if (select != null)
        {
            select.onSelect.AddListener(OnInputFieldSelected);
            select.onDeselect.AddListener(OnInputFieldDeselected);
        }
    }
    public override void Interact()
    {
        if (select != null)
        {
            select.Select();
            select.ActivateInputField();
        }
    }
    public override string GetInteractionPrompt()
    {
        return "";
    }

    private void OnInputFieldDeselected(string text)
    {
        PlayerMovement.Instance.CanMove = true;
    }
    private void OnInputFieldSelected(string text)
    {
        PlayerMovement.Instance.CanMove = false;
    }
}
