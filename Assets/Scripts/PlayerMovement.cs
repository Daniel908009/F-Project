using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float mouseSensitivity = 0.1f;
    [Header("References")]
    [SerializeField] private Transform cameraTransform;

    private Vector2 moveInput;
    private Vector2 lookInput;

    private float verticalRotation;

    public bool CanMove { get; set; } = true;
    public float MinVerticalAngle { get; set; } = -90f;
    public float MaxVerticalAngle { get; set; } = 90f;

    private float periscopeVerticalInput;
    private CharacterController characterController;
    private Vector3 velocity;
    //private Vector3 subM;
    
    public static PlayerMovement Instance { get; private set; }

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        Look();
        //MoveWithSub();
        if (CanMove)
        {
            Move();
        }
    }

    //private void FixedUpdate()   //FIX. This has to be fixedUpdate...
    //{
    //    if (CanMove)
    //    {
    //        Move();
    //    }
    //}

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        //Debug.Log("Look");
        lookInput = value.Get<Vector2>();
    }
    public void OnZoom()
    {
        CameraController.Instance.ZoomCamera(CameraController.Instance.targetFOV == CameraController.Instance.zoomedFOV ? CameraController.Instance.normalFOV : CameraController.Instance.zoomedFOV);
        //Debug.Log("Zoom");
    }
    private void Move()
    {
        Vector3 move =
            transform.right * moveInput.x +
            transform.forward * moveInput.y;

        //Vector3 lastPosition = transform.position;
        //characterController.Move(subM);
        //Debug.Log("before " + transform.position);
        //Debug.Log("after " + transform.position);
        //Vector3 actualDelta = transform.position - lastPosition;
        //Debug.Log($"Actual Delta: {actualDelta}");

        /*RaycastHit hit;
        Vector3 origin =transform.position +
                        characterController.center -
                        Vector3.up * (characterController.height / 2f - characterController.radius);
        if (Physics.Raycast(origin, Vector3.down, out hit, 0.2f))
        {
            velocity.y = 0f;
        }
        else
        {
            velocity.y += Physics.gravity.y * Time.deltaTime;
        }*/
        if (characterController.isGrounded)
        {
            velocity.y = 0f;
            //Debug.Log($"is grounded");
        }
        else
        {
            velocity.y += Physics.gravity.y * Time.deltaTime;
            //Debug.Log($"isnt grounded");
        }
        /*SubmarineWaves submarineWaves = FindObjectOfType<SubmarineWaves>();
        if (submarineWaves != null)
        {
            Transform subTransform = submarineWaves.transform;
            float subY = subTransform.position.y;
            float playerY = transform.position.y;
            float difference = subY - playerY;
            Debug.Log($"Submarine Y: {subY}, Player Y: {playerY}, Difference: {difference}");
        }*/
        //Debug.Log(characterController.collisionFlags);
        Vector3 finalMove = move * moveSpeed + velocity;
        characterController.Move(finalMove * Time.deltaTime);
    }
    //public void SetSubMovement(Vector3 subMovement)
    //{
     //   subM = subMovement;
        //Debug.Log($"Sub Movement: {subM}");
    //}

    public void MoveWithSub(Vector3 subMovement)
    {
        characterController.enabled = false;
        transform.position += subMovement;
        characterController.enabled = true;
    }
public void ApplySubRotation(Quaternion rotationDelta, Vector3 subPosition)
{
    characterController.enabled = false;

    Vector3 offset = transform.position - subPosition;
    offset = rotationDelta * offset;
    transform.position = subPosition + offset;

    transform.rotation = rotationDelta * transform.rotation;

    characterController.enabled = true;
}

    private void Look()
    {
        //Debug.Log($"Look Input: {lookInput}");
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, MinVerticalAngle, MaxVerticalAngle);

        cameraTransform.localRotation =
            Quaternion.Euler(verticalRotation, 0f, 0f);
        lookInput = Vector2.zero;
    }
    public void resetAngleLimits()
    {
        MinVerticalAngle = -90f;
        MaxVerticalAngle = 90f;
    }
}