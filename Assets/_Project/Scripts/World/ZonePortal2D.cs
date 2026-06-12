using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ZonePortal2D : MonoBehaviour
{
    [Header("Portal")]
    [SerializeField] private GameZone targetZone = GameZone.CombatField;
    [SerializeField] private WorldZoneManager worldZoneManager;

    [Header("Trigger Settings")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float teleportCooldownSeconds = 0.25f;

    [Header("Debug")]
    [SerializeField] private bool logTeleport = true;

    private static float lastTeleportTime = -999f;

    private void Awake()
    {
        Collider2D portalCollider = GetComponent<Collider2D>();
        portalCollider.isTrigger = true;

        if (worldZoneManager == null)
        {
            worldZoneManager = FindFirstObjectByType<WorldZoneManager>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag))
        {
            return;
        }

        if (Time.time < lastTeleportTime + teleportCooldownSeconds)
        {
            return;
        }

        if (worldZoneManager == null)
        {
            Debug.LogWarning($"{name} cannot teleport because it has no WorldZoneManager assigned.");
            return;
        }

        lastTeleportTime = Time.time;

        if (logTeleport)
        {
            Debug.Log($"{name} teleporting player to {targetZone}.");
        }

        worldZoneManager.LoadZone(targetZone);
    }
}