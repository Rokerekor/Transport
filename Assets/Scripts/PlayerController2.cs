using UnityEngine;
using System.Collections;

public class PlayerController2 : MonoBehaviour {


	public GameObject forwardThruster;
	public GameObject[] leftThrusters;
	public GameObject[] rightThrusters;

	// mainThruster;
	// frontLeftThruster;
	// frontRightThruster;
	// backLeftThruster;
	// backRightThruster;

	public float mainThrustInc;
	public int mainThrustMax;
	public float sideThrustInc;
	public int sideThrustMax;

	int mainThrust;
	int sideThrust;

	int sideHoldCounter;
	int forwardHoldCounter;

	const int holdSpeed = 10;


	Rigidbody2D rb;

	int[] thrust = new int[5];

	// mainThruster;
	// frontLeftThruster;
	// frontRightThruster;
	// backLeftThruster;
	// backRightThruster;

	void Awake(){
		rb = GetComponent<Rigidbody2D>();
	}
	void Update () {
		checkKeys();
	}
	void FixedUpdate(){
		applyThrust();
	}
	void applyThrust(){
		if(mainThrust != 0){
			forwardThruster.SetActive(true);
			rb.AddForceAtPosition(transform.up * mainThrust * mainThrustInc, forwardThruster.transform.position);
			forwardThruster.GetComponent<ParticleSystem>().startSpeed = -5 - mainThrust;
		}else{
			forwardThruster.SetActive(false);
		}
		GameObject[] sideThrusters;
		int tempSideThrust = sideThrust;
		if(sideThrust > 0){
			sideThrusters = leftThrusters;
		}else if(sideThrust < 0){
			sideThrusters = rightThrusters;
			tempSideThrust *= -1;
		}else{
			rightThrusters[0].SetActive(false);
			rightThrusters[1].SetActive(false);
			leftThrusters[0].SetActive(false);
			leftThrusters[1].SetActive(false);
			return;
		}
		sideThrusters[0].SetActive(true);


		int remainder = tempSideThrust - sideThrustMax;
		if(remainder > 0){
			rb.AddForceAtPosition(sideThrusters[1].transform.forward * remainder * sideThrustInc, sideThrusters[1].transform.position);
			sideThrusters[1].GetComponent<ParticleSystem>().startSpeed = - remainder;
			tempSideThrust = sideThrustMax;
			sideThrusters[1].SetActive(true);
		}else{
			sideThrusters[1].SetActive(false);
		}
		rb.AddForceAtPosition(sideThrusters[0].transform.forward * tempSideThrust * sideThrustInc, sideThrusters[0].transform.position);
		sideThrusters[0].GetComponent<ParticleSystem>().startSpeed =  -tempSideThrust;
	}
	void checkKeys(){
		if(Input.GetButtonDown("forward")){
			if(mainThrust < mainThrustMax){
				mainThrust++;
			}
		}
		if(Input.GetButtonDown("backward")){
			if(mainThrust > 0){
				mainThrust--;
			}
		}
		if(Input.GetButtonDown("right")){
			if(sideThrust < sideThrustMax * 2){
				sideThrust++;
			}
		}
		if(Input.GetButtonDown("left")){
			if(sideThrust > -sideThrustMax * 2){
				sideThrust--;
 			}
		}
		if(Input.GetButton("right")){
			if(sideThrust < sideThrustMax * 2){
				sideHoldCounter++;
				if(sideHoldCounter >= holdSpeed){
					sideHoldCounter = 0;
					sideThrust++;
				}
			}

		}else if(Input.GetButton("left")){
			if(sideThrust > -sideThrustMax * 2){
				sideHoldCounter++;
				if(sideHoldCounter >= holdSpeed){
					sideHoldCounter = 0;
					sideThrust--;
				}
			}
		}else{
			sideHoldCounter = 0;
		}
	}
}