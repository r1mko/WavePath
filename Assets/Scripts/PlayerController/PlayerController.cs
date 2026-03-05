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

        Vector2 targetVelocity;

        if (target == 1)
        {
            targetVelocity = atTopBorder ? new Vector2(waveForceDirectionUp.x, 0f) : waveForceDirectionUp;
        }
        else
        {
            targetVelocity = atBottomBorder ? new Vector2(waveForceDirectionDown.x, 0f) : waveForceDirectionDown;
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
            transform.rotation = Quaternion.Lerp(
                Quaternion.Euler(0, 0, -waveAngleRotation),
                Quaternion.Euler(0, 0, waveAngleRotation),
                moveTransitionCurve.Evaluate(current)
            );
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
            //LevelRestart();
        }
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