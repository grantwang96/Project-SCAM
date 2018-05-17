using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

    public Transform Body;
    
    public bool drunk;
    public bool cameraShaking;

    public static float mouseSensitivity = 150;
	public static float ctrSensitivity = 150;
    float upDownLook = 0f;
    public float shakeForce;
    public float maxShakeForce;
    public float shakeReduceFactor;
    public Vector3 originPosition;

    public Transform affectedYTurn;
    public int normalMove = 1;
    public static CameraMovement Instance;
    public PlayerDamageable playerDamageable;

    public bool separateControl
    {
        get { return separateControl; }
        set
        {
            if (value) { affectedYTurn = Body; }
            else { affectedYTurn = transform; }
        }
    }

    // Use this for initialization
    void Start()
    {
        Instance = this;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        separateControl = true;
        affectedYTurn = Body;
        originPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.U)) { StartCoroutine(shakeCamera(0.3f)); }

        if(playerDamageable.dead) { return; }

        // 1. get mouse input data
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * mouseSensitivity * normalMove; // horizontal mousespeed
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * mouseSensitivity * normalMove; // vertical mousespeed

		//alt ps4 controls
		mouseX += Input.GetAxis("ctr_lookX") * Time.deltaTime * ctrSensitivity * normalMove;
		mouseY += Input.GetAxis("ctr_lookY") * Time.deltaTime * ctrSensitivity * normalMove;

        upDownLook -= mouseY; // minus-equals un-inverts the mouse-look-y
        upDownLook = Mathf.Clamp(upDownLook, -60f, 70f); // constrain look 80 degrees up or down

        // Body.Rotate(0f, mouseX, 0f);
        
        transform.localEulerAngles = new Vector3(upDownLook, transform.localEulerAngles.y, 0f);
        Vector3 bodyTurn = affectedYTurn.localEulerAngles + new Vector3(0, mouseX, 0);
        affectedYTurn.localEulerAngles = bodyTurn;
        
        if (drunk)
        {
            /*
            upDownLook += Mathf.PerlinNoise(Random.value * -5f, 0f) * 0.5f;
            float turnMod = (Mathf.PerlinNoise(Random.value * -5f, Random.value * 5f) * 4f) - 2f;
            upDownLook = Mathf.Clamp(upDownLook, -80f, 80f); // constrain look 80 degrees up or down
            transform.localEulerAngles = new Vector3(upDownLook, transform.localEulerAngles.y, 0f);
            affectedYTurn.localEulerAngles = affectedYTurn.localEulerAngles + new Vector3(0, turnMod, 0);
            */
        }
    }

    public IEnumerator shakeCamera(float newShakeForce)
    {
        float startTime = Time.time;
        shakeForce += newShakeForce;
        if(shakeForce > maxShakeForce) { shakeForce = maxShakeForce; }
        while(shakeForce > 0)
        {
            transform.localPosition = originPosition + Random.insideUnitSphere * shakeForce;
            shakeForce -= Time.deltaTime * shakeReduceFactor;
            yield return new WaitForEndOfFrame();
        }
        shakeForce = 0f;
        transform.localPosition = originPosition;
    }
}
