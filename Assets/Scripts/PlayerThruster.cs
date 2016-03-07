using UnityEngine;
using System.Collections;

public class PlayerThruster : MonoBehaviour {
	public KeyCode key;
	public int maxThrust;
	public float power;
	public float startSize;

	int thrust;

	ParticleSystem ps;

	void Awake () {
		ps = GetComponent<ParticleSystem>();
	}
	void Start () {
		ps.startSize = startSize;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(key)){
			if(Input.GetKey(KeyCode.LeftShift)){
				if(thrust > 0){
					ThrustChange(-1);
				}
			}else{
				if(thrust < maxThrust){
					ThrustChange(1);
				}
			}
		}
	}
	void FixedUpdate(){
		transform.parent.GetComponent<Rigidbody2D>().AddForceAtPosition(transform.forward*thrust*power, transform.position);
	}
	void ThrustChange(int change){
		thrust += change;
		if(thrust == 0){
			ps.Stop();
		}else{
			if(ps.isStopped){
				ps.Play();
			}
			GetComponent<ParticleSystem>().startSpeed = -4 - thrust;
		}
	}
}
