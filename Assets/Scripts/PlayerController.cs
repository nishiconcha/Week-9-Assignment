using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Item item;

    [Header("Movement")]
    public float laneDistance = 6f;
    public float moveSpeed = 8f;

    private int currentLane = 1;
    private float targetX;

    [Header("Jump")]
    public float jumpHeight = 5f;
    public float jumpDuration = 0.7f;

    private float jumpTimer;

    public float CurrentJumpHeight { get; private set; }
    public bool IsJumping { get; private set; }

    [Header("Health")]
    public float maxHP = 100f;
    public float currentHP = 100f;
    public float regeneration = 1f;

    public bool IsDead => currentHP <= 0f;

    [Header("Damage Effect")]
    public Color damageColor = Color.red;
    public float flashDuration = 0.4f;
    public float shakeAmount = 0.5f;

    private Renderer rend;
    private Color originalColor;
    private float flashTimer;
    private Vector3 shakeOffset;

    private const float PLAYER_Y = -8f;
    private const float PLAYER_Z = 500f;

    private void Awake()
    {
        if (item == null)
            item = GetComponent<Item>();

        rend = GetComponentInChildren<Renderer>();
        originalColor = rend.material.color;
    }

    private void Start()
    {
        currentHP = maxHP;
        item.itemPosition = new Vector3(0f, PLAYER_Y, PLAYER_Z);
        
        if (MusicManager.Instance != null)
            MusicManager.Instance.PlayMusic("main");
    }

    private void Update()
    {
        item.itemPosition -= shakeOffset;
        shakeOffset = Vector3.zero;
        
        if (!IsDead)
        {
            MovePlayer();
            Jump();
            RegenerateHP();
        }

        // Damage effect (color fade + shake)
        UpdateDamageEffect();
        item.itemPosition += shakeOffset;
    }

    private void MovePlayer()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            currentLane = Mathf.Max(0, currentLane - 1);

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            currentLane = Mathf.Min(2, currentLane + 1);

        targetX = (currentLane - 1) * laneDistance;

        // easing toward the lane
        float easing = 1f - Mathf.Exp(-moveSpeed * Time.deltaTime);

        item.itemPosition.x = Mathf.Lerp(item.itemPosition.x, targetX, easing);
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !IsJumping)
        {
            IsJumping = true;
            jumpTimer = 0f;
        }

        if (IsJumping)
        {
            jumpTimer += Time.deltaTime;

            float progress = Mathf.Clamp01(jumpTimer / jumpDuration);
            CurrentJumpHeight = Mathf.Sin(progress * Mathf.PI) * jumpHeight;

            if (progress >= 1f)
            {
                IsJumping = false;
                CurrentJumpHeight = 0f;
            }
        }
        else
        {
            CurrentJumpHeight = 0f;
        }

        item.itemPosition.y = PLAYER_Y + CurrentJumpHeight;
    }

    private void RegenerateHP()
    {
        currentHP = Mathf.Min(maxHP, currentHP + regeneration * Time.deltaTime);
    }

    private void UpdateDamageEffect()
    {
        if (flashTimer <= 0f)
            return;

        flashTimer -= Time.deltaTime;

        float t = Mathf.Clamp01(flashTimer / flashDuration);

        // Red
        rend.material.color = Color.Lerp(originalColor, damageColor, t);

        // Shake
        shakeOffset = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            0f
        ) * shakeAmount * t;

        if (flashTimer <= 0f)
        {
            rend.material.color = originalColor;
            shakeOffset = Vector3.zero;
        }
    }

    public void TakeDamage(float amount)
    {
        if (IsDead)
            return;

        currentHP = Mathf.Max(0f, currentHP - amount);

        // Start the effect
        flashTimer = flashDuration;
        
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySound2D("DMG");
    }
}