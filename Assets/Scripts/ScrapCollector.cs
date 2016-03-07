using UnityEngine;
using System.Collections;

public class ScrapCollector : ShipAI {
	GameObject scrap;
	LayerMask scrapMask = 1 << 11;

	Vector3 destination;

	enum states : int {onRoute, returnHome, enterHome};

	bool hasScrap;


	LineRenderer lr;

	const float distFromScrap = 0.5f;

	protected override void Awake () {
		base.Awake();
		lr = GetComponent<LineRenderer>();
	}
	IEnumerator scrapperBehaviour(){
		setSpriteLayer((int)spriteLayer.enterShip);
		StartCoroutine(goToPos(bay.transform.position + bay.transform.up, 0.3f));
		taskInProgress = true;
		yield return new WaitWhile(() => taskInProgress);
		setSpriteLayer((int)spriteLayer.def);
		StartCoroutine(goTo(scrap, distFromScrap));
		taskInProgress = true;
		yield return new WaitWhile(() => taskInProgress);
		taskInProgress = true;
		StartCoroutine(faceScrap());
		StartCoroutine(harvestScrap());
		yield return new WaitWhile(() => taskInProgress);
		StartCoroutine(enterBay(bay));
		taskInProgress = true;
	}
	IEnumerator harvestScrap(){
		int i = 0;
		lr.SetPosition(0, transform.position + transform.up * 0.05f);
		lr.SetPosition(1, transform.position);
		while(i < 5){
			yield return new WaitForSeconds(0.5f);
			lr.SetPosition(1, transform.position + transform.up * Random.Range(0.3f, 0.6f) + transform.right * Random.Range(-0.05f, 0.05f));
			yield return new WaitForSeconds(0.1f);
			lr.SetPosition(1, transform.position);
			i++;
		}
		lr.SetPosition(0, transform.position);
		lr.SetPosition(1, transform.position);
		taskInProgress = false;
		Destroy(scrap);
	}
	IEnumerator faceScrap(){
		while(taskInProgress && scrap != null){
			if(Vector3.Distance(transform.position, scrap.transform.position) > 1.2*distFromScrap){
				rb.AddForce((scrap.transform.position - transform.position).normalized);
			}
			Vector3 direction = scrap.transform.position - transform.position;
			float difference = (Vector3.Dot(direction.normalized, transform.right));
			if(difference > 0){
				rb.AddTorque(-rotationSpeed);
			}else{
				rb.AddTorque(rotationSpeed);
			}
			yield return new WaitForFixedUpdate();
		}
	}
	public void setScrap(GameObject val){
		scrap = val;
		StartCoroutine(scrapperBehaviour());
	}
}
