using UnityEngine;
using System.Collections;

public class Generator : MonoBehaviour {
	public float maxPower;
	public float regenSpeed;
	public float overloadTime;

	float power;
	bool overloaded;

	void Start () {
		StartCoroutine(powerGen());
		power = maxPower;
	}
	void Update(){
		
	}
	IEnumerator powerGen(){
		while(true){
			if(overloaded){
				yield return new WaitForSeconds(overloadTime);
			}
			power += regenSpeed;
			if(power > maxPower) power = maxPower;
			yield return new WaitForSeconds(0.1f);
		}
	}
	public bool drawPower(float drawn){
		if(overloaded) return false;
		if(drawn > power){
			overloaded = true;
			return false;
		}
		power -= drawn;
		return true;
	}
	public float powerPercent(){
		return power/maxPower;
	}
}
