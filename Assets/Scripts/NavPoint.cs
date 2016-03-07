using UnityEngine;
using System.Collections;

public class NavPoint : MonoBehaviour {

	GameObject precedingObject;
	void Start () {
		transform.localScale = Vector3.zero;
	}
	
	void FixedUpdate () {
		GetComponent<LineRenderer>().SetPosition(0, transform.position - (transform.position - precedingObject.transform.position).normalized * GetComponent<SpriteRenderer>().bounds.size.x/2);
		if(precedingObject.tag.Equals("Player")){
			GetComponent<LineRenderer>().SetPosition(1, precedingObject.transform.position);
		}else{
			GetComponent<LineRenderer>().SetPosition(1, precedingObject.transform.position + (transform.position - precedingObject.transform.position).normalized * precedingObject.GetComponent<SpriteRenderer>().bounds.size.x/2);
		}
		if(transform.localScale.magnitude < 1){
			transform.localScale = transform.localScale + new Vector3(0.05f,0.05f,0.05f);
		}else{
			transform.localScale = Vector3.one;
		}
	}
	public void setPrecedingObject(GameObject precedingObject){
		this.precedingObject = precedingObject;
	}
}