using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public class PlayerDamageable : Damageable {

    public static PlayerDamageable Instance;

    Coroutine seduced;
    Coroutine hurtVisuals;

    public Transform playerCanvas;
    public Transform playerCanvasPrefab;
    public CameraMovement HeadMove;
    public Transform mainCamerPivot;
    public GameObject DrunkHead;

    public Sprite transmutedIcon;

    public Image statusEffectPrefab;
    public Transform statusEffectBar;

    // public Image healthBar;
    public MeshRenderer healthBar;
	public MeshRenderer encasing;
    public Image fadeToBlackImage;
    public Image ouchImage;
    Coroutine deathSequence;

	AudioPlayer sounds;

	// Use this for initialization
	public override void Start () {
        base.Start();
        Instance = this;
		sounds = GetComponent<AudioPlayer>();

        myMovement.hamper = 1;
        StartCoroutine(fadeInScene());
	}
	
	// Update is called once per frame
	public override void Update () {
        float fillAmount = (float)health / max_health;
        if(fillAmount > 1f) { fillAmount = 1f; }
        else if(fillAmount < 0) { fillAmount = 0f; }

        healthBar.transform.localScale = new Vector3(.7f, fillAmount, .7f);
		// Color c = Color.Lerp(Color.green, Color.red, 1f - (float)health / max_health);
		// healthBar.material.color = c;
		// encasing.material.SetColor("_RimColor", c);
    }

    public override void TakeDamage(Transform attacker, int hpLost, Vector3 dir, float force)
    {
        if (hurt || dead) { Debug.Log("Omae wa mo...shindeiru!"); return; }
        
        base.TakeDamage(attacker, hpLost, dir, force);
        if(health <= 0) { Die(); return; }
        if(attacker == null) { return; }
        PlayerMagic.instance.invokeChangeFollowers(attacker.GetComponent<Damageable>());

		sounds.PlayClip("hurt");
        if(hurtVisuals != null) { StopCoroutine(hurtVisuals); }
        hurtVisuals = StartCoroutine(visualizeHurt(hpLost));
        StartCoroutine(hurtFrames(hpLost));
    }

    IEnumerator hurtFrames(int hpLost) {
        hurt = true;
        float time = 0f;

        Camera.main.transform.localEulerAngles = Vector3.zero;
        float xRot = 20f * hpLost / max_health;
        float yRot = Random.Range(-5, 5);
        float zRot = Random.Range(-5, 5);
        Vector3 startRot = new Vector3(xRot, yRot, zRot);
        Camera.main.transform.localEulerAngles = startRot;


        while(time < hurtTime) {
            Camera.main.transform.localEulerAngles = Vector3.Lerp(startRot, Vector3.zero, time / hurtTime);
            yield return new WaitForEndOfFrame();
            time += Time.deltaTime;
        }
        Camera.main.transform.localEulerAngles = Vector3.zero;
        hurt = false;
    }

    IEnumerator visualizeHurt(int hpLost)
    {
        // add opacity to hurt image
        float hurtVal = ouchImage.color.a + (float)hpLost / max_health;
        Color hurtColor = new Color(ouchImage.color.r, ouchImage.color.g, ouchImage.color.b, hurtVal);
        ouchImage.color = hurtColor;

        if(ouchImage.color.a > 0.8f) { ouchImage.color = new Color(ouchImage.color.r, ouchImage.color.g, ouchImage.color.b, 0.8f); }

        while(ouchImage.color.a > 0f) {
            ouchImage.color =
                new Color(ouchImage.color.r, ouchImage.color.g, ouchImage.color.b, ouchImage.color.a - (Time.deltaTime * .25f));
            yield return new WaitForEndOfFrame();
        }
        hurtVisuals = null;
    }

    public override void Die()
    {
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        // CheckpointManager.Instance.ResetToLastCheckpoint();

        // Play some death sfx
        if(deathSequence == null) { deathSequence = StartCoroutine(DeathSequence()); }
    }

    IEnumerator DeathSequence() {
        dead = true;
        CharacterController charCon = GetComponent<CharacterController>();
        while(!charCon.isGrounded) { yield return new WaitForFixedUpdate(); }

        Animator anim = Camera.main.GetComponent<Animator>();
        anim.Play("Death");
        Debug.Log("Waiting for death animation change over...");
        while(!anim.GetCurrentAnimatorStateInfo(0).IsName("Death")) { yield return new WaitForFixedUpdate(); }
        AnimationClip clip = anim.GetCurrentAnimatorClipInfo(0)[0].clip;
        Debug.Log("Death animation started!");
        yield return new WaitForSeconds(clip.length);
        Debug.Log("Death animation finished!");

        float time = 0f;
        fadeToBlackImage.color = Color.clear;
        while(time < 1f) {
            time += Time.deltaTime * .33f;
            fadeToBlackImage.color = Color.Lerp(Color.clear, Color.black, time);
            yield return new WaitForEndOfFrame();
        }

        Camera.main.transform.localPosition = Vector3.zero;
        Camera.main.transform.localRotation = Quaternion.identity;
        anim.Play("Default");
        CheckpointManager.Instance.ResetToLastCheckpoint();
        fadeToBlackImage.color = Color.clear;
        dead = false;
        deathSequence = null;
    }

    /*
    public override void Fly(float force, float duration)
    {
        if(flight != null) {
            StopCoroutine(flight);
            myMovement.hamper--;
        }
        flight = StartCoroutine(processFlying(force, duration));
    }

    IEnumerator processFlying(float force, float duration)
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
    }

    public override void Drunk(float duration)
    {
        if(drunkness != null) { StopCoroutine(drunkness); }
        drunkness = StartCoroutine(processDrunk(duration));
    }

    IEnumerator processDrunk(float duration)
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
    }
    */

    public override void InitiateTransmutation(float duration, GameObject replacement)
    {
        if (!transmutable) { return; }
        StartCoroutine(processTransmutation(duration, replacement));
    }

    public override IEnumerator processTransmutation(float duration, GameObject replacement)
    {
        transmutable = false; // set transmutable to false
        GameObject newBody = Instantiate(replacement, transform); // create new body

        // prime new body to replace player
        newBody.layer = gameObject.layer;
        newBody.transform.position = transform.position;
        newBody.transform.rotation = transform.rotation;
        Rigidbody newrbody = newBody.GetComponent<Rigidbody>();
        Damageable newDam = newBody.GetComponent<Damageable>();
        Collider newBodyColl = newBody.GetComponent<Collider>();
        
        //shut off unnecessary components of newbody
        newrbody.isKinematic = true;
        newrbody.useGravity = false;
        newDam.parentHit = this;
        newDam.transmutable = false;

        // shift gun(to accomodate for larger bodies)
        Vector3 localOrigin = Camera.main.transform.localPosition;
        Transform gun = transform.Find("Gun");
        Vector3 gunOrigin = gun.localPosition;
        gun.localPosition += Vector3.forward * .5f;

        CharacterController charCon = GetComponent<CharacterController>();

        // change player's collider to become similar to newbody's
        float newHeight = newBodyColl.bounds.extents.y * 2;
        float newRadius = (newBodyColl.bounds.extents.x + newBodyColl.bounds.extents.z) / 2;
        newBodyColl.enabled = false;
        float originHeight = charCon.height; // save for later
        float originRadius = charCon.radius; // save for later
        charCon.height = newHeight;
        charCon.radius = newRadius;
        charCon.detectCollisions = true;

        myMovement.Head.position = transform.position + Vector3.up * charCon.height / 2;
        myMovement.Head.forward = transform.forward;
        // charCon.enabled = false;
        mainCamerPivot.position -= transform.forward * 3f;
        mainCamerPivot.transform.LookAt(myMovement.Head);

        gun.parent = myMovement.Head;
        Vector3 localPos = myMovement.Head.localPosition;
        // myMovement.Head.parent = newrbody.transform;
        myMovement.Head.position = newrbody.transform.TransformPoint(localPos);
        myMovement.Head.forward = newrbody.transform.forward;

        // myPlayMagic.enabled = false; // ALLOW FOR SPELL COMBAT
        // myMovement.hamper += 1;

        Transform oldTransmuteStatus = statusEffectBar.Find("transmutedStatus");
        if(oldTransmuteStatus) { Destroy(oldTransmuteStatus.gameObject); }
        Image newTransmuteStatus = Instantiate(statusEffectPrefab, statusEffectBar);
        newTransmuteStatus.rectTransform.localRotation = Quaternion.Euler(new Vector3(0, 0, 180));
        newTransmuteStatus.name = "transmutedStatus";

        float time = 0f;
        while(time < duration) {
            newTransmuteStatus.fillAmount = 1f - (time / duration);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        // yield return new WaitForSeconds(duration);

        transform.position = newrbody.position;
        Destroy(newTransmuteStatus.gameObject);
        // reset head components
 
        charCon.enabled = true;
        charCon.detectCollisions = true;
        charCon.height = originHeight;
        charCon.radius = originRadius;

        // myMovement.Head.parent = transform;
        myMovement.Head.localPosition = Vector3.up * charCon.height / 2;
        myMovement.Head.forward = transform.forward;
        gun.parent = transform;
        gun.localPosition = gunOrigin;
        gun.forward = transform.forward;
        mainCamerPivot.transform.localPosition = localOrigin;
        mainCamerPivot.transform.rotation = myMovement.Head.rotation;

        // re-enable spell combat and transmutable
        // myMovement.hamper--;
        setTransmutable(true);

        // get rid of newbody
        Destroy(newBody);
    }

//    public override void Seduce(float duration, GameObject target, SpellCaster owner)
//    {
//        
//    }

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

    IEnumerator fadeInScene()
    {
        float time = 0f;
        while(time < 1f) {
            time += Time.deltaTime / 3f;
            fadeToBlackImage.color = Color.Lerp(Color.black, Color.clear, time);
            yield return new WaitForEndOfFrame();
        }
        myMovement.hamper = 0;
    }
}
