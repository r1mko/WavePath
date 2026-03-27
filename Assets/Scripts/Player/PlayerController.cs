using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private const string DEATH_KEY = "DEATH_COUNT";

    public bool LevelStarted;
    public bool LevelFinished;
    public bool PauseMovement;

    // wave movement
    [SerializeField] private Vector2 waveForceDirectionUp;
    [SerializeField] private Vector2 waveForceDirectionDown;

    // wave angle
    [SerializeField] private float waveAngleRotation;
    [SerializeField] private float waveLerpBackSpeed;
    [SerializeField] private AnimationCurve moveTransitionCurve;

    // border detection
    [SerializeField] private LayerMask borderLayer;
    [SerializeField] private float borderCheckRadius;

    [SerializeField] private ParticleSystem deathParticle;
    [SerializeField] private AnimationCurve scaleCurve;

    private bool playerIsDead;

    private bool canSpeed = true;
    private bool canSlow = true;
    private bool canReverse = true;

    private float cooldownZones = 1f;
    private float cooldownSpeedZones = 0.5f;

    private int horizontalDirection = 1;
    private float speedMultiplicator = 1f;
    private float speedStep = 0.5f;

    private Rigidbody2D playerRb;
    private float current, target;

    [SerializeField] private float animationDuration = 0.15f;

    private bool atTopBorder;
    private bool atBottomBorder;

    private bool inputActive;


    public void StartedMove()
    {
        LevelStarted = true;
    }

    private void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
        PauseMovement = false;
        playerIsDead = false;
        LevelFinished = false;
        horizontalDirection = 1;
        speedMultiplicator = 1f;
        StartCoroutine(PlayScaleSequence(Vector3.zero, Vector3.one));
        SoundManager.Instance.PlaySpawn();
    }

    private void Update()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space))
        {
            if (!LevelStarted)
            {
                StartedMove();
            }

            inputActive = true;
        }
        else
        {
            inputActive = false;
        }

        target = inputActive ? 1 : 0;
    }

    private void FixedUpdate()
    {
        if (!LevelStarted) return;
        if (PauseMovement)
        {
            playerRb.linearVelocity = Vector2.zero;
            return;
        }

        CheckBorders();

        float currentForceX_Up = Mathf.Abs(waveForceDirectionUp.x) * horizontalDirection * speedMultiplicator;
        float currentForceX_Down = Mathf.Abs(waveForceDirectionDown.x) * horizontalDirection * speedMultiplicator;

        Vector2 targetVelocity;

        if (target == 1)
        {
            if (atTopBorder)
            {
                targetVelocity = new Vector2(currentForceX_Up, 0f);
            }
            else
            {
                targetVelocity = new Vector2(currentForceX_Up, waveForceDirectionUp.y);
            }
        }
        else
        {
            if (atBottomBorder)
            {
                targetVelocity = new Vector2(currentForceX_Down, 0f);
            }
            else
            {
                targetVelocity = new Vector2(currentForceX_Down, waveForceDirectionDown.y);
            }
        }

        Vector2 velocityChange = (targetVelocity - playerRb.linearVelocity) * playerRb.mass * 50f;
        playerRb.AddForce(velocityChange);

        current = Mathf.MoveTowards(current, target, waveLerpBackSpeed * Time.fixedDeltaTime);

        if (atTopBorder || atBottomBorder)
        {
            transform.rotation = Quaternion.Euler(Vector3.zero);
        }
        else
        {
            float angleMultiplier = horizontalDirection;
            float currentAngle = Mathf.Lerp(-waveAngleRotation, waveAngleRotation, moveTransitionCurve.Evaluate(current));
            transform.rotation = Quaternion.Euler(0, 0, currentAngle * angleMultiplier);
        }
    }

    private void CheckBorders()
    {
        Vector2 topCheckPos = (Vector2)transform.position + Vector2.up * borderCheckRadius;
        Collider2D[] topColliders = Physics2D.OverlapCircleAll(topCheckPos, borderCheckRadius, borderLayer);
        atTopBorder = false;
        foreach (var col in topColliders)
        {
            if (col.CompareTag("Border"))
            {
                atTopBorder = true;
                break;
            }
        }

        Vector2 bottomCheckPos = (Vector2)transform.position + Vector2.down * borderCheckRadius;
        Collider2D[] bottomColliders = Physics2D.OverlapCircleAll(bottomCheckPos, borderCheckRadius, borderLayer);
        atBottomBorder = false;
        foreach (var col in bottomColliders)
        {
            if (col.CompareTag("Border"))
            {
                atBottomBorder = true;
                break;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (LevelFinished) return;

        string tag = collision.tag;

        switch (tag)
        {
            case "Obstacle":
                StartCoroutine(LevelRestart());
                break;
            case "RevertMovement":
                if (canReverse) ReverseMovement();
                break;
            case "SpeedZone":
                if (canSpeed) SpeedUpMovement();
                break;
            case "SlowZone":
                if (canSlow) SlowDownMovement();
                break;
        }
    }

    private void ReverseMovement()
    {
        canReverse = false;
        horizontalDirection *= -1;
        GameManager.Instance.Camera.SetDirection(horizontalDirection);
        SoundManager.Instance.PlayDirectionChange();
        StartCoroutine(CooldownCoroutine(cooldownZones, () => canReverse = true));
    }

    private void SpeedUpMovement()
    {
        canSpeed = false;
        speedMultiplicator += speedStep;
        SoundManager.Instance.PlayAcceleration();
        StartCoroutine(CooldownCoroutine(cooldownSpeedZones, () => canSpeed = true));
    }

    private void SlowDownMovement()
    {
        canSlow = false;
        speedMultiplicator -= (speedStep / 2);
        SoundManager.Instance.PlaySlow();
        StartCoroutine(CooldownCoroutine(cooldownZones, () => canSlow = true));
    }

    private IEnumerator CooldownCoroutine(float seconds, System.Action onCooldownEnd)
    {
        yield return new WaitForSeconds(seconds);
        onCooldownEnd?.Invoke();
    }

    private IEnumerator PlayScaleSequence(Vector3 from, Vector3 to)
    {
        float elapsed = 0f;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / animationDuration);
            float curveValue = scaleCurve.Evaluate(t);
            Vector3 newScale = Vector3.Lerp(from, to, curveValue);
            transform.localScale = newScale;

            yield return null;
        }

        transform.localScale = to;
    }

    public IEnumerator LevelRestart()
    {
        if (playerIsDead) yield break;

        playerIsDead = true;
        PauseMovement = true;
        int deathCount = PlayerPrefs.GetInt(DEATH_KEY) + 1;
        PlayerPrefs.SetInt(DEATH_KEY, deathCount);
        PlayerPrefs.Save();
        HidePlayerView();
        SoundManager.Instance.PlayDeath();
        Instantiate(deathParticle, transform.position, transform.rotation).Play();
        yield return new WaitForSeconds(0.7f);
        Destroy(gameObject);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void HidePlayerView()
    {
        SpriteRenderer playerView = GetComponent<SpriteRenderer>();
        playerView.enabled = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector2 topPos = (Vector2)transform.position + Vector2.up * borderCheckRadius;
        Gizmos.DrawWireSphere(topPos, borderCheckRadius);

        Gizmos.color = Color.cyan;
        Vector2 bottomPos = (Vector2)transform.position + Vector2.down * borderCheckRadius;
        Gizmos.DrawWireSphere(bottomPos, borderCheckRadius);
    }
}