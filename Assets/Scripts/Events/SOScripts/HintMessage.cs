using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Custom Delegate/HintMessage")]
public class HintMessage : EventFunction {

    [SerializeField] string message;

    public override IEnumerator doThing(GameObject target, float delay)
    {
        yield return new WaitForSeconds(delay);
        GameObject chatMessage = Instantiate(target, PlayerDamageable.Instance.playerCanvas);
        chatMessage.GetComponent<Text>().text = message;
        Destroy(chatMessage);
    }
}
