/* 
 * author : jiankaiwang
 * description : The script provides you with basic operations of first personal control.
 * platform : Unity
 * date : 2017/12
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour {

    public float speed = 10.0f;
    public float translation;
    public float straffe;
    public Camera camera;
    public Rigidbody rb;
    public Transform head;



     [Header("Camera Effects")]
    public float baseCameraFov = 60f;
    public float baseCameraHeight = .85f;

    public float walkBobbingRate = .75f;
    public float runBobbingRate = 1f;
    public float maxWalkBobbingOffset = .2f;
    public float maxRunBobbingOffset = .3f;

    public float cameraShakeThreshold = 10f;
    [Range(0f, 0.03f)] public float cameraShakeRate = .015f;
    public float maxVerticalFallShakeAngle = 40f;
    public float maxHorizontalFallShakeAngle = 40f;

       [Header("Configurations")]
    public float walkSpeed;
    public float runSpeed;
    public float jumpSpeed;
    public float impactThreshold;
    public float itemPickupDistance;

      [Header("Runtime")]
    Vector3 newVelocity;
    bool isGrounded = false;
    bool isJumping = false;
    float vyCache;
    string activeAudioName = "default";
    Transform attachedObject = null;
    float attachedDistance = 0f;

    // Use this for initialization
    void Start () {
        // turn off the cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;		
	}
	
	// Update is called once per frame
	void Update () {

        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * 2f);   // Adjust the multiplier for different rotation speed

        newVelocity = Vector3.up * rb.velocity.y;
        // ////////////////////////////////////////////
        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        newVelocity.x = Input.GetAxis("Horizontal") * speed;
        newVelocity.z = Input.GetAxis("Vertical") * speed;

        // float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        newVelocity.x = Input.GetAxis("Horizontal") * speed;
        newVelocity.z = Input.GetAxis("Vertical") * speed;

        if (isGrounded) {
            if (Input.GetKeyDown(KeyCode.Space) && !isJumping) {
                newVelocity.y = jumpSpeed;
                isJumping = true;
            }
        }
        rb.velocity = transform.TransformDirection(newVelocity);
        bool isMovingOnGround = (Input.GetAxis("Vertical") != 0f || Input.GetAxis("Horizontal") != 0f) && isGrounded;
        
        //  Head bob
        if (isMovingOnGround) {
        float bobbingRate = Input.GetKey(KeyCode.LeftShift) ? runBobbingRate : walkBobbingRate;
        float bobbingOffset = Input.GetKey(KeyCode.LeftShift) ? maxRunBobbingOffset : maxWalkBobbingOffset;
        Vector3 targetHeadPosition = Vector3.up * baseCameraHeight + Vector3.up * (Mathf.PingPong(Time.time * bobbingRate, bobbingOffset) - bobbingOffset*.5f);
        head.localPosition = Vector3.Lerp(head.localPosition, targetHeadPosition, .1f);
        }
        // Input.GetAxis() is used to get the user's input
        // You can furthor set it on Unity. (Edit, Project Settings, Input)
        translation = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        straffe = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        transform.Translate(straffe, 0, translation);

        if (Input.GetKeyDown("escape")) {
            // turn on the cursor
            Cursor.lockState = CursorLockMode.None;
        }
    }



    //////////////
    //  void FixedUpdate() {
    //     //  Shoot a ray of 1 unit towards the ground
    //     if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1f)) {
    //         isGrounded = true;

    //         //  Check the ground tag for different walking sounds
    //         //  Edit this part to check for the other tags of yours!
    //         if (hit.transform.tag == "concrete")
    //             activeAudioName = "concrete";
    //         else activeAudioName = "default";
    //     }
    //     else isGrounded = false;

    //     //  Cache the velocity in the y direction
    //     vyCache = rb.velocity.y;
    // }
    //  void OnCollisionStay(Collision col) {
    //     if (Vector3.Dot(col.GetContact(0).normal, Vector3.up) <= .3f)
    //         return;

    //     isGrounded = true;
    //     isJumping = false;

    //     if (col.transform.tag == "concrete")
    //         activeAudioName = "concrete";
    //     else activeAudioName = "default";
    // }
        void LateUpdate() {
        // Vertical rotation
        Vector3 e = head.eulerAngles;
        e.x -= Input.GetAxis("Mouse Y") * 2f;   //  Edit the multiplier to adjust the rotate speed
        e.x = RestrictAngle(e.x, -85f, 85f);    //  This is clamped to 85 degrees. You may edit this.
        head.eulerAngles = e;
        }
//////////////



        public static float RestrictAngle(float angle, float angleMin, float angleMax) {
        if (angle > 180)
            angle -= 360;
        else if (angle < -180)
            angle += 360;

        if (angle > angleMax)
            angle = angleMax;
        if (angle < angleMin)
            angle = angleMin;

        return angle;
        }
}
