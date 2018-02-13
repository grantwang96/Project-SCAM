using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerDamageable : Damageable {

    public static PlayerDamageable Instance;

    Coroutine flight;
    Coroutine drunkness;
    Coroutine slowness;
    Coroutine seduced;

    public Transform playerCanvas;
    public Transform playerCanvasPrefab;
    public CameraMovement HeadMove;
    public GameObject DrunkHead;

    public Sprite drunkIcon;
    public Sprite slowIcon;
    public Sprite floatIcon;
    public Image statusEffectPrefab;
    public Transform statusEffectBar;

    public Image healthBar;

	// Use this for initialization
	public override void Start () {
        base.Start();
        Instance = this;
        // playerCanvas = Instantiate(playerCanvasPrefab);
        // healthBar = playerCanvas.Find("HealthBar").GetComponent<Image>();
	}
	
	// Update is called once per frame
	public override void Update () {
        // update healthbar
        healthBar.fillAmount = (float)health / max_health;
	}

    public override void TakeDamage(Transform attacker, int hpLost, Vector3 dir, float force)
    {
        if (hurt) { return; }
        // Visual hurt effects
        base.TakeDamage(attacker, hpLost, dir, force);
        if(health <= 0) { Die(); return; }
        PlayerMagic.instance.invokeChangeFollowers(attacker.GetComponent<Damageable>());
        Debug.Log("Player HP: " + health);
        StartCoroutine(hurtFrames());
    }

    IEnumerator hurtFrames()
    {
        hurt = true;
        yield return new WaitForSeconds(hurtTime);
        hurt = false;
    }

    public override void Die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /*public override void Fly(float force, float duration)
    {
        if(flight != null) {
            StopCoroutine(flight);
            myMovement.hamper--;
        }
        flight = StartCoroutine(processFlying(force, duration));
    }*/

    /*IEnumerator processFlying(float force, float duration)
    {
        Debug.Log("Duration is: " + duration);
        float startTime = Time.time;
        Movement myOwnerMove = GetComponent<Movement>();
        myOwnerMove.hamper++;
        HeadMove.separateControl = false;
        Transform statEffectObj = statusEffectBar.transform.Find("Flight");
        Image newStatusEffect;
        if (statEffectObj == null) {
            newStatusEffect = Instantiate(statusEffectPrefab);
            newStatusEffect.transform.name = "Flight";
            newStatusEffect.sprite = floatIcon;
            newStatusEffect.transform.SetParent(statusEffectBar.transform, false);
        }
        else {
            newStatusEffect = statEffectObj.GetComponent<Image>();
        }
        // UI handling
        rbody.constraints = RigidbodyConstraints.None;
        if (rbody.useGravity)
        {
            rbody.useGravity = false;
            rbody.AddForce(Vector3.up * force * 0.5f, ForceMode.Impulse);
        }
        rbody.angularDrag = 1f;
        while (Time.time - startTime < duration)
        {
            float vertical = Input.GetAxis("Vertical"); // Get player inputs
            float horizontal = Input.GetAxis("Horizontal"); // Get player inputs
            rbody.AddForce(myOwnerMove.Head.transform.forward * vertical * myOwnerMove.currSpeed / 2 + (myOwnerMove.Head.transform.right * horizontal * myOwnerMove.currSpeed / 2));
            if (rbody.velocity.magnitude > myOwnerMove.currSpeed / 2) { rbody.velocity = rbody.velocity.normalized * myOwnerMove.currSpeed / 2; }
            newStatusEffect.fillAmount = 1f - (Time.time - startTime) / duration;
            yield return new WaitForEndOfFrame();
        }
        rbody.useGravity = true;
        rbody.angularDrag = 0.05f;
        transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
        rbody.angularVelocity = Vector3.zero;
        rbody.constraints = RigidbodyConstraints.FreezeRotation;
        HeadMove.separateControl = true;
        HeadMove.transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
        transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
        Destroy(newStatusEffect.gameObject);
        flight = null;
        myOwnerMove.hamper--;
    }*/

    /*public override void Drunk(float duration)
    {
        if(drunkness != null) { StopCoroutine(drunkness); }
        drunkness = StartCoroutine(processDrunk(duration));
    }*/

    /*IEnumerator processDrunk(float duration)
    {
        PlayerMovementV2 myMove = GetComponent<PlayerMovementV2>();
        float startTime = Time.time;
        DrunkHead.SetActive(true);
        HeadMove.drunk = true;
        Transform statEffectObject = statusEffectBar.transform.Find("drunk");
        Image newStatusEffect;
        if (statEffectObject == null)
        {
            newStatusEffect = Instantiate(statusEffectPrefab);
            newStatusEffect.transform.name = "drunk";
            newStatusEffect.sprite = drunkIcon;
            newStatusEffect.transform.SetParent(statusEffectBar.transform, false);
        }
        else
        {
            newStatusEffect = statEffectObject.GetComponent<Image>();
        }
        HeadMove.normalMove = -1;
        myMove.drunkMod = -1;

        while (Time.time - startTime < duration)
        {
            newStatusEffect.fillAmount = 1f - (Time.time - startTime) / duration;
            yield return new WaitForEndOfFrame();
        }
        HeadMove.normalMove = 1;
        myMove.drunkMod = 1;
        DrunkHead.SetActive(false);
        HeadMove.drunk = false;
        Destroy(newStatusEffect.gameObject);
        drunkness = null;
    }*/

    public override void InitiateTransmutation(float duration, GameObject replacement)
    {
        if (!transmutable) { return; }
        StartCoroutine(processTransmutation(duration, replacement));
    }

    public override IEnumerator processTransmutation(float duration, GameObject replacement)
    {
        transmutable = false; // set transmutable to false
        PlayerMagic myPlayMagic = GetComponent<PlayerMagic>(); // get magical abilities
        GameObject newBody = Instantiate(replacement, transform); // create new body

        // prime new body to replace player
        newBody.layer = gameObject.layer;
        newBody.transform.position = transform.position;
        newBody.transform.rotation = transform.rotation;
        Rigidbody newrbody = newBody.GetComponent<Rigidbody>();
        Damageable newDam = newBody.GetComponent<Damageable>();
        Collider newBodyColl = newBody.GetComponent<Collider>();
        
        //shut off unnecessary components of newbody
        // newrbody.isKinematic = true;
        // newrbody.useGravity = false;
        newDam.parentHit = this;
        newDam.transmutable = false;

        // shift gun(to accomodate for larger bodies)
        Vector3 localOrigin = Camera.main.transform.localPosition;
        Transform gun = transform.Find("Gun");
        Vector3 gunOrigin = gun.localPosition;
        gun.localPosition += Vector3.forward * .5f;

        CharacterController charCon = GetComponent<CharacterController>();
        /*
        // change player's collider to become similar to newbody's
        float newHeight = newBodyColl.bounds.extents.y * 2;
        float newRadius = (newBodyColl.bounds.extents.x + newBodyColl.bounds.extents.z) / 2;
        newBodyColl.enabled = false;
        float originHeight = charCon.height; // save for later
        float originRadius = charCon.radius; // save for later
        charCon.height = newHeight;
        charCon.radius = newRadius;
        // charCon.detectCollisions = true;
        */
        myMovement.Head.position = transform.position + Vector3.up * charCon.height / 2;
        myMovement.Head.forward = transform.forward;
        charCon.enabled = false;
        Camera.main.transform.position -= transform.forward * 3f;
        Camera.main.transform.LookAt(myMovement.Head);

        gun.parent = newrbody.transform;
        myMovement.Head.parent = newrbody.transform;

        // myPlayMagic.enabled = false; // ALLOW FOR SPELL COMBAT
        myMovement.hamper += 1;

        // StatusBarHandler.instance.applyStatus("transmute", duration);
        float time = 0f;
        while(time < duration) {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            newrbody.AddForce((newrbody.transform.forward * vertical + newrbody.transform.right * horizontal)
                * myMovement.maxSpeed);
            if(newrbody.velocity.magnitude > myMovement.maxSpeed) { newrbody.velocity = new Vector3(horizontal, 0f, vertical) * myMovement.maxSpeed; }

            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        // yield return new WaitForSeconds(duration);

        transform.position = newrbody.position;
        // reset head components
 
        charCon.enabled = true;
        charCon.detectCollisions = true;
        // charCon.height = originHeight;
        // charCon.radius = originRadius;
        myMovement.Head.position = transform.position + Vector3.up * charCon.height / 2;
        myMovement.Head.parent = transform;
        myMovement.Head.forward = transform.forward;
        gun.parent = transform;
        gun.localPosition = gunOrigin;
        gun.forward = transform.forward;
        Camera.main.transform.localPosition = localOrigin;
        Camera.main.transform.rotation = myMovement.Head.rotation;

        // re-enable spell combat and transmutable
        myPlayMagic.enabled = true;
        myMovement.hamper--;
        setTransmutable(true);

        // get rid of newbody
        Destroy(newBody);
    }

    public override void Seduce(float duration, GameObject target, Transform owner)
    {
        
    }

    public override void knockBack(Vector3 dir, float force)
    {
        if(parentHit != null) { parentHit.knockBack(dir, force); }
        myMovement.knockBack(dir, force);
    }

    public override void vortexGrab(Transform center, float force)
    {
        float dist = Vector3.Distance(center.position, transform.position);
        Vector3 dir = (center.position - transform.position).normalized;
        myMovement.knockBack(dir, force);
    }
}
