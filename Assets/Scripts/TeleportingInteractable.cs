using UnityEngine;

public class TeleportingInteractable : Interactable
{
    [SerializeField] private Transform PointA;
    [SerializeField] private Transform PointB;

    [SerializeField] private Transform playerTransform;
    [SerializeField] private CharacterController characterController;

    [SerializeField] private HatchInteractable hatchInteractable;

    [SerializeField] private bool isToOutside = false;

    private bool Underwater = false;
    private Transform targetPoint;    public override void Interact()
    {
        //Debug.Log("Player interacted with ladder");
        if (hatchInteractable != null && !hatchInteractable.IsHatchOpen())
        {
            return;
        }
        Underwater = WaveScript.Instance.CalculateWave(PointA.position).y - PointA.position.y > 0f;
        //Debug.Log("Underwater: " + Underwater);
        if(Vector3.Distance(playerTransform.position, PointA.position) < Vector3.Distance(playerTransform.position, PointB.position))
        {
            targetPoint = PointB;
            //Debug.Log("Player moved to Point B");
            if (isToOutside)
            {
                OceanVisibilityTrigger.Instance.OceanVisible(false);
                Environment.Instance.playerInsideSub = true;
            }
        }
        else if (!Underwater || !isToOutside)
        {
            targetPoint = PointA;
            //Debug.Log("Player moved to Point A");
            if (isToOutside)
            {
                OceanVisibilityTrigger.Instance.OceanVisible(true);
                Environment.Instance.playerInsideSub = false;
            }
        }
        else
        {
            return;
        }
        characterController.enabled = false;
        //Debug.Log("player position before moving: " + playerTransform.position);
        //Debug.Log("target position: " + targetPoint.position);
        playerTransform.position = targetPoint.position;
        //Debug.Log("player position after moving: " + playerTransform.position);
        characterController.enabled = true;
    }

    public override string GetInteractionPrompt()
    {
        return "E";
    }
}
