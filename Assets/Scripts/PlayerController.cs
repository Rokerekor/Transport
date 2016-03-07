using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {
	//Public
	public float moveSpeed;
	public float rotationSpeed;

	public GameObject navPoint;

	//Components
	Rigidbody2D rb;
	LineRenderer lr;

	//Private
	List<GameObject> navPoints = new List<GameObject>();
	bool mouseDown;

	void Start () {
		rb = GetComponent<Rigidbody2D>();
		lr = GetComponent<LineRenderer>();
	}
	
	void FixedUpdate(){
		if(navPoints.Count > 0){
			Rotate();
			Move();
		}
	}
	void Update () {
		if(Input.GetButtonDown("move")){
			if(!Input.GetKey(KeyCode.LeftShift)){
				clearNavPoints();
			}
			AddNavPoint();
		}

	}
	void clearNavPoints(){
		foreach(GameObject nav in navPoints){
			Destroy(nav);
		}
		navPoints.Clear();
	}
	void AddNavPoint(){
		if(navPoints.Count >= 3) return;
		Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		pos.z = 0;
		GameObject newNavPoint = (GameObject)Instantiate(navPoint, pos, Extensions.ZeroQuaternion());
		if(navPoints.Count > 0){
			newNavPoint.GetComponent<NavPoint>().setPrecedingObject(navPoints[navPoints.Count-1]);
		}else{
			newNavPoint.GetComponent<NavPoint>().setPrecedingObject(gameObject);
		}
		navPoints.Add(newNavPoint);
	}
	void Rotate(){
		Vector3 direction = transform.position - navPoints[0].transform.position;
		float difference = transform.rotation.eulerAngles.z - Quaternion.LookRotation(transform.forward, - direction).eulerAngles.z;
		if(difference > 180){
			difference -= 360;
		}else if(difference < -180){
			difference += 360;
		}
		if(Mathf.Abs(difference) < 1){
			Adjust();
		}else{
			float dir = Mathf.Clamp(-difference, -1, 1);
			rb.AddTorque(dir*rotationSpeed);
		}
	}
	void Move(){
		rb.AddForce(transform.up*moveSpeed);

		float distance = Vector3.Distance(transform.position, navPoints[0].transform.position);
		if(distance < 3){
			Destroy(navPoints[0]);
			navPoints.RemoveAt(0);
			if(navPoints.Count > 0){
				navPoints[0].GetComponent<NavPoint>().setPrecedingObject(gameObject);
			}
		}

	}
	void Adjust(){
		rb.AddForce((transform.up - rb.velocity.toVector3()));
	}
//	void UpdateLR(){
//		lr.enabled = targetPos.Count > 0;
//		lr.SetPosition(0, transform.position);
//		lr.SetVertexCount(targetPos.Count+1);
//		for(int i=0;i<targetPos.Count;i++){
//			lr.SetPosition(i+1, targetPos[i]);
//		}
//	}
}
