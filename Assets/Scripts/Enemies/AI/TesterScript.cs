using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TesterScript : MonoBehaviour {

    public GameObject landMark;

    private NavMeshAgent dumbass;
    public Vector3 target;

    [Range(1f, 100f)] public float searchRange;
    [Range(-0.5f, 0.5f)] public float dotProdThresh;

    // Use this for initialization
    void Start () {
        dumbass = GetComponent<NavMeshAgent>();
        dumbass.updateRotation = false;
	}
	
	// Update is called once per frame
	void Update () {
        /*
        Vector3 dir = landMark.transform.position - transform.position;
        dir.y = 0;
        Quaternion forward = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Lerp(transform.rotation, forward, .8f);
        */

        if (Input.GetMouseButtonDown(0)) {
            TakeCover();
        }
        if (transform.position == dumbass.pathEndPosition) {
            Debug.Log("Reached end!");
        }
    }

    void TakeCover() {
        Debug.Log("Bleep");
        List<NavMeshHit> potentialPositions = new List<NavMeshHit>();
        for(int i = 0; i < 8; i++) {

            Vector3 randPos = getRandomLocation(transform.position, searchRange);
            NavMeshHit hit;
            if(NavMesh.FindClosestEdge(randPos, out hit, dumbass.areaMask)) {
                Vector3 enemyDir = landMark.transform.position - transform.position;
                Vector3 dir = landMark.transform.position - hit.position;
                dir.y = 0;
                /*
                float dotProd = Vector3.Dot(hit.normal, enemyDir.normalized);
                Debug.Log("Hit Position " + hit.position + ". Dot Product is: " + dotProd);
                if(dotProd < dotProdThresh) { potentialPositions.Add(hit); }
                */

                float angle = Vector3.Angle(hit.normal, dir.normalized);
                if(angle >= 90f) { potentialPositions.Add(hit);
                    Debug.Log("Position: " + hit.position + ". Angle: " + angle);
                    Instantiate(landMark, hit.position, Quaternion.identity);
                }
            }
        }

        if(potentialPositions.Count > 0) {
            NavMeshHit[] arr = potentialPositions.ToArray();
            NavMeshHit[] m = SplitNMerge(arr);

            target = m[0].position;
            dumbass.SetDestination(target);
        }
    }

    void TakeCover2()
    {
        NavMeshHit hit;
        if(NavMesh.FindClosestEdge(landMark.transform.position, out hit, dumbass.walkableMask))
        {

        }
    }

    NavMeshHit[] SplitNMerge(NavMeshHit[] array)
    {
        if(array.Length < 2) { return array; }
        int middle = array.Length / 2;

        NavMeshHit[] leftArray = new NavMeshHit[middle];
        CopyArray(array, leftArray, 0, middle);
        leftArray = SplitNMerge(leftArray);

        NavMeshHit[] rightArray = new NavMeshHit[array.Length - middle];
        CopyArray(array, rightArray, middle, array.Length);
        rightArray = SplitNMerge(rightArray);

        NavMeshHit[] newArray = MergeBack(leftArray, rightArray);
        return newArray;
    }

    NavMeshHit[] MergeBack(NavMeshHit[] a1, NavMeshHit[] a2)
    {
        int a = 0, b = 0;
        int length = a1.Length + a2.Length;
        NavMeshHit[] finishedArray = new NavMeshHit[length];
        for(int i = 0; i < length; i++)
        {
            float distA = -1;
            float distB = -1;
            if (a < a1.Length) { distA = Vector3.Distance(a1[a].position, landMark.transform.position); }
            if (b < a2.Length) { distB = Vector3.Distance(a2[b].position, landMark.transform.position); }

            if(distB == -1) { finishedArray[i] = a1[a]; a++; }
            else if(distA == -1) { finishedArray[i] = a2[b]; b++; }
            else {
                if(distA <= distB) { finishedArray[i] = a1[a]; a++; }
                else { finishedArray[i] = a2[b]; b++; }
            }
        }
        return finishedArray;
    }

    void CopyArray(NavMeshHit[] A, NavMeshHit[] B, int start, int end)
    {
        int idx = start;
        for(int i = 0; i < B.Length; i++) {
            B[i] = A[idx];
            idx++;
        }
    }

    public Vector3 getRandomLocation(Vector3 origin, float range)
    {
        Vector3 randPos = Random.insideUnitSphere * range;
        randPos += landMark.transform.position;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randPos, out navHit, range, dumbass.areaMask);

        return navHit.position;
    }
}
