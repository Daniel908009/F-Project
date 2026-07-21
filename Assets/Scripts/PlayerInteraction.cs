using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class PlayerInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private TextMeshProUGUI interactionText;

    [Header("Interaction Settings")]
    [SerializeField] private float interactDistance = 3f;

    private PlayerControls controls;
    private Interactable currentInteractable;

    private void Awake()
    {
        controls = new PlayerControls();
    }

    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    private void Update()
    {
        CheckForInteractable();
    }

    public void OnInteract()
    {
        if (currentInteractable != null)
        {
            currentInteractable.Interact();
        }
    }

    private void CheckForInteractable()
    {
        currentInteractable = null;
        interactionText.text = "";
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            Interactable interactable = hit.collider.GetComponentInParent<Interactable>();
            if (interactable != null)
            {
                currentInteractable = interactable;
                interactionText.text = interactable.GetInteractionPrompt();
            } else if (hit.collider.GetComponentInParent<MonitorScript>() != null)
            {
                Vector2 pointerEventDelta = new Vector2(Screen.width / 2f, Screen.height / 2f);
                List<RaycastResult> results = new List<RaycastResult>();
                hit.collider.GetComponentInParent<MonitorScript>().raycaster.Raycast(new PointerEventData(EventSystem.current) { position = pointerEventDelta }, results);
                //Debug.Log("results count: " + results.Count);
                if (results.Count > 0)
                {
                    Interactable button;
                    foreach (RaycastResult result in results)
                    {
                        button = result.gameObject.GetComponent<Interactable>();
                        //Debug.Log("Button: " + button);
                        if (button != null)
                        {
                            //Debug.Log("Interacted with " + button);
                            currentInteractable = button;
                            interactionText.text = button.GetInteractionPrompt();
                            break;
                        }
                    }
                }
            }
            
        }
    }
}