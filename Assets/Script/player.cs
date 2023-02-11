using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    public float speed;
    float hAxis;
    float vAxis;
    bool rDown;
    bool isCrawl = false;
    bool jDown = false;
    bool fDown = false;
    bool isJump=false;
    bool isInWater=false;
    bool isfloating = false;
    bool headToSlope = false;

    Vector3 moveVec;
    Vector3 headVec;

    Rigidbody rigid;
    public CapsuleCollider stand;
    public BoxCollider lie;
    public SphereCollider liehead;
    public ParticleSystem splash;
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
        splash.Stop();
        isfloating = false;
        anim.SetBool("IsFloating", false);
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
    }   
    void GetInput()
    {
        vAxis = Input.GetAxisRaw("Horizontal");
        hAxis = Input.GetAxisRaw("Vertical");
        rDown = Input.GetButton("Run");
        jDown = Input.GetButtonDown("Jump");
        fDown = Input.GetButton("Jump");
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
        /*
        if(isSwim)
        {
            if (Input.GetButtonDown("Horizontal") || Input.GetButtonDown("Vertical"))
                splash.Play();
            if (Input.GetButtonUp("Horizontal") || Input.GetButtonUp("Vertical"))
                splash.Stop();
        }
        */
        transform.position += moveVec * speed * ((rDown&&!isJump)&&!isCrawl && !headToSlope ? 1f : 0.3f) * Time.deltaTime;

        anim.SetBool("IsWalk", moveVec != Vector3.zero);
        anim.SetBool("IsRun", rDown);
        anim.SetBool("IsCrawl", isCrawl);
    }

    void Turn()
    {
        transform.LookAt(transform.position + headVec);
    }
    void Jump()
    {
        if (jDown && !isJump&&!isCrawl!&&!isfloating)
        {
            rigid.AddForce(Vector3.up * 400,ForceMode.Impulse);
            anim.SetBool("IsJump",true);
            anim.SetBool("IsJump", true);
            isJump = true;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "floor")
        {
            isfloating = false;
            anim.SetBool("IsFloating", false);

            if (isJump)
            {
                isJump = false;
                anim.SetBool("IsJump", false);
            }
        }
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
        if (collision.gameObject.tag == "floor")
        {
            if (isInWater)
            {
                isfloating = true;
                anim.SetBool("IsFloating", true);
                //stand.height = 3;
                //stand.center = new Vector3(0f, 1.5f ,0f);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "underwater")
        {
            isInWater = true;   
            isJump = false;
            anim.SetBool("IsJump", false);
            isCrawl = false;
            anim.SetBool("IsCrawl", false);
        }
    }
    private void OnTriggerStay(Collider other)
    {       
        if (other.gameObject.tag == "water")
        {
            isInWater = true;
            rigid.AddForce(Vector3.down * 20, ForceMode.Acceleration);
        }
        if (other.gameObject.tag == "underwater")
        {
            isInWater = true;
            rigid.AddForce(Vector3.up * 20, ForceMode.Acceleration);
            if (isfloating)
            {
                anim.SetBool("IsFloating", true);               
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        headToSlope= false;
        if (other.gameObject.tag == "underwater")
        {
            isInWater = false;
        }
    }
}
