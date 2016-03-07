using UnityEngine;
using System.Collections;

public abstract class Weapon : MonoBehaviour {

	public char key;
	public float cooldown;

	void Start () {
		StartCoroutine("manager");
	}
	IEnumerator manager(){
		while(true){
			string keyName = "attack" + key;
			if(Input.GetButtonDown(keyName)){
				activate();
				GameObject.FindGameObjectWithTag("HUD").transform.Find("Ability" + key).GetComponent<AbilityGUI>().abilityUsed(cooldown);
				yield return new WaitForSeconds(cooldown);
			}
			yield return new WaitForFixedUpdate();
		}
	}

	abstract protected void activate();
}
