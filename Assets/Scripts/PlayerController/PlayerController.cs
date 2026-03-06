using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public bool GameStarted;

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

    // reverse movement
    private bool canReverse = true; 
    private float reverseCooldown = 3f;
    private int horizontalDirection = 1;

    private Rigidbody2D playerRb;
    private float current, target;

    private bool atTopBorder;
    private bool atBottomBorder;


    public void StartedMove()
    {
        GameStarted = true;
    }

    private void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
        horizontalDirection = 1;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && !GameStarted)
        {
            StartedMove();
        }

        target = Input.GetMouseButton(0) ? 1 : 0;
    }

    private void FixedUpdate()
    {
        if (!GameStarted) return;

        CheckBorders();

        float currentForceX_Up = Mathf.Abs(waveForceDirectionUp.x) * horizontalDirection;
        float currentForceX_Down = Mathf.Abs(waveForceDirectionDown.x) * horizontalDirection;

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
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log("Обнаружено препятствие");
            //LevelRestart();
        }

        if (collision.gameObject.CompareTag("RevertMovement"))
        {
            if (canReverse)
            {
                ReverseMovement();
                Debug.Log("Инвертирование движения выполнено");
            }
        }
    }

    private void ReverseMovement()
    {
        canReverse = false;
        horizontalDirection *= -1;
        GameManager.Instance.Camera.SetDirection(horizontalDirection);
        StartCoroutine(ReverseCooldownCoroutine());
    }

    private IEnumerator ReverseCooldownCoroutine()
    {
        yield return new WaitForSeconds(reverseCooldown);
        canReverse = true;
    }

    public void LevelRestart()
    {
        Destroy(gameObject);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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