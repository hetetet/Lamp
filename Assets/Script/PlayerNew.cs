using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNew : MonoBehaviour
{
    public float speed;
    float hAxis;
    float vAxis;
    bool rDown;
    bool jDown = false;

    bool isCrawl = false;
    bool isJump = false;
    bool isSwim = false;

    bool headToSlope = false;

    Vector3 moveVec;
    Vector3 headVec;

    Rigidbody rigid;
    public CapsuleCollider stand;
    public BoxCollider lie;
    public SphereCollider liehead;
    public ParticleSystem splash;
    public BoxCollider watersensor_waist;
    public BoxCollider watersensor_foot;
    ParticleSystem.ShapeModule _editableShape;
    /*
    public Rigidbody rigidBody;
    public float depthBeforeSubmerged = 1f;
    public float displacementAmount = 1f;
    */
    Animator anim;
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        _editableShape = splash.shape;
        splash.Stop();
    }

    private void Start()
    {

    }
    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        makeSplash();
    }
    void GetInput()
    {
        vAxis = Input.GetAxisRaw("Horizontal");
        hAxis = Input.GetAxisRaw("Vertical");
        rDown = Input.GetButton("Run");
        jDown = Input.GetButtonDown("Jump");
    }

    void Move()
    {
        moveVec = new Vector3(vAxis, 0, hAxis).normalized;
        headVec = new Vector3(hAxis, 0, -vAxis).normalized;
        if (Input.GetButtonDown("Crawl"))
        {
            isCrawl = !isCrawl;
            if (isCrawl)
            {
                stand.enabled = false;
                lie.enabled = true;
                liehead.enabled = true;
            }
            else
            {
                stand.enabled = true;
                lie.enabled = false;
                liehead.enabled = false;
            }
        }


        transform.position += moveVec * speed * ((rDown && !isJump) && !isCrawl && !headToSlope ? 1f : 0.3f) * Time.deltaTime;
        anim.SetBool("isWalk", moveVec != Vector3.zero);
        anim.SetBool("isRun", rDown);
        anim.SetBool("isCrawl", isCrawl);
    }

    void Turn()
    {
        transform.LookAt(transform.position + headVec);
    }
    void Jump()
    {
        if (jDown && !isJump && !isCrawl! && !isSwim)
        {
            rigid.AddForce(Vector3.up * 400, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            anim.SetBool("isJump", true);
            isJump = true;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "floor")
        {

            if (isJump)
            {
                isJump = false;
                anim.SetBool("isJump", false);
            }
        }
    }

    public void makeSplash()
    {
        if(!splash.isPlaying && moveVec != Vector3.zero && isSwim)
            splash.Play();
        else if(!isSwim || (vAxis==0 && hAxis==0))
            splash.Stop();
    }
    private void OnCollisionStay(Collision collision)
    {
        Vector3 normalVec = collision.contacts[0].normal;
        if (Mathf.Abs(normalVec.x) > 0.4 || Mathf.Abs(normalVec.z) > 0.4)
            headToSlope = true;
        else
            headToSlope = false;
    }
    private void OnCollisionExit(Collision collision)
    {

    }
    private void OnTriggerEnter(Collider other)
    {

    }

    private void OnTriggerStay(Collider other)
    {
        
        if (other.gameObject.tag == "water")
        {
            isSwim = true;
            rigid.AddForce(Vector3.down * 20, ForceMode.Acceleration);
            anim.SetBool("isSwim", true);            
            _editableShape.position=new Vector3(0, other.gameObject.transform.position.y - transform.position.y+1, 0);    
        }
        if (other.gameObject.tag == "underwater")
        {
            isSwim = true;
            rigid.AddForce(Vector3.up * 20, ForceMode.Acceleration);
            anim.SetBool("isSwim", true);
            _editableShape.position = new Vector3(0, other.gameObject.transform.position.y - transform.position.y + 1, 0);
        }

        if (!isSwim)
        {
            isSwim = true;
            anim.SetBool("isSwim", true);
        }

        if (isJump)
        {
            isJump = false;
            anim.SetBool("isJump", false);
        }

        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "water")
        {           
            rigid.AddForce(Vector3.down * 20, ForceMode.Acceleration);
            isSwim = false;
            anim.SetBool("isSwim", false);
        }
    }

    public void getInWater()
    {
        Debug.Log("Get in the water");
        //isInWater = true;
        //anim.SetBool("isSwim", true);
    }

    public void getOutWater()
    {
        Debug.Log("Get out of the water");
        //isInWater = false;
        //anim.SetBool("isSwim", false);
    }
}
