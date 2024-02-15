using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject bulletPrefab;
    
    private Game gameManager;

    public float maxSpeed = 3;
    public float dragCoeff = 4f;
    public float thrustCoeff = 23f;
    public float rotateThrustCoeff = 3.1f;
    public Vector2 respawnPoint;

    Vector3 velocity = new Vector3(0, 0, 0); // has units of 1unit/s.
    float playerRot = 0.0f; // in radians.

    [HideInInspector]
    public float invulnerability;

    //to control engine sound and effects independently I use two audio sources
    public AudioSource effectsAudioSource;

    public AudioSource thrustSounder;
    public AudioClip fireSound;

    private void Awake()
    {
        gameManager = FindObjectOfType<Game>();

        // the audio source is always going to be looping, but we just toggle the volume
        // when we want to hear it.
        thrustSounder.volume = 0;
    }

    private void Update()
    {
        //Input.GetAxis(); // unity will smooth things.
        float horziontalInput = Input.GetAxisRaw("Horizontal"); // unity won't touch the input.
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 velocityDirAndMag = Vector3.ClampMagnitude( velocity, 1.0f );

        velocity -= velocityDirAndMag*dragCoeff * Time.deltaTime; // apply drag.

        if (horziontalInput != 0.0f)
            playerRot += -rotateThrustCoeff * Time.deltaTime * Mathf.Sign(horziontalInput);

        Vector3 playerDir = new Vector3(Mathf.Cos(playerRot), Mathf.Sin(playerRot), 0);
        
        if (verticalInput > 0.0f) {
            velocity += playerDir * thrustCoeff * Time.deltaTime;
        }

        velocity = Vector3.ClampMagnitude( velocity, maxSpeed );

        // set the volume based on velocity.
        thrustSounder.volume = Vector3.Magnitude(velocity) / maxSpeed;

        // use the vertical input to move along the current "forward" direction of the ship.
        transform.position += velocity * Time.deltaTime;

        float phaseDiff = -90;
        transform.localRotation = Quaternion.Euler(0, 0, playerRot * Mathf.Rad2Deg + phaseDiff );

        if (Input.GetButtonDown("Fire1"))
        {
            Vector3 bulletPosition = transform.position + transform.up * 0.2f;
            GameObject newBullet = Instantiate(bulletPrefab, bulletPosition, transform.rotation);
            effectsAudioSource.PlayOneShot(fireSound, 1);
        }

        invulnerability -= Time.deltaTime;
    }

    private void OnEnable()
    {
        thrustSounder.Play();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (invulnerability <= 0 && collision.gameObject.CompareTag("Asteroid"))
        {
            //rb.velocity = Vector3.zero;
           // rb.angularVelocity = 0f;
            gameManager.PlayerDeathNotify(this);
        }
    }
}
