using UnityEngine;

public class OceanTexture : MonoBehaviour
{
    [Header("Interaction Texture")]
    [SerializeField] private int textureResolution = 2048;
    //[SerializeField] private float sInteractionWidth = 0.0125f;
    //[SerializeField] private float sInteractionLength = 0.05f;
    //[SerializeField] private float sWakeWidth = 0.2f;
    //[SerializeField] private float sWakeLength = 0.3f;
    //[SerializeField] private float sWakeAngle = 0.5f;
    //[SerializeField] private float WakeEdgeSharpness = 5f;
    //[SerializeField] private Transform submarineHull;
    //[SerializeField] private Transform wakePosition;
    [SerializeField] private Material interactionMaterial;
    [SerializeField] private Material accumulatedMaterial;

    [Header("Other References")]
    [SerializeField] private FloatingObject submarine;
    [SerializeField] private Material waterMaterial;

    private RenderTexture interactionTexture;
    private RenderTexture interactionTexture2;
    private RenderTexture activeInteractionTexture;
    private bool useFirstTexture = true;

    public static OceanTexture Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    private void Start()
    {
        interactionTexture = CreateInteractionTexture(interactionTexture);
        interactionTexture2 = CreateInteractionTexture(interactionTexture2);
        activeInteractionTexture = CreateInteractionTexture(activeInteractionTexture);
    }
    private RenderTexture CreateInteractionTexture(RenderTexture texture)
    {
        RenderTextureDescriptor descriptor = new RenderTextureDescriptor(
            textureResolution,
            textureResolution,
            RenderTextureFormat.ARGB32,
            0
        );

        descriptor.sRGB = false;

        texture = new RenderTexture(descriptor);

        texture.name = "Ocean Interaction Texture";
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Bilinear;
        texture.Create();

        RenderTexture.active = texture;
        GL.Clear(true, true, Color.black);
        RenderTexture.active = null;
        return texture;
    }

    private void Update()
    {
        Vector3 submarinePosition = submarine.Hull.position;
        Vector3 wakePos = submarine.WakePosition.position;

        //ClearInteractionTexture();
        float strength = 1f - (SubmarineWaves.Instance.GetCurrentDepth() / 5f) * 0.75f;
        strength = Mathf.Clamp01(strength);
        //Debug.Log("Angles: " + submarine.Hull.rotation.eulerAngles.y);
        float strengthWake = submarine.CurrentSpeed / submarine.MaximumSpeed;
        //Debug.Log("strengthWake: " + strengthWake);
        float magnitude = submarine.MaxWake / WaveScript.Instance.MaxWakeHeight;
        DrawInteraction(submarine, wakePos, submarine.transform.rotation.eulerAngles.y, strength, strengthWake, magnitude);
        AccumulateTextures();
        useFirstTexture = !useFirstTexture;
        EnemyShip[] enemyShips = EnemyManager.Instance.GetEnemyShips();
        foreach (EnemyShip enemyShip in enemyShips)
        {
            Vector3 enemyWakePos = enemyShip.WakePosition.position;
            float enemyStrength = 1f;
            enemyStrength = Mathf.Clamp01(enemyStrength);
            float enemyStrengthWake = enemyShip.CurrentSpeed / enemyShip.MaximumSpeed;
            float enemyMagnitude = enemyShip.MaxWake / WaveScript.Instance.MaxWakeHeight;
            DrawInteraction(enemyShip, enemyWakePos, enemyShip.transform.rotation.eulerAngles.y, enemyStrength, enemyStrengthWake, enemyMagnitude);
            AccumulateTextures();
            useFirstTexture = !useFirstTexture;
        }

        waterMaterial.SetTexture("_InteractionTexture", useFirstTexture ? interactionTexture : interactionTexture2);
        //waterMaterial.SetTexture("_InteractionTexture", activeInteractionTexture);
        ClearInteractionTexture(useFirstTexture ? interactionTexture2 : interactionTexture);
        useFirstTexture = true;
    }
    private void AccumulateTextures()
    {
        accumulatedMaterial.SetTexture("_Interaction1", activeInteractionTexture);
        accumulatedMaterial.SetTexture("_Interaction2", useFirstTexture ? interactionTexture : interactionTexture2);
        Graphics.Blit(
            null,
            useFirstTexture ? interactionTexture2 : interactionTexture,
            accumulatedMaterial,
            0
        );
    }
    private void ClearInteractionTexture(RenderTexture texture)
    {
        RenderTexture.active = texture;

        GL.Clear(
            true,
            true,
            Color.black
        );

        RenderTexture.active = null;
    }
    private void DrawInteraction(FloatingObject fObject, Vector3 wakePosition, float rotation, float strength, float strengthWake, float magnitude)
    {

        interactionMaterial.SetFloat(
            "_WakeEdgeSharpness",
            fObject.WakeEdgeSharpness
        );
        Vector2 uv = WorldToInteractionUV(fObject.Hull.position);
        Vector2 wakeUV = WorldToInteractionUV(wakePosition);

        //Debug.Log($"Position: {position} | UV: {uv} | Rotation: {rotation}");
        float radians = rotation * Mathf.Deg2Rad;

        //Debug.Log($"Submarine Position: {position} | Submarine UV: {uv} | Submarine Rotation: {rotation} | Submarine Radians: {radians}");
        interactionMaterial.SetVector(
            "_InteractionPosition",
            uv
        );

        interactionMaterial.SetFloat(
            "_InteractionRotation",
            -radians
        );

        interactionMaterial.SetFloat(
            "_InteractionWidth",
            fObject.InteractionWidth
        );

        interactionMaterial.SetFloat(
            "_InteractionLength",
            fObject.InteractionLength
        );
        interactionMaterial.SetFloat(
            "_InteractionStrength",
            strength
        );

        //Debug.Log($"Wake Position: {wakePosition} | Wake UV: {wakeUV}");
        interactionMaterial.SetVector(
            "_InteractionWakePosition",
            wakeUV
        );
        interactionMaterial.SetFloat(
            "_InteractionWakeMagnitude",
            magnitude
        );
        interactionMaterial.SetFloat(
            "_WakeStrength",
            strengthWake
        );
        //Debug.Log("wake pos: " + fObject.WakePosition.transform.position);
        //Debug.Break();
        interactionMaterial.SetVector(
            "_InteractionWakePosNormal",
            fObject.WakePosition.position
        );
        //Debug.Log("wake length normal: " + fObject.WakeLengthNormal);
        interactionMaterial.SetFloat(
            "_InteractionWakeLength",
            fObject.WakeLength
        );
        interactionMaterial.SetFloat(
            "_InteractionWakeAngle",
            fObject.WakeAngle
        );
        interactionMaterial.SetFloat(
            "_InteractionWakeRotation",
            -radians + Mathf.PI
        );

        Graphics.Blit(
            null,
            activeInteractionTexture,
            interactionMaterial,
            0
        );
    }

    public Vector2 WorldToInteractionUV(Vector3 worldPosition)
    {
        worldPosition.x -= transform.position.x;
        worldPosition.z -= transform.position.z;
        float u = (worldPosition.x + 1050) / 2100f;
        float v = (worldPosition.z + 1050) / 2100f;
        //Debug.Log($"World Position: {worldPosition} | Interaction UV: ({u}, {v})");
        //Debug.Break();
        return new Vector2(u, v);
    }

    private void OnDestroy()
    {
        if (activeInteractionTexture != null)
        {
            activeInteractionTexture.Release();
            Destroy(activeInteractionTexture);
        }
        if (interactionTexture != null)
        {
            interactionTexture.Release();
            Destroy(interactionTexture);
        }
        if (interactionTexture2 != null)
        {
            interactionTexture2.Release();
            Destroy(interactionTexture2);
        }
    }
}