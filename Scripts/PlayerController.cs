using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float boundaryLeft = 167f;
    public float boundaryRight = 197f;
    public float boundaryTop = 6f;
    public float boundaryBottom = -10f;
    
    public TextMeshProUGUI healthText;
    public GameObject gameOverPanel;
    public Button restartButton;
    public TextMeshProUGUI timeSurvivedText;

    private float timeSurvived = 0f;
    private float playerHealth = 100f; // Update health in script instead of inspector

    private Rigidbody2D rb;
    private Camera mainCamera;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        // Add this at the beginning of Start() to ensure the game is not paused
        Time.timeScale = 1;

        rb = GetComponent<Rigidbody2D>();
        transform.position = new Vector3(-296, -268, 0);
        mainCamera = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Deactivate GameOver panel at the beginning
        gameOverPanel.SetActive(false);
        restartButton.onClick.AddListener(RestartGame);
    }

    public void TakeDamage(float damage)
    {
        playerHealth -= damage;
        healthText.text = "Health: " + playerHealth.ToString();

        if (playerHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (!gameOverPanel.activeSelf)
        {
            Debug.Log("Player has died.");
            gameOverPanel.SetActive(true);
            restartButton.interactable = true; 
            Time.timeScale = 0;
            //Destroy(gameObject);
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        timeSurvived = 0f; // Reset the timeSurvived variable to 0
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    void FixedUpdate()
    {
        // Increment time survived
        timeSurvived += Time.fixedDeltaTime;

        // Update time survived text
        timeSurvivedText.text = "Time Survived: " + Mathf.RoundToInt(timeSurvived).ToString();

        // Update health text
        healthText.text = "Health: " + playerHealth.ToString();

        if (playerHealth <= 0)
        {
            Die();
        }

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector2 targetVelocity = new Vector2(moveHorizontal, moveVertical) * speed;
        rb.velocity = targetVelocity;

        // Clamp player position to camera bounds
        Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
        Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.nearClipPlane));

        float playerWidth = spriteRenderer.bounds.size.x;
        float playerHeight = spriteRenderer.bounds.size.y;

        float clampedX = Mathf.Clamp(transform.position.x, bottomLeft.x + playerWidth / 2, topRight.x - playerWidth / 2);
        float clampedY = Mathf.Clamp(transform.position.y, bottomLeft.y + playerHeight / 2, topRight.y - playerHeight / 2);

        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }
}
