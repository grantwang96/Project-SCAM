using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PlayerMagic : MonoBehaviour, SpellCaster {

    public static PlayerMagic instance;
    public Damageable myDamageable;
    [SerializeField] List<SpellBook> spellsInventory = new List<SpellBook>();
    public int maxSpells;
    int currentHeld;

    #region Able To Fire
    [SerializeField] bool canFireSpells;
    public bool canShoot() { return canFireSpells; }
    public void setCanShoot(bool can) { canFireSpells = can; }
    bool canFire;
    #endregion

    public float spellPickUpSpeed;

    #region Body Parts
    public Transform body;
    public Transform Head;
    [SerializeField] Transform gun;
    #endregion

    public delegate void seductionHit(Damageable target, SpellCaster owner);
    public event seductionHit changeFollowerTarget;

    public LayerMask interactLayers;
    public float grabRange;
    [SerializeField] Interactable currentInteractable;

    public TextMesh currentSpellTitle;
    public TextMesh currentSpellDescription;
    public SpriteRenderer currentSpellSideEffectImage;
    public TextMesh ammoCount;
    public MeshRenderer currentSpellCover;

    Coroutine enemyDisplayRoutine;

    #region UIStuff

    [SerializeField] Transform spellSlots;

    [SerializeField] Text SpellTitle;
    [SerializeField] Text SpellDescription;
    [SerializeField] Image ammoGaugeFill;
    [SerializeField] Image ammoGaugeBackground;
    [SerializeField] Image reticule;
    [SerializeField] GameObject enemyDataSection;
    [SerializeField] Image enemyHealthGauge;
    [SerializeField] Image enemyHealthMask;
    [SerializeField] Image enemyHealthGaugeBackground;
    [SerializeField] Text enemyName;

    [SerializeField] Sprite reticuleNormal;
    [SerializeField] Sprite reticuleInteractable;
    [SerializeField] Sprite reticuleRecharging;

    public Transform bookUI;
    public MeshRenderer leftBook;
    public MeshRenderer rightBook;
    public Vector3 bookNormalPosition;
    public Vector3 startSwitchPosition;
    public Vector3 bookNormalRotation;
    public Vector3 startSwitchRotation;
    Coroutine bookSwapRoutine;
    public float swapSpeed;
    #endregion

	#region Audio

	AudioPlayer sounds;

	#endregion

    // Use this for initialization
    void Start () {
        instance = this;
        currentHeld = 0;
        canFire = true;
        canFireSpells = true;

        enemyHealthGauge.enabled = false;
        enemyHealthMask.enabled = false;
        enemyHealthGaugeBackground.enabled = false;
        enemyName.enabled = false;

		sounds = GetComponentInParent<AudioPlayer>();

        // updateCurrentHeld();
        leftBook.enabled = false;
        rightBook.enabled = false;
        UpdateSpellData();
        Debug.Log("Setup complete!");
    }
	
	// Update is called once per frame
	void Update () {
        if (GameManager.Instance.menuMode || myDamageable.dead) { return; }

        foreach(SpellBook book in spellsInventory) {
            if(book.sparklyEffect != null && book.sparklyEffect.gameObject.activeInHierarchy) {
                book.sparklyEffect.gameObject.SetActive(false);
            }
        }

        processScrolling(); // if the player scrolls
        processNumKeys(); // if the player hits the keys

        if(currentInteractable == null && canFire) { reticule.sprite = reticuleNormal;  }
        else if(canFire) { reticule.sprite = reticuleInteractable; }
        
        if (Input.GetButtonDown("Fire1")) { // make sure player hits shoot button and has something to shoot
            fireSpell();
        }
        if(Input.GetButtonDown("Fire2") && currentInteractable != null) {
            currentInteractable.Interact(this); currentInteractable = null;
        }
        if (spellsInventory.Count > 0 && ammoGaugeBackground.gameObject.activeInHierarchy) { // update the ammo gauge
            ammoGaugeFill.fillAmount = (float)spellsInventory[currentHeld].getAmmo() / spellsInventory[currentHeld].getMaxAmmo();
        }
        processLooking();
    }

	#region for checkpointing

	public List<SpellBook> GetSpellsInventory() {
		return spellsInventory;
	}

	public void ResetSpellsToSerialized(List<string> jsons) {
		for (int i = 0; i < jsons.Count; i ++) {
			JsonUtility.FromJsonOverwrite(jsons[i], spellsInventory[i]);
		}
	}

	public void UpdateUI() {
		if (currentHeld >= spellsInventory.Count) { currentHeld = 0; }
		else if (currentHeld < 0) { currentHeld = spellsInventory.Count - 1; }

		if (spellsInventory.Count == 0) {
			return;
		}

		currentSpellTitle.text = spellsInventory[currentHeld].primaryEffect.title;
        // currentSpellDescription.text = spellsInventory[currentHeld].secondaryEffect.title;
        currentSpellSideEffectImage.sprite = spellsInventory[currentHeld].secondaryEffect.descriptiveImage;
		ammoCount.text = "Charges: " + spellsInventory[currentHeld].getAmmo();
		currentSpellCover.material.color = spellsInventory[currentHeld].baseColor;
	}

    IEnumerator switchBooks() {
        Vector3 startPos = bookUI.localPosition;
        Vector3 startRot = bookUI.localRotation.eulerAngles;
        float time = 0f;

        while(time < 1f) {
            time += Time.deltaTime * swapSpeed;
            bookUI.localPosition = Vector3.Lerp(startPos, startSwitchPosition, time);
            bookUI.localRotation = Quaternion.Lerp(Quaternion.Euler(startRot), Quaternion.Euler(startSwitchRotation), time);
            yield return new WaitForEndOfFrame();
        }

        UpdateUI();
        UpdateBookCovers();

        time = 0f;

        while(time < 1f) {
            time += Time.deltaTime * swapSpeed;
            bookUI.localPosition = Vector3.Lerp(startPos, bookNormalPosition, time);
            bookUI.localRotation = Quaternion.Lerp(Quaternion.Euler(startSwitchRotation), Quaternion.Euler(bookNormalRotation), time);
            yield return new WaitForEndOfFrame();
        }

        bookSwapRoutine = null;
    }

    void UpdateBookCovers() {
        if (spellsInventory.Count == maxSpells) {
            leftBook.enabled = true;
            rightBook.enabled = true;
            if(currentHeld == 0) {
                leftBook.materials[1].color = spellsInventory[2].baseColor;
                rightBook.materials[1].color = spellsInventory[1].baseColor;
            }
            else if(currentHeld == 1) {
                leftBook.materials[1].color = spellsInventory[0].baseColor;
                rightBook.materials[1].color = spellsInventory[2].baseColor;
            }
            else {
                leftBook.materials[1].color = spellsInventory[1].baseColor;
                rightBook.materials[1].color = spellsInventory[0].baseColor;
            }
        }
        else if(spellsInventory.Count == 2) {
            leftBook.enabled = true;
            rightBook.enabled = false;
            if(currentHeld == 0) { leftBook.materials[1].color = spellsInventory[1].baseColor; }
            else { leftBook.materials[1].color = spellsInventory[0].baseColor; }
        } else {
            leftBook.enabled = false;
            rightBook.enabled = false;
        }
    }

	#endregion

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

    #region process Inputs
    void processNumKeys()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            currentHeld = 0;
            // updateCurrentHeld();
            UpdateSpellData();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && spellsInventory.Count > 1) {
            currentHeld = 1;
            // updateCurrentHeld();
            UpdateSpellData();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && spellsInventory.Count > 2) {
            currentHeld = 2;
            // updateCurrentHeld();
            UpdateSpellData();
        }
    }

    void processScrolling()
    {
        float mouse = Input.GetAxis("Mouse ScrollWheel"); // record input from mouse scrollwheel
        if(mouse != 0)
        {
            if (mouse > 0) { currentHeld--; }
            else if (mouse < 0) { currentHeld++; }
            // updateCurrentHeld();
            UpdateSpellData();

			//play audio
			sounds.PlayClip("swap");
        }
    }
    #endregion

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

    void UpdateSpellData() {
        if (spellsInventory.Count == 0) { // shut everything off
            currentHeld = 0;
            currentSpellTitle.text = "";
            // currentSpellDescription.text = "";
            currentSpellSideEffectImage.sprite = null;
            ammoCount.text = "";
            currentSpellCover.material.color = new Color(.3f, .3f, .3f);
        }
        else { // update the ammo gauge and makesure current held is within inventory count
            if (currentHeld >= spellsInventory.Count) { currentHeld = 0; }
            else if (currentHeld < 0) { currentHeld = spellsInventory.Count - 1; }

            if(bookSwapRoutine != null) { StopCoroutine(bookSwapRoutine); }
            bookSwapRoutine = StartCoroutine(switchBooks());
			// UpdateUI();
        }
    }

    public void displayEnemyData(Damageable enemy)
    {
        if(enemy == body.GetComponent<Damageable>()) { return; }
        if(enemyDisplayRoutine != null) { StopCoroutine(enemyDisplayRoutine); }
        enemyDisplayRoutine = StartCoroutine(displayCurrentEnemy(enemy));
    }

    IEnumerator displayCurrentEnemy(Damageable enemy)
    {
        float time = 0f;
        enemyName.enabled = true;
        enemyHealthGaugeBackground.enabled = true;
        enemyHealthMask.enabled = true;
        enemyHealthGauge.enabled = true;
        enemyName.text = enemy.gameObject.name;

        RectTransform enemyHPBackground = enemyHealthGaugeBackground.GetComponent<RectTransform>();
        RectTransform enemyHP = enemyHealthGauge.GetComponent<RectTransform>();
        RectTransform enemyHPMask = enemyHealthMask.GetComponent<RectTransform>();
        float sizeModifier = 1f + (100f / enemy.max_health);
        enemyHPBackground.sizeDelta = new Vector2((enemy.max_health * sizeModifier), 15);
        enemyHPMask.sizeDelta = enemyHPBackground.sizeDelta;
        enemyHP.sizeDelta = new Vector2(enemyHPBackground.sizeDelta.x, 30);
        enemyHP.anchoredPosition = enemyHPBackground.anchoredPosition;

        while(time < 5f) {
            if(enemy == null || enemy.gameObject == null) { break; }

            enemyHealthGauge.fillAmount = (float)enemy.health / enemy.max_health;

            time += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        enemyName.enabled = false;
        enemyHealthGaugeBackground.enabled = false;
        enemyHealthMask.enabled = false;
        enemyHealthGauge.enabled = false;
        enemyDisplayRoutine = null;
    }

    #region Triggers
    void OnTriggerStay(Collider coll) {
        Interactable inter = coll.GetComponent<Interactable>();
        if(inter != null && currentInteractable == null) { currentInteractable = inter; }
    }

    void OnTriggerExit(Collider coll) {
        Interactable inter = coll.GetComponent<Interactable>();
        if(inter == currentInteractable) { currentInteractable = null; }
    }
    #endregion

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
        if(target.myMovement != null && target.health > 0 & target.max_health > 0) {
            displayEnemyData(target);
        }

        if(changeFollowerTarget != null) {
            changeFollowerTarget(target, this);
        }
    }

    public void fireSpell() // Shoot the spell
    {
        if (!canFire) { return; } // If cooling down
        if(spellsInventory.Count == 0) { return; }
        spellsInventory[currentHeld].primaryEffect.ActivateSpell(this, spellsInventory[currentHeld].secondaryEffect, Head.forward, spellsInventory[currentHeld].OffChance); // activate currently held spellbook
        spellsInventory[currentHeld].useAmmo(); // the player uses ammo in a spellbook

        /*
        spellslot data = spellSlots.GetChild(currentHeld).GetComponent<spellslot>();
        data.modifyDetails(spellsInventory[currentHeld].spellTitle, spellsInventory[currentHeld].spellDescription,
            spellsInventory[currentHeld].getAmmo(), spellsInventory[currentHeld].getMaxAmmo(), spellsInventory[currentHeld].baseColor);
        // data.ammoBarInner.fillAmount = (float)spellsInventory[currentHeld].getAmmo() / spellsInventory[currentHeld].getMaxAmmo();
        */

        UpdateSpellData();

        // Calculate and initiate cooldown
        float coolDown = spellsInventory[currentHeld].primaryEffect.coolDown;
        coolDown += spellsInventory[currentHeld].secondaryEffect.coolDown;
        StartCoroutine(fireCoolDown(coolDown));

		//sound
		sounds.PlayClip("fire");
    }

    public Transform returnGun() { return gun; }

    public Transform returnBody() { return body; }

    public Transform returnHead() { return Head; }

    public Transform returnTransform() { return transform; }

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
            UpdateSpellData();
        }
        // updateCurrentHeld();
        newSpell.Deactivate();
        newSpell.transform.parent = transform.parent;
        newSpell.transform.localPosition = Vector3.zero;
        newSpell.transform.localRotation = Quaternion.identity;

		//audio
		sounds.PlayClip("pickup");
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
        // Debug.Log("Dropped Spell");
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

		sounds.PlayClip("out_of_spell");
        StartCoroutine(dropSpellProcess(dropSpell, originPos));
    }

    IEnumerator dropSpellProcess(SpellBook dropSpell, Vector3 originPos) // visualize dropping the book
    {
        // dropSpell.dead = true;
        FloatyRotaty fr = dropSpell.GetComponent<FloatyRotaty>();
        fr.active = false;
        UpdateSpellData();
        dropSpell.Activate(); // turn on the book
        dropSpell.transform.parent = null;

        float time = 0f;

        Vector3 startPos = dropSpell.transform.position;
        while (!dropSpell.dead && Vector3.Distance(dropSpell.transform.position, originPos) > 0.2f) {
            time += Time.deltaTime;
            dropSpell.transform.position = Vector3.Lerp(startPos, originPos, time / spellPickUpSpeed); // shift book to position
            yield return new WaitForEndOfFrame();
        }
        if (fr != null) { fr.enabled = true; }
        fr.SetPosition();
        fr.active = true;
        // updateCurrentHeld();
    }

    public SpellBook returnSpell()
    {
        if(spellsInventory.Count == 0) { return null; }
        return spellsInventory[currentHeld];
    }

    public IEnumerator fireCoolDown(float duration) // process cool down
    {
        canFire = false;
        reticule.sprite = reticuleRecharging;
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
