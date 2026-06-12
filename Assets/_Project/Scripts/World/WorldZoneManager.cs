using UnityEngine;

public enum GameZone
{
    Town,
    CombatField
}

public class WorldZoneManager : MonoBehaviour
{
    [Header("Current Zone")]
    [SerializeField] private GameZone startingZone = GameZone.Town;
    [SerializeField] private GameZone currentZone;

    [Header("Player")]
    [SerializeField] private Transform player;
    [SerializeField] private AutoCombatController autoCombatController;
    [SerializeField] private PlayerMovement2D playerMovement;

    [Header("Camera")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform townCameraAnchor;
    [SerializeField] private Transform combatCameraAnchor;

    [Header("Player Spawn Anchors")]
    [SerializeField] private Transform townPlayerAnchor;
    [SerializeField] private Transform combatPlayerAnchor;

    [Header("Combat")]
    [SerializeField] private EnemySpawner[] combatSpawners;

    [Header("Zone UI")]
    [SerializeField] private GameObject townActionPanel;
    [SerializeField] private GameObject combatActionPanel;

    [Header("Movement Bounds")]
    [SerializeField] private Vector2 townXBounds = new Vector2(-11f, -5f);
    [SerializeField] private Vector2 combatXBounds = new Vector2(-3.5f, 3.5f);

    public GameZone CurrentZone => currentZone;
    private bool hasInitialized;

    private void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (autoCombatController == null && player != null)
        {
            autoCombatController = player.GetComponent<AutoCombatController>();
        }

        if (playerMovement == null && player != null)
        {
            playerMovement = player.GetComponent<PlayerMovement2D>();
        }
    }

    private void Start()
    {
        if (!hasInitialized)
        {
            SetZone(startingZone);
        }
    }

    public void GoToTown()
    {
        SetZone(GameZone.Town);
    }

    public void GoToCombatField()
    {
        SetZone(GameZone.CombatField);
    }

    public void LoadZone(GameZone zone)
    {
        SetZone(zone);
    }

    private void SetZone(GameZone zone)
    {
        hasInitialized = true;
        currentZone = zone;

        switch (zone)
        {
            case GameZone.Town:
                EnterTown();
                break;

            case GameZone.CombatField:
                EnterCombatField();
                break;
        }

        GameEvents.RaiseZoneChanged(currentZone);
    }

    private void EnterTown()
    {
        MovePlayerTo(townPlayerAnchor);
        MoveCameraTo(townCameraAnchor);

        if (playerMovement != null)
        {
            playerMovement.SetCanMove(true);
            playerMovement.SetXBounds(townXBounds.x, townXBounds.y);
        }

        if (autoCombatController != null)
        {
            autoCombatController.enabled = false;
        }

        StopCombatSpawners();

        if (townActionPanel != null)
        {
            townActionPanel.SetActive(true);
        }

        if (combatActionPanel != null)
        {
            combatActionPanel.SetActive(false);
        }   
    }

    private void EnterCombatField()
    {
        MovePlayerTo(combatPlayerAnchor);
        MoveCameraTo(combatCameraAnchor);

        if (playerMovement != null)
        {
            playerMovement.SetCanMove(true);
            playerMovement.SetXBounds(combatXBounds.x, combatXBounds.y);
        }

        if (autoCombatController != null)
        {
            autoCombatController.enabled = true;
        }

        StartCombatSpawners();

        if (townActionPanel != null)
        {
            townActionPanel.SetActive(false);
        }

        if (combatActionPanel != null)
        {
            combatActionPanel.SetActive(true);
        }

    }

    private void MovePlayerTo(Transform anchor)
    {
        if (player == null || anchor == null)
        {
            return;
        }

        player.position = anchor.position;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    private void MoveCameraTo(Transform anchor)
    {
        if (mainCamera == null || anchor == null)
        {
            return;
        }

        Vector3 newPosition = anchor.position;
        newPosition.z = mainCamera.transform.position.z;

        mainCamera.transform.position = newPosition;
    }

    private void StartCombatSpawners()
    {
        if (combatSpawners == null)
        {
            return;
        }

        for (int i = 0; i < combatSpawners.Length; i++)
        {
            if (combatSpawners[i] != null)
            {
                combatSpawners[i].BeginSpawning();
            }
        }
    }

    private void StopCombatSpawners()
    {
        if (combatSpawners == null)
        {
            return;
        }

        for (int i = 0; i < combatSpawners.Length; i++)
        {
            if (combatSpawners[i] != null)
            {
                combatSpawners[i].StopSpawningAndDespawn();
            }
        }
    }
}