using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float maxSpeed = 2;
    public float dragCoeff = 0.7f;
    public float thrustCoeff = 1.0f;
    public float rotateThrustCoeff = 1.0f;

    AudioSource thrustSounder;

    Vector3 velocity = new Vector3(0, 0, 0); // has units of 1unit/s.
    
    float playerRot = 0.0f; // in radians.

    // Start is called before the first frame update
    void Start()
    {
        thrustSounder = GetComponent<AudioSource>();

        // the audio source is always going to be looping, but we just toggle the volume
        // when we want to hear it.
        thrustSounder.volume = 0;
    }

    // Update is called once per frame, and the game is running at 60 frames per second, maybe.
    void Update()
    {
        //Input.GetAxis(); // unity will smooth things.
        float horziontalInput = Input.GetAxisRaw("Horizontal"); // unity won't touch the input.
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 velocityDirAndMag = Vector3.ClampMagnitude( velocity, 1.0f );

        velocity -= velocityDirAndMag*dragCoeff * Time.deltaTime; // apply drag.

        // TODO: angular accel.
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
    }
}
