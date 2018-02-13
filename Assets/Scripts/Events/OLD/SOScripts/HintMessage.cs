using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Custom Delegate/HintMessage")]
public class HintMessage : EventFunction {
    
    public override IEnumerator doThing(GameObject target, float delay, string message, int count)
    {
        yield return new WaitForSeconds(delay);
        GameObject chatMessage = Instantiate(target, PlayerDamageable.Instance.playerCanvas);
        GameHint hint = chatMessage.GetComponent<GameHint>();
        if(count > 0) { hint.lifeSpan = count; } // override lifespan if needed
        chatMessage.GetComponent<GameHint>().message = message;
    }
}
