using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMagic : MonoBehaviour, SpellCaster {

    public static PlayerMagic instance;
    List<SpellBook> spellsInventory = new List<SpellBook>();
    public int maxSpells;
    int currentHeld;

    [SerializeField] bool canFireSpells;
    public bool canShoot() { return canFireSpells; }
    public void setCanShoot(bool can) { canFireSpells = can; }

    public float spellPickUpSpeed;

    public Transform body;
    public Transform Head;
    [SerializeField] Transform gun;

    public delegate void seductionHit(Damageable target, SpellCaster owner);
    public event seductionHit changeFollowerTarget;

    bool canFire;
    public LayerMask interactLayers;
    public float grabRange;
    [SerializeField] Interactable currentInteractable;
    #region UIStuff

    [SerializeField] Transform spellSlots;

    [SerializeField] Text SpellTitle;
    [SerializeField] Text SpellDescription;
    [SerializeField] Image ammoGaugeFill;
    [SerializeField] Image ammoGaugeBackground;
    [SerializeField] Image reticule;

    [SerializeField] Color reticuleNormal;
    [SerializeField] Color reticuleInteractable;
    #endregion

    // Use this for initialization
    void Start () {
        instance = this;
        currentHeld = 0;
        canFire = true;
        canFireSpells = true;
        updateCurrentHeld();
	}
	
	// Update is called once per frame
	void Update () {
        processScrolling(); // if the player scrolls
        processNumKeys(); // if the player hits the keys

        if(currentInteractable != null) { reticule.color = reticuleInteractable; }
        else { reticule.color = reticuleNormal; }
        
        if (Input.GetButtonDown("Fire1")) { // make sure player hits shoot button and has something to shoot
            // if(interactable != null) { interactable.Interact(this); }
            // else if (spellsInventory.Count != 0) { fireSpell(); }
            if(currentInteractable != null && currentInteractable.Interact(this)) { currentInteractable = null; }
            else if(spellsInventory.Count != 0) { fireSpell(); }
        }
        if (spellsInventory.Count > 0 && ammoGaugeBackground.gameObject.activeInHierarchy) { // update the ammo gauge
            ammoGaugeFill.fillAmount = (float)spellsInventory[currentHeld].getAmmo() / spellsInventory[currentHeld].getMaxAmmo();
        }
        processLooking();
    }

    Interactable processLooking() {
        Ray ray = new Ray(Head.position, Head.forward);
        // RaycastHit hit;
        // Debug.DrawLine(transform.position, transform.position + transform.forward * grabRange, Color.green, 1f);
        RaycastHit[] rayHits = Physics.RaycastAll(ray, grabRange, interactLayers, QueryTriggerInteraction.Collide);
        foreach(RaycastHit hit in rayHits) {
            Interactable interactable = hit.collider.GetComponent<Interactable>();
            if (interactable != null) { return interactable; }
        }
        /*
        if (Physics.Raycast(ray, out hit, grabRange, interactLayers, QueryTriggerInteraction.Collide)) { // if you hit something that is interactable
            Interactable interactable = hit.collider.GetComponent<Interactable>();
            if(interactable != null) { return interactable; }
        }*/
        return null;
    }

    void processNumKeys()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            currentHeld = 0;
            updateCurrentHeld();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && spellsInventory.Count > 1) {
            currentHeld = 1;
            updateCurrentHeld();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && spellsInventory.Count > 2) {
            currentHeld = 2;
            updateCurrentHeld();
        }
    }

    void processScrolling()
    {
        float mouse = Input.GetAxis("Mouse ScrollWheel"); // record input from mouse scrollwheel
        if(mouse != 0)
        {
            if (mouse > 0) { currentHeld--; }
            else if (mouse < 0) { currentHeld++; }
            updateCurrentHeld();
        }
    }

    void updateCurrentHeld() // make sure currentheld is within inventory count
    {
        if(spellsInventory.Count == 0) { // shut everything off
            currentHeld = 0;

            foreach(Transform child in spellSlots) {
                child.Find("Title").GetComponent<Text>().text = "Empty Spell Slot";
                spellslot data = child.GetComponent<spellslot>();
                data.Deselect();
            }
        }
        else { // update the ammo gauge and makesure current held is within inventory count
            if (currentHeld >= spellsInventory.Count) { currentHeld = 0; }
            else if (currentHeld < 0) { currentHeld = spellsInventory.Count - 1; }

            foreach(Transform child in spellSlots) {
                spellslot data = child.GetComponent<spellslot>();
                if (child.GetSiblingIndex() == currentHeld) {
                    SpellBook currSpell = spellsInventory[currentHeld];
                    data.Select(currSpell.spellTitle, currSpell.spellDescription, currSpell.getAmmo(), currSpell.getMaxAmmo(), currSpell.baseColor);
                }
                else {
                    if(child.GetSiblingIndex() >= spellsInventory.Count) { data.setTitle("None Held"); }
                    data.Deselect();
                }
            }
        }
    }

    void OnTriggerStay(Collider coll) {
        Interactable inter = coll.GetComponent<Interactable>();
        if(inter != null && currentInteractable == null) { currentInteractable = inter; }
    }

    void OnTriggerExit(Collider coll) {
        Interactable inter = coll.GetComponent<Interactable>();
        if(inter == currentInteractable) { currentInteractable = null; }
    }

    #region SpellCaster Implementations

    public void addToSeductionList(Damageable loser)
    {
        changeFollowerTarget += loser.setCurrentTarget;
    }

    public void removeFromSeductionList(Damageable loser)
    {
        changeFollowerTarget -= loser.setCurrentTarget;
    }

    public void invokeChangeFollowers(Damageable target)
    {
        if(changeFollowerTarget != null) {
            changeFollowerTarget(target, this);
        }
    }

    public void fireSpell() // Shoot the spell
    {
        if (!canFire) { return; } // If cooling down
        spellsInventory[currentHeld].primaryEffect.ActivateSpell(this, spellsInventory[currentHeld].secondaryEffect, Head.forward); // activate currently held spellbook
        spellsInventory[currentHeld].useAmmo(); // the player uses ammo in a spellbook

        spellslot data = spellSlots.GetChild(currentHeld).GetComponent<spellslot>();
        data.modifyDetails(spellsInventory[currentHeld].spellTitle, spellsInventory[currentHeld].spellDescription,
            spellsInventory[currentHeld].getAmmo(), spellsInventory[currentHeld].getMaxAmmo(), spellsInventory[currentHeld].baseColor);
        // data.ammoBarInner.fillAmount = (float)spellsInventory[currentHeld].getAmmo() / spellsInventory[currentHeld].getMaxAmmo();

        // Calculate and initiate cooldown
        float coolDown = spellsInventory[currentHeld].primaryEffect.coolDown;
        coolDown += spellsInventory[currentHeld].secondaryEffect.coolDown;
        StartCoroutine(fireCoolDown(coolDown));
    }

    public Transform returnGun() { return gun; }

    public Transform returnBody() { return body; }

    public Transform returnHead() { return Head; }

    public void pickUpSpell(SpellBook newSpell)
    {
        // Debug.Log("Grabbed Spell");
        if(spellsInventory.Count == maxSpells) { // if the player's inventory is full
            SpellBook lostSpell = spellsInventory[currentHeld];
            spellsInventory[currentHeld] = newSpell;
            dropSpell(lostSpell, newSpell.transform.position);
        }
        else { // otherwise just add the spell
            spellsInventory.Add(newSpell);
            currentHeld = spellsInventory.Count - 1;
        }
        updateCurrentHeld();
        newSpell.Deactivate();
        newSpell.transform.localPosition = Vector3.zero;
        newSpell.transform.localRotation = Quaternion.identity;
        // StartCoroutine(pickUpProcess(newSpell)); // visualize pick up
    }

    IEnumerator pickUpProcess(SpellBook newSpell) // pick up the spellbook
    {
        float startTime = Time.time;
        newSpell.transform.parent = transform;
        while(Time.time - startTime < spellPickUpSpeed) // pull spellbook closer
        {
            newSpell.transform.position = Vector3.Lerp(newSpell.transform.position, transform.position, (Time.deltaTime / spellPickUpSpeed) * 2);
            yield return new WaitForEndOfFrame();
        }

        // place spellbook at the middle and stop rendering book)
        newSpell.transform.localPosition = Vector3.zero;
        newSpell.Deactivate();
    }

    public void dropSpell(SpellBook dropSpell, Vector3 originPos) // drop the spellbook
    {
        Debug.Log("Dropped Spell");
        // unlink spellbook from player
        if(dropSpell.owner == this.GetComponent<SpellCaster>()) { dropSpell.owner = null; }
        if (spellsInventory.Contains(dropSpell)) { spellsInventory.Remove(dropSpell); }

        // update current held
        // updateCurrentHeld();
        dropSpell.Activate();
        dropSpell.transform.parent = null;

        dropSpell.transform.position = transform.position;
        // dropSpell.Drop(originPos);
        // dropSpell.Die();

        // visualize dropping book
        StartCoroutine(dropSpellProcess(dropSpell, originPos));
    }

    IEnumerator dropSpellProcess(SpellBook dropSpell, Vector3 originPos) // visualize dropping the book
    {
        // dropSpell.dead = true;
        dropSpell.Activate(); // turn on the book
        dropSpell.transform.parent = null;
        Vector3 startPos = dropSpell.transform.position;
        while (!dropSpell.dead && Vector3.Distance(dropSpell.transform.position, originPos) > 0.2f)
        {
            dropSpell.transform.position = Vector3.Lerp(dropSpell.transform.position, originPos, Time.deltaTime / spellPickUpSpeed); // shift book to position
            yield return new WaitForEndOfFrame();
        }
        updateCurrentHeld();
    }

    public SpellBook returnSpell()
    {
        if(spellsInventory.Count == 0) { return null; }
        return spellsInventory[currentHeld];
    }

    public IEnumerator fireCoolDown(float duration) // process cool down
    {
        canFire = false;
        float recovery = 0f;
        while(recovery < duration) {
            reticule.fillAmount = recovery / duration; // update reticule to show progress
            recovery += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        reticule.fillAmount = 1f;
        canFire = true;
    }

    #endregion
}
