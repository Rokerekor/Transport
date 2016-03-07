using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Crosshair : MonoBehaviour {

	public GameObject prefabTargeted;
	public static List<Targeted> targets = new List<Targeted>();

	void Start(){
		//menu.SetActive(false);
	}
	void Update () {
		transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition).setZ(0);

		if(Input.GetKeyDown(KeyCode.Mouse1)){
			Collider2D col = Physics2D.OverlapCircle(Camera.main.ScreenToWorldPoint(Input.mousePosition), 0.32f, 1 << 9);
			if(col != null && !isTarget(col.gameObject)){
				GameObject newTargeted = (GameObject)Instantiate(prefabTargeted, col.transform.position, Extensions.ZeroQuaternion());
				newTargeted.GetComponent<Targeted>().setTarget(col.gameObject);
				newTargeted.GetComponent<Targeted>().setCrosshair(this);
				targets.Add(newTargeted.GetComponent<Targeted>());
			}
			col = Physics2D.OverlapCircle(Camera.main.ScreenToWorldPoint(Input.mousePosition), 0.32f);
		}
//		if(Input.GetKeyDown(KeyCode.Mouse2)){
//			Collider2D col = Physics2D.OverlapCircle(Camera.main.ScreenToWorldPoint(Input.mousePosition), 0.32f, 1 << 8);
//			if(col != null){
//				Fighter fighter = col.GetComponent<Fighter>();
//				if(fighter != null){
//					fighter.commandEnterBay();
//					return;
//				}
//			}
//		}
	}
	public static void targetDestroyed(Targeted target){
		targets.Remove(target);
		Destroy(target.gameObject);
	}
//	public static GameObject[] getAvailableTargets(){
//		return newTargeted.ToArray();
//	}
	public static GameObject giveNearestAvailableTarget(GameObject requester, int damage){
		List<GameObject> available = new List<GameObject>();
		foreach(Targeted t in targets.ToArray()){
			if(!t.dealtWith()){
				available.Add(t.getTarget());
			}
		}
		GameObject go = available.ToArray().getNearestTo(requester);
		if(go != null)
			getTargeted(go).addIncomingDamage(damage);
		return go;
	}
	public static Targeted getTargeted(GameObject target){
		foreach(Targeted t in targets.ToArray()){
			if(t.getTarget() == target){
				return t;
			}
		}
		return null;
	}
	public static Crosshair getCrosshair(){
		return GameObject.Find("crosshair").GetComponent<Crosshair>();
	}
//	public static void checkTarget(GameObject go){
//		Debug.Log(go);
//		if(go != null){
//			available.Add(go);
//			unavailable.Remove(go);
//		}
//	}
	static bool isTarget(GameObject target){
		foreach(Targeted targeted in targets){
			if(target == targeted.getTarget()) return true;
		}
		return false;
	}
//	public static GameObject[] getTargets(){
//		Crosshair ch = getCrosshair();
//		return ch.targets.ToArray();
//	}


}
