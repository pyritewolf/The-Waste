using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private float freeFallSpeed;
    private Animator anim;
    private CharacterController cc;
    private float savedGravity;

    public float speed;
    public float rotationSpeed;
    public float gravity;
    public float jumpSpeed;


    // Use this for initialization
    void Awake()
    {
        anim = GetComponent<Animator>();
        cc = GetComponent<CharacterController>();
        savedGravity = gravity;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (cc.isGrounded)
        {
            if (Input.GetButton("Jump"))
            {
                freeFallSpeed = jumpSpeed * (-1);
            }
            else
            {
                freeFallSpeed = 0;
            }
        }

        // gestión de gravedad con CC sin RB
        freeFallSpeed += gravity * Time.deltaTime;
        Vector3 movement = freeFallSpeed * Vector3.down;

        float mov = Input.GetAxis("Vertical") * speed;
        movement += mov * transform.forward * Time.deltaTime;

        cc.Move(movement);
        float movementPercentage = mov / speed;
        anim.SetFloat("speed", movementPercentage);
        anim.SetFloat("vSpeed", freeFallSpeed);


        float rotationMovement = Input.GetAxis("Horizontal") * rotationSpeed;
        //cc.angularVelocity =new Vector3(0, rotationMovement, 0);
        transform.Rotate(new Vector3(0, rotationMovement * Time.deltaTime, 0));


    }

    public void PickObject(Transform objetivo)
    {
        Vector3 diff = objetivo.position - transform.position;
        float dist = diff.magnitude;
        Vector3 dir = diff / dist;
        transform.position += dir * speed * Time.deltaTime;
        transform.forward = dir;
    }

    void OnTriggerEnter()
    {

    }

    //reemplaza on collission enter
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //de donde me pegaronnn?
        //bitmask!
        if ((cc.collisionFlags & CollisionFlags.Sides) != 0 && (cc.collisionFlags & CollisionFlags.Above) != 0)
        {
            //whatevsss
        }
    }

}

