using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraEffects : MonoBehaviour {

	public int minSize;
	public int maxSize;

	GameObject player;
	Vector3 fixedPos;
	Vector3 velocity = Vector3.zero;
	List<GameObject> colList = new List<GameObject>();

	void Awake(){
		player = GameObject.FindGameObjectWithTag("Player");
	}
	void Start () {
	
	}
	void Update(){
		float change = -Input.GetAxis("zoom");
		float size = GetComponent<Camera>().orthographicSize;
		size += change;
		if(size > maxSize){
			size = maxSize;
		}else if(size < minSize){
			size = minSize;
		}
		GetComponent<Camera>().orthographicSize = size;
	}
	void FixedUpdate () {
		FollowPlayer();
	}
	void OnTriggerStay2D(Collider2D col){
		if(col.gameObject.layer == 9){
			colList.Add(col.gameObject);
		}
	}
	void FollowPlayer(){
		if(player == null) return;
		Vector3 targetPos = player.transform.position;
		for(int i=0;i<colList.Count;i++){
			if(colList[i] != null){
				targetPos += colList[i].transform.position;
				targetPos += player.transform.position;
			}
		}
		targetPos /= 1+colList.Count*3; 
		fixedPos = Vector3.SmoothDamp(fixedPos, targetPos, ref velocity, 1f,10);
		colList.Clear();
		transform.position = new Vector3(fixedPos.x,fixedPos.y,-10);
	}
}
