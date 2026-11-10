using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public Item item;

    [Header("Collision (clamped so bad Inspector values can't break it)")]
    public float collisionRange = 10f;
    public float laneTolerance = 0.5f;
    public float requiredJumpHeight = 1f;
    
    private const float MAX_COLLISION_RANGE = 12f;
    private const float MAX_LANE_TOLERANCE = 2f;
    private const float MAX_JUMP_REQUIRED = 4f;

    private PlayerController player;
    private bool hasHitPlayer;

    private void Awake()
    {
        if (item == null)
            item = GetComponent<Item>();

        ClampValues();
    }
    
    private void OnValidate()
    {
        ClampValues();
    }

    private void ClampValues()
    {
        collisionRange = Mathf.Clamp(collisionRange, 1f, MAX_COLLISION_RANGE);
        laneTolerance = Mathf.Clamp(laneTolerance, 0.1f, MAX_LANE_TOLERANCE);
        requiredJumpHeight = Mathf.Clamp(requiredJumpHeight, 0.5f, MAX_JUMP_REQUIRED);
    }

    private void Start()
    {
        player = FindFirstObjectByType<PlayerController>();
    }

    private void Update()
    {
        if (player == null || player.IsDead || hasHitPlayer)
            return;

        float zDifference = item.itemPosition.z - player.item.itemPosition.z;
        
        if (zDifference > collisionRange || zDifference < -collisionRange * 2f)
            return;

        float xDifference = Mathf.Abs(item.itemPosition.x - player.item.itemPosition.x);

        bool sameLane = xDifference <= laneTolerance;
        bool jumping = player.CurrentJumpHeight >= requiredJumpHeight;

        if (sameLane && !jumping)
        {
            player.TakeDamage(5f);
            hasHitPlayer = true;
        }
    }
}