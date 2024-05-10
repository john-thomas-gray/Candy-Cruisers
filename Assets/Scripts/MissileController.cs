using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileController : MonoBehaviour
{
    // Missile color
    public GameObject player;
    // Destroy Timer
    private float destroyTimer = 0;

    private float destroyPlain = 5.3f;
    public bool homing;
    private float initialZRotation;
    private float rotationSpeed = 2f;
    private float maxRotationDifference = 35f;

    // Collision Detections
    Vector3 StartPoint;
    Vector3 Origin;
    public int NumRays = 10;
    RaycastHit HitInfo;
    float LengthOfRay, DistanceBetweenRays, DirectionFactor;
    float margin = 0.015f;
    Ray ray;

    void Start()
    {
        player = GameObject.Find("Player");
        initialZRotation = transform.eulerAngles.z;

        if (gameObject.name.Contains("Homing"))
        {
            homing = true;
            Debug.Log("working");
        }
        else
        {
            homing = false;
            Debug.Log("not working");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Set raycast values
        LengthOfRay = collider.bounds.extends.y;
        DirectionFactor = Mathf.Sign (Vector3.up.y);
        if (!IsCollidingVertically ()) {
            // call death on player
        }
        MoveMissile();
        // Destroy missile if onscreen too long
        destroyTimer += Time.deltaTime;
        if(destroyTimer > 2f)
        {
            Destroy(this.gameObject);
        }
        // Destroy off-screen missiles
        if(Mathf.Abs(transform.position.y) > destroyPlain)
        {
            Destroy(this.gameObject);
        }


    }

    void MoveMissile()
    {

        float missileSpeed = 5.0f;
        if (homing == false)
        {
            transform.Translate(Vector3.up * missileSpeed * Time.deltaTime);
        } else
        {
            Homing();
        }
    }

    void Homing()
    {
        float homingSpeed = 5f;
        Debug.Log("homing");
        transform.Translate(Vector3.up * homingSpeed * Time.deltaTime);

        // Calculate potential new rotation values
        float newZRotationClockwise = transform.eulerAngles.z + rotationSpeed;
        float newZRotationCounterclockwise = transform.eulerAngles.z - rotationSpeed;

        if (player.transform.position.x > transform.position.x)
        {
            // Rotate clockwise if within bounds
            if (Mathf.Abs(newZRotationClockwise - initialZRotation) <= maxRotationDifference)
            {
                transform.rotation = Quaternion.Euler(0, 0, newZRotationClockwise);
            }
        }
        else if (player.transform.position.x < transform.position.x)
        {
            // Rotate counterclockwise if within bounds
            if (Mathf.Abs(newZRotationCounterclockwise - initialZRotation) <= maxRotationDifference)
            {
                transform.rotation = Quaternion.Euler(0, 0, newZRotationCounterclockwise);
            }
        }

    }

    bool IsCollidingVertically ()
    {
        Origin = StartPoint;
        DistanceBetweenRays = (collider.bounds.size.x - 2 * margin) / (NumRays - 1);
        for (i = 0; i<NumRays; i++) {
            // Ray to be casted.
            ray = new Ray (Origin, Vector3.up * DirectionFactor);
            //Draw ray on screen to see visually. Remember visual length is not actual length.
            Debug.DrawRay (Origin, Vector3.up * DirectionFactor, Color.yellow);
            if (Physics.Raycast (ray, out HitInfo, LengthOfRay)) {
                print ("Collided With " + HitInfo.collider.gameObject.name);
                // Negate the Directionfactor to reverse the moving direction of colliding cube(here cube2)
                DirectionFactor = -DirectionFactor;
                return true;
            }
            Origin += new Vector3 (DistanceBetweenRays, 0, 0);
        }
        return false;
    }
}

}
