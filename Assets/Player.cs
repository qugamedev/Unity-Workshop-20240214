using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody2D rb;
    public GameObject bulletPrefab;
    public Animator animator;
    
    private Game gameManager;

    public float thrustSpeed;
    public float maxSpeed;
    public float rotationSpeed;
    public float maxAngularVelocity;
    public Vector2 respawnPoint;

    [HideInInspector]
    public float invulnerability;

    //the name of the inputs as defined in project settings
    public string HORIZONTAL_AXIS = "Horizontal";
    public string VERTICAL_AXIS = "Vertical";
    public string FIRE = "Fire1";

    //to control engine sound and effects independently I use two audio sources
    public AudioSource engineAudioSource;
    public AudioSource effectsAudioSource;

    public AudioClip thrustSound;
    public AudioClip fireSound;

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (animator == null)
            animator = GetComponent<Animator>();

        gameManager = FindObjectOfType<Game>();

        //the thrust sound is always looping and the sound is turn on or off.
        engineAudioSource.clip = thrustSound;
        engineAudioSource.loop = true;
        engineAudioSource.Play();
        engineAudioSource.volume = 0;
    }


    private void Update()
    {
        float horizontalInput = Input.GetAxisRaw(HORIZONTAL_AXIS);
        float verticalInput = Input.GetAxisRaw(VERTICAL_AXIS);

        rb.AddTorque(rotationSpeed * -horizontalInput * Time.deltaTime);

        if (verticalInput > 0)
        {
            rb.AddForce(transform.up * thrustSpeed * verticalInput * Time.deltaTime);            
            engineAudioSource.volume = 1;
            animator.Play("thrust");
        }
        else
        {
            engineAudioSource.volume = 0;
            animator.Play("idle");
        }

        // apply limits.
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);
        rb.angularVelocity = Mathf.Clamp(rb.angularVelocity, -maxAngularVelocity, +maxAngularVelocity);

        if (Input.GetButtonDown(FIRE))
        {
            Vector3 bulletPosition = transform.position + transform.up * 0.2f;
            GameObject newBullet = Instantiate(bulletPrefab, bulletPosition, transform.rotation);
            effectsAudioSource.PlayOneShot(fireSound, 1);
        }

        invulnerability -= Time.deltaTime;
    }

    private void OnEnable()
    {
        engineAudioSource.Play();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (invulnerability <= 0 && collision.gameObject.CompareTag("Asteroid"))
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = 0f;
            gameManager.PlayerDeathNotify(this);
        }
    }

}
