using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HAVIGAME;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D body;
   
    public const float maxSpeedOnLand = 4f;
    [SerializeField]
    private float accelerationSpeed = 0.07f;
    [SerializeField]
    private float deaccelerationSpeed = 1f;
    private float moveSpeedX = 0;
    private float moveSpeedY = 0;
    public float maxSpeed = 4f;
    public float rotateSpeed = 10f;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        CameraController.Instance.AddTarget(this.transform, CameraMode.Follow);
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        calculateSpeed(h,v);
        
    }

    private void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
      
        if (h!=0 || v!=0)
        {
            RotateOnKey(h, v);
        }
        moveOnKey(h, v);
        stopBodyVelocity();
    }

    private void calculateSpeed(float h, float v)
    {
        if (h > 0) moveSpeedX += accelerationSpeed;
        if (h < 0) moveSpeedX -= accelerationSpeed;
        if (h == 0) 
        {
            //deaccelaration to standing still (the speed cant go over zero as it will cause the tank to change moving direction)
            if (moveSpeedX > 0) 
            {
                moveSpeedX -= deaccelerationSpeed;
                moveSpeedX = Mathf.Clamp(moveSpeedX, 0, maxSpeed);
            }
            if (moveSpeedX < 0)
            {
                moveSpeedX += deaccelerationSpeed;
                moveSpeedX = Mathf.Clamp(moveSpeedX, -maxSpeed, 0);
            }
        }

        if (v > 0) moveSpeedY += accelerationSpeed;
        if (v < 0) moveSpeedY -= accelerationSpeed;
        if (v == 0)
        {
            if (moveSpeedY > 0)
            {
                moveSpeedY -= deaccelerationSpeed;
                moveSpeedY = Mathf.Clamp(moveSpeedY, 0, maxSpeed);
            }
            if (moveSpeedY < 0)
            {
                moveSpeedY += deaccelerationSpeed;
                moveSpeedY = Mathf.Clamp(moveSpeedY, -maxSpeed, 0);
            }
        }
        moveSpeedX = Mathf.Clamp(moveSpeedX, -maxSpeed, maxSpeed);
        moveSpeedY = Mathf.Clamp(moveSpeedY, -maxSpeed, maxSpeed);

        //Debug.Log(moveSpeedX + "," + moveSpeedY);
    }

    private void moveOnKey(float h, float v)
    {
        Vector2 pos = transform.position;
        pos.x += moveSpeedX * Time.deltaTime;
        pos.y += moveSpeedY * Time.deltaTime;
        transform.position = pos;

        //move by rigidbody
        //Vector2 moveDir = new Vector2(h, v);
        //gameObject.GetComponent<Rigidbody2D>().AddForce(moveDir, ForceMode2D.Force);
    }

    private void RotateOnKey(float h, float v)
    {
        Vector2 lookDir = new Vector2(h, v);
        //Quaternion targetRot = Quaternion.FromToRotation(Vector2.up, lookDir);
        Quaternion lookRot = Quaternion.LookRotation(lookDir, Vector3.back);
        Quaternion targetRot = Quaternion.RotateTowards(transform.rotation, lookRot, rotateSpeed);
        if (targetRot.x != 0) targetRot.x = 0;
        if (targetRot.y != 0) targetRot.y = 0;

        transform.rotation = targetRot;
    }

    //stop the physics affecting control after collision with other objects
    void stopBodyVelocity()
    {
        if (body.velocity != Vector2.zero) body.velocity = Vector2.zero;
        if (body.angularVelocity != 0) body.angularVelocity = 0;
    }

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // if in water -> slow down max speed
        if (collision.gameObject.layer == 4)
        {
            maxSpeed = 2.5f;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 4)
        {
            maxSpeed = maxSpeedOnLand;
        }
    }
}
