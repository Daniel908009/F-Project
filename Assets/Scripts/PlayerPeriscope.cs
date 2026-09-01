using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPeriscope : MonoBehaviour
{
    [SerializeField] private PeriscopeController playerPeriscope;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Transform periscopeTransform;

    CharacterController characterController;
    private float verticalInput;
    private bool moved = false;
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }
    public void OnVerticalChange(InputValue value)
    {
        verticalInput = value.Get<float>();
        //Debug.Log("Periscope Vertical Input: " + verticalInput);
        
    }
    private void Update()
    {
        moved = playerPeriscope.MovePeriscope(verticalInput);
        if(playerInput.currentActionMap.name == "Periscope" && moved)
        {
            Vector3 newPosition = verticalInput * Vector3.up * playerPeriscope.moveSpeed * Time.deltaTime;
            transform.position = new Vector3(periscopeTransform.position.x, transform.position.y + newPosition.y, periscopeTransform.position.z);
        }
    }
    public void OnPeriscopeZoom(InputValue value)
    {
        float zoomInput = value.Get<float>();
        playerPeriscope.Zoom(zoomInput);
    }
}
