using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementV2 : Movement {

    public int drunkMod;
    public float slownessSeverity;
    public float linearDrag;

    public float jumpForce;
    Vector3 moveDir;

    [SerializeField] bool falling;
    [SerializeField] float yMove;

    public CharacterController charCon;
    Coroutine movementTakeover;

    // public PlayerMagic myPlayerMagic;

	// Use this for initialization
	public override void Start () {
        // setup();
        charCon = GetComponent<CharacterController>();
        charCon.detectCollisions = true;
        yMove = -3f;
        falling = true;
	}

    // Update is called once per frame
    public override void Update () {
        processMovement();
	}

    void FixedUpdate()
    {
        if (yMove > Physics.gravity.y) { yMove += Time.deltaTime * Physics.gravity.y; }
        Vector3 move = moveDir * slownessSeverity * drunkMod; // Get the total movement
        // if(hamper <= 0) { Move(move * Time.deltaTime); }
        Move(move * Time.deltaTime);
        if(charCon != null && charCon.enabled) { charCon.Move(Vector3.up * yMove * Time.deltaTime); }
    }

    void calculateMove()
    {
        /*
            Vector3 dirNorm = move.normalized; // get direction of movement
            float distCheck = dirNorm.magnitude * Time.deltaTime; // get total distance moved
            RaycastHit[] hits = rbody.SweepTestAll(dirNorm, distCheck, QueryTriggerInteraction.Ignore);
            bool okayToMove = true;
            if(hits.Length != 0) { // check if collision will occur
                foreach(RaycastHit hit in hits) {
                    Debug.Log(hit.collider.name);
                    if(!hit.collider.tag.Contains("Furniture") || !hit.collider.tag.Contains("Ground")) {
                        okayToMove = false;
                    }
                }
            }
            if (okayToMove) { rbody.MovePosition(rbody.position + move); } // if it's okay to move, then move
            */
    }

    public override void Move(Vector3 movement)
    {
        if (charCon == null || !charCon.enabled) { return; }
        if (charCon.isGrounded) {
            currVel = Vector3.Lerp(currVel, movement, friction);
            charCon.Move(currVel);
        }
        else {
            currVel = Vector3.Lerp(currVel, movement, friction * 0.1f);
            charCon.Move(currVel);
        }
    }

    public override void Teleport(Vector3 newLocation)
    {
        Vector3 dir = (newLocation + Vector3.up * charCon.height) - transform.position;
        if (charCon.enabled) { charCon.Move(dir); }
    }

    public override void setup()
    {
        // Do NOT give this a state machine!
        // Do NOT run base.setup() on this!
        rbody = GetComponent<Rigidbody>();
        currSpeed = maxSpeed;
    }

    public override void processMovement()
    {
        float horizontal = 0f;
        float vertical = 0f;
        // if (hamper > 0) { return; }
        horizontal = Input.GetAxis("Horizontal"); // Get player inputs
        vertical = Input.GetAxis("Vertical"); // Get player inputs

        if (Input.GetKeyDown(KeyCode.Space)) { Jump(); }
        moveDir = ((transform.forward * vertical * currSpeed) + (transform.right * horizontal * currSpeed));
    }

    public override void knockBack(Vector3 dir, float force)
    {
        if(movementTakeover != null) {
            StopCoroutine(movementTakeover);
            hamper--;
        }
        Vector3 knock = dir * force;
        movementTakeover = StartCoroutine(knockingBack(knock));
    }

    IEnumerator knockingBack(Vector3 force) {
        hamper++;
        Vector3 knock = force;
        float time = 0f;
        yMove = force.y;
        knock.y = 0;
        Vector3 start = knock;

        falling = true;

        while (time < .3f) {
            if(charCon.isGrounded) { time += Time.deltaTime; }
            Move(knock * Time.deltaTime);
            knock = Vector3.Lerp(knock, Vector3.zero, 0.5f);
            yield return new WaitForEndOfFrame();
        }
        movementTakeover = null;
        hamper--;
    }

    void Jump()
    {
        if (!charCon.isGrounded) { return; }
        yMove = jumpForce;
        falling = true;
    }

    void OnControllerColliderHit(ControllerColliderHit coll)
    {
        string tag = coll.collider.tag;
        if (tag.Contains("Book")) {
            SpellBook touchedBook = coll.collider.GetComponent<SpellBook>();
            if (touchedBook) {
                Debug.Log("I touched book!");
            }
        }
        if (tag.Contains("Ground") || tag.Contains("Roof")) {
            if (coll.point.y > transform.position.y && yMove > 0) // If collided with head
            {
                Debug.Log("I hit my head!");
                yMove = 0f;
                return;
            }
            Vector3 feet = transform.position + Vector3.down * charCon.bounds.extents.y;
            if (Vector3.Distance(coll.point, feet) < 0.2f && !charCon.isGrounded) // If collided with feet
            {
                falling = false;
                yMove = Physics.gravity.y;
                return;
            }
        }
        if(tag.Contains("Wall") || tag.Contains("Furniture")) {
            if(movementTakeover != null) {
                StopCoroutine(movementTakeover);
                movementTakeover = null;
                hamper--;
            }
            if(coll.collider.attachedRigidbody != null) {
                Vector3 velocity = coll.collider.transform.position - transform.position;
                coll.collider.attachedRigidbody.AddForce(velocity.normalized * currSpeed * 6f);
            }
        }
    }
}
