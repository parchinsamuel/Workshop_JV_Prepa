using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    private void Awake()
    {
        Instance = this;
        startYScale = transform.localScale.y;
    }

    public Vector2 input;

    [Header("Movement")]
    public Rigidbody rb;
    public Transform cameraLookerTransform;
    public bool moving;
    public bool readyToRun;

    [SerializeField] float speed;
    [SerializeField] float acc;
    [SerializeField] float runForce;

    Vector2 smoothedInput;
    Vector2 inputVelocity;

    [Header("Jumping")]
    [SerializeField] float botRayHeight;
    [SerializeField] float botRaySize;
    [SerializeField] float botRayOffset;
    public float jumpDuration;

    [SerializeField] AnimationCurve jumpCurve;
    [SerializeField] AnimationCurve jumpDirectionCurve;

    [SerializeField] LayerMask ignoredLayer;

    [HideInInspector]
    public float jumpTimer;
    [HideInInspector]
    public bool jumping;
    [HideInInspector]
    public bool touchingGround;

    Vector3 jumpDirection;
    Vector3 moveVector;

    [HideInInspector]
    public Vector3 vel;
    [HideInInspector]
    public bool running;
    public bool applyingRunForce;

    [Header("Input")]
    [SerializeField] private KeyCode _slideKey = KeyCode.LeftControl;

    [Header("Slide")]
    [SerializeField] private Transform playerObj;

    [SerializeField] private float maxSlideTime;
    [SerializeField] private float slideForce = 10f;

    [SerializeField] private float slideVelocityMultiplier = 1.5f;
    [SerializeField] private float minSlideSpeed = 5f;
    [SerializeField] private float maxSlideSpeed = 15f;

    [SerializeField] private float slideYScale;

    [SerializeField] private float minSlideTime = 0.5f;

    [SerializeField] private float slopeAngleStartThreshold = 5f;
    [SerializeField] private float slopeDotThreshold = 0.1f;

    private float startYScale;
    private float slideTimer;
    private bool sliding;

    private Vector3 currentSlopeNormal = Vector3.up;
    private float currentSlopeAngle = 0f;

    private Vector2 slideInput;

    public void PlayerControlUpdate()
    {
        InputUpdate();

        GroundUpdate();

        JumpUpdate();
    }


    void InputUpdate()
    {
        moving = false;
        input = Vector2.zero;
        applyingRunForce = false;


        if (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            input.y += 1;

            if (!touchingGround)
            {
                if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    readyToRun = !readyToRun;

                    if (running)
                    {
                        StopRunning();

                        readyToRun = false;
                    }


                }

            }
            else
            {
                if(running)
                {
                    if (Input.GetKeyDown(KeyCode.LeftShift))
                    {
                        StopRunning();
                    }
                }
                else
                {
                    if (Input.GetKeyDown(KeyCode.LeftShift) || readyToRun)
                    {
                        StartRunning();
                    }
                }

                
            }


            if(running)
            {
                applyingRunForce = true;
            }
        }
        if (Input.GetKeyDown(_slideKey) && touchingGround && !sliding)
        {
            StartSlide();
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            input.y -= 1;
            if(running)
            {
                StopRunning();
            }
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow))
        {
            input.x -= 1;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            input.x += 1;
        }

        if (input.magnitude > 0) moving = true;
        else
        {
            if (running) StopRunning();

            if(Input.GetKey(KeyCode.LeftShift))
            {
                readyToRun = true;
            }
            else
            {
                readyToRun = false;
            }
        }

        input = input.normalized;

        smoothedInput = Vector2.SmoothDamp(smoothedInput, input, ref inputVelocity, acc, 999f, Time.deltaTime);

    }

    void StartRunning()
    {
        readyToRun = false;
        running = true;

    }

    void StopRunning()
    {
        running = false;
    }


    void StartSlide()
    {
        sliding = true;

        slideInput = input;

        playerObj.localScale = new Vector3(playerObj.localScale.x, slideYScale, playerObj.localScale.z);

        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        Vector3 flatVelocity = rb.linearVelocity;
        flatVelocity.y = 0f;

        float currentSpeed = flatVelocity.magnitude;

        float finalSlideSpeed = Mathf.Clamp(
            currentSpeed * slideVelocityMultiplier,
            minSlideSpeed,
            maxSlideSpeed
        );

        Vector3 slideDirection;

        if (input.sqrMagnitude > 0.01f)
        {
            Vector3 flatForward = cameraLookerTransform.forward;
            flatForward.y = 0f;

            Vector3 flatRight = cameraLookerTransform.right;
            flatRight.y = 0f;

            slideDirection = flatForward.normalized * input.y + flatRight.normalized * input.x;
        }
        else
        {
            slideDirection = cameraLookerTransform.forward;
            slideDirection.y = 0f;
        }

        slideDirection.Normalize();

        rb.linearVelocity = slideDirection * finalSlideSpeed + Vector3.up * rb.linearVelocity.y;

        bool wasMoving = currentSpeed > 0.1f;
        slideTimer = wasMoving ? maxSlideTime : minSlideTime;
    }

    void SlidingMovement()
    {
        Vector3 inputDirection = playerObj.forward * slideInput.y + playerObj.right * slideInput.x;

        bool onSlope = OnSlope();
        Vector3 slopeMoveDir = Vector3.zero;
        if (onSlope)
        {
            slopeMoveDir = GetSlopeMoveDirection();
        }

        if(onSlope)
        {
            Vector3 inputOnSlope = Vector3.ProjectOnPlane(inputDirection, currentSlopeNormal).normalized;
            Vector3 finalDir = (slopeMoveDir * 0.9f + (inputOnSlope * 0.1f)).normalized;

            rb.AddForce(finalDir * slideForce, ForceMode.Force);

            float downDot = Vector3.Dot(rb.linearVelocity.normalized, slopeMoveDir);

            bool slidingDownSlope = rb.linearVelocity.sqrMagnitude > 0.01f && downDot > slopeDotThreshold;

            if (!slidingDownSlope)
            {
                slideTimer -= Time.deltaTime * 2f;
            }
        }
        else
        {
            Vector3 dir = (inputDirection.sqrMagnitude > 0.01f) ? inputDirection.normalized : playerObj.forward;
            rb.AddForce(dir * slideForce, ForceMode.Force);

            slideTimer -= Time.deltaTime;
        }

        if (slideTimer <= 0)
            StopSlide();
    }

    void StopSlide()
    {
        sliding = false;

        playerObj.localScale = new Vector3(playerObj.localScale.x, startYScale, playerObj.localScale.z);
    }

    public void MoveUpdate()
    {
        vel = Vector3.zero;

        Vector3 flatForward = cameraLookerTransform.forward;
        flatForward.y = 0;
        Vector3 flatRight = cameraLookerTransform.right;
        flatRight.y = 0;

        moveVector = (flatForward.normalized * smoothedInput.y) + (flatRight.normalized * smoothedInput.x);

        vel = moveVector * speed;

        if (sliding)
        {
            vel = Vector3.zero;

            SlidingMovement();
            vel = rb.linearVelocity;

            if (sliding && !touchingGround)
            {
                StopSlide();
                vel.y = jumpCurve.Evaluate(1f);

            }
        }
        else
        {
            if (applyingRunForce)
            {
                vel += new Vector3(cameraLookerTransform.forward.x, 0, cameraLookerTransform.forward.z).normalized * runForce;
            }

            if(jumping)
            {
                vel.y = jumpCurve.Evaluate(1f - (jumpTimer / jumpDuration));
 
                vel += jumpDirection * jumpDirectionCurve.Evaluate(1f - jumpTimer / jumpDuration);

                if (touchingGround)
                {
                    if (vel.y < 0f) vel.y = 0f;
                }
            }
            else if(!touchingGround)
            {
                vel.y = jumpCurve.Evaluate(1f);
            }
        }

        rb.linearVelocity = vel;
    }

    public void StopUpdate()
    {
        rb.linearVelocity = Vector3.zero;
    }

    void JumpUpdate()
    {
        jumping = false;

        if (jumpTimer > 0f)
        {
            jumpTimer -= Time.deltaTime;
            if (jumpTimer <= 0f) jumpTimer = 0f;

            jumping = true;
        }
        else
        {
            jumping = false;
        }
 
        //touchingGround = Physics.BoxCast(transform.position + (transform.up * botRayHeight), Vector3.one * botRaySize / 2f, -transform.up, Quaternion.identity, botRaySize, ~ignoredLayer); ;

        if(touchingGround)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }
        }
    }

    void Jump()
    {
        jumpTimer = jumpDuration;

        //jump direction
        jumpDirection = vel;

    }

    void GroundUpdate()
    {
        touchingGround = false;

        Ray[] rays = new Ray[4];
        Vector3 start = transform.position + (transform.up * botRayHeight);
        Vector3 offset = new Vector3(botRayOffset, 0, botRayOffset);
        rays[0] = new Ray(start + offset, -transform.up + offset);
        offset = new Vector3(-botRayOffset, 0, botRayOffset);
        rays[1] = new Ray(start + offset, -transform.up + offset);
        offset = new Vector3(botRayOffset, 0, -botRayOffset);
        rays[2] = new Ray(start + offset, -transform.up + offset);
        offset = new Vector3(-botRayOffset, 0, -botRayOffset);
        rays[3] = new Ray(start + offset, -transform.up + offset);

        for (int i = 0; i < 4; i++)
        {
            Debug.DrawRay(rays[i].origin, rays[i].direction*botRaySize);
            if(Physics.Raycast(rays[i], botRaySize, ~ignoredLayer))
            {
                touchingGround = true;
            }
        }

        RaycastHit hit;
        Vector3 centerStart = transform.position + (transform.up * botRayHeight);
        if (Physics.Raycast(centerStart, -transform.up, out hit, botRaySize + 0.1f, ~ignoredLayer))
        {
            currentSlopeNormal = hit.normal;
            currentSlopeAngle = Vector3.Angle(hit.normal, Vector3.up);
        }
        else
        {
            currentSlopeNormal = Vector3.up;
            currentSlopeAngle = 0f;
        }

        //touchingGround = Physics.Raycast(transform.position + (transform.up * botRayHeight), -transform.up, botRaySize, ~ignoredLayer);
    }

    bool OnSlope()
    {
        return touchingGround && currentSlopeAngle > slopeAngleStartThreshold;
    }

    Vector3 GetSlopeMoveDirection()
    {
        Vector3 down = Vector3.down;
        Vector3 slopeDir = Vector3.ProjectOnPlane(down, currentSlopeNormal);
        if (slopeDir.sqrMagnitude < 0.0001f) return Vector3.zero;
        return slopeDir.normalized;
    }

    public void Die()
    {

    }
}
