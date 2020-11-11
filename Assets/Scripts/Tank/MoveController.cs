using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class MoveController : MonoBehaviour
{
    public float MaxSpeed = 2f;
    public float MinSpeed = -1f;

    public Vector2 currentDirection;
    public int ForwardAccelerate = 4;
    public int BackAccelerate = -2;

    private int rotateSpeed = 30;

    public float currentSpeed;
    private bool isInit;


    public Vector3 destination;

    // Start is called before the first frame update
    void Start()
    {
        Debug.DrawRay(gameObject.transform.position, gameObject.transform.up * 5, Color.green, 60f);
        Debug.DrawRay(gameObject.transform.position, new Vector3(2.05f, -1.66f, 0) * 5, Color.green, 60f);
    }

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.W))
        {
            if (currentSpeed <= MaxSpeed)
            {
                var tempSpeed = ForwardAccelerate * Time.fixedUnscaledDeltaTime + currentSpeed;
                currentSpeed = tempSpeed > MaxSpeed ? MaxSpeed : tempSpeed;
            }
        }

        if (Input.GetKey(KeyCode.S))
        {
            if (currentSpeed >= MinSpeed)
            {
                var tempSpeed = BackAccelerate * Time.fixedUnscaledDeltaTime + currentSpeed;
                currentSpeed = tempSpeed < MinSpeed ? MinSpeed : tempSpeed;
            }
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.forward, -rotateSpeed * Time.fixedUnscaledDeltaTime);
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward, rotateSpeed * Time.fixedUnscaledDeltaTime);
        }


        if (Input.GetKey(KeyCode.Space))
        {
            var posX = gameObject.transform.position.x;
            var posY = gameObject.transform.position.y;
            var posZ = gameObject.transform.position.z;

            Debug.Log($"X: {posX}, Y: {posY}, Z: {posZ}");
        }

        //var newDirection = currentDirection * 2;

        transform.Translate(currentSpeed * Time.fixedUnscaledDeltaTime * gameObject.transform.up, Space.World);
    }
}
