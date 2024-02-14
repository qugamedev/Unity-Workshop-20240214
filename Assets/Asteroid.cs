using UnityEngine;


public class Asteroid : MonoBehaviour
{
    public Game gameManager;

    public new Rigidbody2D rigidbody;
    public SpriteRenderer spriteRenderer;
    public Sprite[] sprites;

    public float size = 1f;
    public float minSize = 0.35f;
    public float maxSize = 1.65f;
    public float movementSpeed = 50f;
    public float maxLifetime = 30f;

    private void Awake()
    {
        gameManager = FindObjectOfType<Game>();

        if(spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if(rigidbody == null)
            rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)];
        transform.eulerAngles = new Vector3(0f, 0f, Random.Range(0f, 360f));

        // Set the scale and mass of the asteroid based on the assigned size so
        // the physics is more realistic
        transform.localScale = Vector3.one * size;
        rigidbody.mass = size;        
    }

    public void SetTrajectory(Vector2 direction)
    {
        rigidbody.AddForce(direction * movementSpeed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            // Check if the asteroid is large enough to split in half
            // (both parts must be greater than the minimum size)
            if ((size * 0.5f) >= minSize)
            {
                Vector2 randDir = Random.insideUnitCircle.normalized;

                CreateSplit(randDir);
                CreateSplit(-randDir);
            }
            else
            {
                // set size to zero to indicate death.
                size = 0;
            }

            gameManager.AsteroidDestroyedNotify(this);
            Destroy(gameObject);
        }
    }

    private Asteroid CreateSplit(Vector2 newTrajectory)
    {
        Vector2 position = (Vector2)transform.position + newTrajectory * 0.5f;
        Asteroid half = Instantiate(this, position, transform.rotation);
        half.size = size * 0.5f;
        half.SetTrajectory(newTrajectory);
        return half;
    }

}
