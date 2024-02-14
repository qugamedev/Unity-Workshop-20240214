using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

enum SpawnSide {
    TOP = 0,
    BOTTOM = 1,
    LEFT = 2,
    RIGHT = 3,
    COUNT = 4
}

public class Game : MonoBehaviour
{
    public Player player;
    public Asteroid asteroidPrefab;
    public ParticleSystem explosionEffect;
    public GameObject gameOverUI;
    public Canvas canvas;

    public int score = 0;
    public int lives = 3;

    public int asteroidsPerWave = 3;
    public float spawnMargin = 1f;

    public TMP_Text scoreText;
    public TMP_Text livesText;

    public float respawnDelay = 2;
    public float respawnInvulnerability = 2;

    public AudioSource audioSource;
    public AudioClip smallExplosionSound;
    public AudioClip mediumExplosionSound;
    public AudioClip bigExplosionSound;

    private Vector2 GetScreenDim()
    {
        // this is the width of the screen in world units (it depends on the camera settings)
        float screenWidth = Camera.main.orthographicSize * Camera.main.aspect * 2;
        float screenHeight = Camera.main.orthographicSize * 2;
        return new Vector2(screenWidth, screenHeight);
    }

    private void Awake()
    {
        explosionEffect.gameObject.SetActive(false);

        // update canvas since it is in world space - it won't otherwise react to camera size changes.
        Vector2 dim = GetScreenDim();

        // todo: need to be updating this whenever the size of the screen changes.
        RectTransform rectTransform = canvas.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(dim.x, dim.y);
    }

    private void Start()
    {
        scoreText.text = score.ToString();
        livesText.text = lives.ToString();

        gameOverUI.SetActive(false);
        SpawnPlayer();
        SpawnAsteroidWave();
    }

    private void Update()
    {
        // restart game logic.
        if (lives <= 0 && Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void SpawnPlayer()
    {
        player.transform.position = player.respawnPoint;
        player.invulnerability = respawnInvulnerability;
        player.gameObject.SetActive(true);
    }

    public void SpawnAsteroidWave()
    {
        Vector2 dim = GetScreenDim();
        float screenWidth = dim.x;
        float screenHeight = dim.y;

        for (int i = 0; i < asteroidsPerWave; i++)
        {
            SpawnSide spawnSide = (SpawnSide)Random.Range(0, (int)SpawnSide.COUNT);
            Vector2 spawnPoint;

            switch(spawnSide) {
                case SpawnSide.TOP:
                spawnPoint = new Vector2(Random.Range(-screenWidth / 2, screenWidth / 2), screenHeight / 2 + spawnMargin);
                break;
                case SpawnSide.BOTTOM:
                spawnPoint = new Vector2(Random.Range(-screenWidth / 2, screenWidth / 2), -screenHeight / 2 - spawnMargin);
                break;
                case SpawnSide.LEFT:
                spawnPoint = new Vector2(-screenWidth / 2 - spawnMargin, Random.Range(-screenHeight / 2, screenHeight / 2));
                break;
                case SpawnSide.RIGHT:
                spawnPoint = new Vector2(screenWidth / 2 +spawnMargin, Random.Range(-screenHeight / 2, screenHeight / 2));
                break;
                default:
                Debug.Log("oops this is bug!"); // TODO: should be statically detectable that we would hit this, since code right above
                // details what random numbers we will get.
                spawnPoint = new Vector2(0,0);
                break;
            }

            Asteroid newAsteroid = Instantiate(asteroidPrefab);
            newAsteroid.transform.position = spawnPoint;
            newAsteroid.SetTrajectory(Random.insideUnitCircle);
        }
    }


    // asteroid is notifying us.
    public void AsteroidDestroyedNotify(Asteroid asteroid)
    {
        // NOTE: particle effect plays for a set mount of time. we don't need to destroy. we just reuse the same particle dude.
        explosionEffect.transform.position = asteroid.transform.position;
        explosionEffect.gameObject.SetActive(true);
        explosionEffect.Play(); 

        //score based on size
        if (asteroid.size < 0.7f)
        {
            score += 100; // small asteroid
            audioSource.PlayOneShot(smallExplosionSound, 1);
        }
        else if (asteroid.size < 1.4f)
        {
            score += 50; // medium asteroid
            audioSource.PlayOneShot(mediumExplosionSound, 1);
        }
        else
        {
            score += 25; // large asteroid
            audioSource.PlayOneShot(bigExplosionSound, 1);
        }

        scoreText.text = score.ToString();

        //check the winning condition
        Asteroid[] asteroids = FindObjectsOfType<Asteroid>();

        // Destroy() takes effect at the end of the frame
        // hence cannot check that no asteroids exist from here.
        // require other soln and that what's written below. using size == 0
        // to indicate asteroid "death".
        bool stageClear = true;
        
        foreach (Asteroid a in asteroids)
        {
            if (a.size > 0)
                stageClear = false;
        }

        if(stageClear)
        {
            asteroidsPerWave++;
            SpawnAsteroidWave();
        }
    }
    
    // player is notifying us.
    public void PlayerDeathNotify(Player player)
    {
        explosionEffect.transform.position = player.transform.position;
        explosionEffect.gameObject.SetActive(true);
        explosionEffect.Play();

        player.gameObject.SetActive(false);
        lives--;
        livesText.text = lives.ToString();
        audioSource.PlayOneShot(bigExplosionSound, 1);

        if (lives <= 0)
            GameOver();
        else
            // invoke calls a function by name after a delay in seconds
            Invoke("SpawnPlayer", respawnDelay);
    }

    public void GameOver()
    {
        gameOverUI.SetActive(true);
    }

}
