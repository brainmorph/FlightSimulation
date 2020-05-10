using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thrust : MonoBehaviour
{

    public Rigidbody rb;
    
    // Start is called before the first frame update
    void Start()
    {

    }

    private float thrustCommand = 0f;
    private float rollCommand = 0f;
    private float pitchCommand = 0f;
    private float yawCommand = 0f;

    private float desiredRoll = 0f;     // angle
    private float desiredPitch = 0f;    // angle
    private float desiredYaw = 0f;      // angle

    [Range(1,10)]
    public float thrustMultiplier = 2f;
    public float rollMultiplier = 0.0000002f;
    public float pitchMultiplier = 0.0000002f;
    public float yawMultiplier = 1f;

    // Update is called once per frame
    void FixedUpdate()
    {
        //thrust = Input.GetAxis("Vertical");
        //roll = Input.GetAxis("Horizontal");
        desiredPitch += -1 * Input.GetAxis("alternativeYAxis"); // right hand stick Y
        desiredRoll += -1 * Input.GetAxis("alternativeXAxis"); // right hand stick X
        yawCommand = Input.GetAxis("Horizontal"); // left hand stick X
        thrustCommand = Input.GetAxis("Vertical"); // left hand stick Y

        //if (Input.GetKey(KeyCode.W))
        //    pitchCommand = 1;
        //else if (Input.GetKey(KeyCode.S))
        //    pitchCommand = -1;
        //else
        //    pitchCommand = 0;

        //if (Input.GetKey(KeyCode.D))
        //    rollCommand = 1;
        //else if (Input.GetKey(KeyCode.A))
        //    rollCommand = -1;
        //else
        //    rollCommand = 0;

        //if (Input.GetKey(KeyCode.Q))
        //    yawCommand = -1;
        //else if (Input.GetKey(KeyCode.E))
        //    yawCommand = 1;
        //else
        //    yawCommand = 0;

        //// check thrust input
        //if (Input.GetKeyDown(KeyCode.Equals))
        //    thrustCommand += 0.5f;
        //if (Input.GetKeyDown(KeyCode.Minus))
        //    thrustCommand -= 0.5f;

        if (thrustCommand < 0)
            thrustCommand = 0;

        //Debug.Log("thrust command = " + thrustCommand);
        Debug.Log("roll command = " + rollCommand);
        Debug.Log("pitch command = " + pitchCommand);
        //Debug.Log("yaw command = " + yawCommand);

        
        rollCommand = rollCommandFromPID(desiredRoll);
        pitchCommand = pitchCommandFromPID(desiredPitch);

        // react based on roll command
        if (rollCommand > 0)
        {
            if (this.name == "FL" || this.name == "BL")
            {
                rb.AddForceAtPosition(this.transform.up * rollMultiplier * rollCommand,
                    new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z));

                Debug.DrawRay(this.transform.position, this.transform.up * rollMultiplier * rollCommand, Color.green);
            }
        }
        else if(rollCommand < 0)
        {
            if (this.name == "FR" || this.name == "BR")
            {
                rb.AddForceAtPosition(this.transform.up * rollMultiplier * -1 * rollCommand,
                    new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z));
                Debug.DrawRay(this.transform.position, this.transform.up * rollMultiplier * -1 * rollCommand, Color.green);
            }
        }

        // react based on pitch command
        if (pitchCommand > 0)
        {
            if (this.name == "BL" || this.name == "BR")
            {
                rb.AddForceAtPosition(this.transform.up * pitchMultiplier * pitchCommand,
                    new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z));

                Debug.DrawRay(this.transform.position, this.transform.up * pitchMultiplier * pitchCommand, Color.blue);
            }
        }
        else if (pitchCommand < 0)
        {
            if (this.name == "FL" || this.name == "FR")
            {
                rb.AddForceAtPosition(this.transform.up * pitchMultiplier * -1 * pitchCommand,
                    new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z));
                Debug.DrawRay(this.transform.position, this.transform.up * pitchMultiplier * -1 * pitchCommand, Color.blue);
            }
        }


        // All 4 motor force
        rb.AddForceAtPosition(this.transform.up * thrustMultiplier * thrustCommand,
            new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z));

        Debug.DrawRay(this.transform.position, this.transform.up * thrustMultiplier * thrustCommand, Color.red);


        // Yaw force
        rb.AddRelativeTorque(Vector3.up * yawMultiplier * yawCommand);


    } // void FixedUpdate()



    float previousMeasuredRoll = 0f;
    private float rollCommandFromPID(float desired)
    {

        float kp = 0.0f;
        float kd = 0.0f;

        // calculate roll error
        Vector3 destinationVector = new Vector3(0, 1, 0);

        //float rollError = angleBetweenVectors(destinationVector, this.transform.up);
        float measured = rb.transform.rotation.eulerAngles.x;

        //TODO: I need to map the measured value to a different distribution of numbers...  the values need to be offest because the 0 point is really iffy
        if (measured > 180)
            measured = measured - 360;

        if (measured > 360)
            measured = 0;

        measured = measured * -1f; // necessary?

        float rollError = desired - measured;

        Debug.Log("desired = " + desired);
        Debug.Log("measured = " + measured);
        Debug.Log("rollError = " + rollError);


        // at this point I need to also calculate derivative term
        float deltaMeasured = previousMeasuredRoll - measured; // if the angle between previous and current is changing fast then we need to slow down
        previousMeasuredRoll = measured;
        float Kd = -30f;
        rollError -= Kd * deltaMeasured;



        float returnConstant = 1000f;
        //return rollError * returnConstant;
        return rollError/returnConstant;
    }

    float previousMeasuredPitch = 0f;
    private float pitchCommandFromPID(float desired)
    {

        float kp = 0.0f;
        float kd = 0.0f;

        // calculate roll error
        Vector3 destinationVector = new Vector3(0, 1, 0);

        //float rollError = angleBetweenVectors(destinationVector, this.transform.up);
        float measured = rb.transform.rotation.eulerAngles.z;

        //TODO: I need to map the measured value to a different distribution of numbers...  the values need to be offest because the 0 point is really iffy
        if (measured > 180)
            measured = measured - 360;

        if (measured > 360)
            measured = 0;

        measured = measured * -1f; // necessary?

        float pitchError = desired - measured;

        Debug.Log("desired = " + desired);
        Debug.Log("measured = " + measured);
        Debug.Log("pitchError = " + pitchError);


        // at this point I need to also calculate derivative term
        float deltaMeasured = previousMeasuredPitch - measured; // if the angle between previous and current is changing fast then we need to slow down
        previousMeasuredPitch = measured;
        float Kd = -30f;
        pitchError -= Kd * deltaMeasured;



        float returnConstant = 1000f;
        //return rollError * returnConstant;
        return pitchError / returnConstant;
    }

    private float angleBetweenVectors(Vector3 destination, Vector3 initial)
    {
        // the vector that we want to measure an angle from
        Vector3 referenceForward = /* some vector that is not Vector3.up */
                                   // the vector perpendicular to referenceForward (90 degrees clockwise)
                                   // (used to determine if angle is positive or negative)

        referenceForward = this.transform.up;

        Vector3 referenceRight = Vector3.Cross(Vector3.up, referenceForward);
        // the vector of interest
        Vector3 newDirection = /* some vector that we're interested in */
                               // Get the angle in degrees between 0 and 180

        newDirection = destination;

        float angle = Vector3.Angle(newDirection, referenceForward);
        // Determine if the degree value should be negative.  Here, a positive value
        // from the dot product means that our vector is on the right of the reference vector   
        // whereas a negative value means we're on the left.
        float sign = Mathf.Sign(Vector3.Dot(newDirection, referenceRight));
        float finalAngle = sign * angle;

        return finalAngle;
    }
}
