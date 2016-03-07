using UnityEngine;
using System.Collections;

public class TeslaCoil : MonoBehaviour {
	public int damage;
	public float maxPower;
	public float dischargeCost;
	public float rechargeSpeed;

	float power;

	LayerMask mask;

	LineRenderer lr;
	void Awake(){
		lr = GetComponent<LineRenderer>();
	}
	void Start () {
		power = maxPower;
		lr.SetPosition(0, transform.position);
		lr.SetPosition(1, transform.position);
		if(gameObject.layer == 9){
			mask = 1 << 8;
		}else if(gameObject.layer == 8){
			mask = 1 << 9;
		}
		StartCoroutine(teslaBehaviour());
		StartCoroutine(powerCharge());
	}
	IEnumerator powerCharge(){
		while(true){
			transform.Find("indicator").localScale =  Vector3.one * transform.Find("indicator").localScale.x / transform.Find("indicator").GetComponent<SpriteRenderer>().bounds.size.x * power * 2;

			power += 0.1f;
			if(power > maxPower)
				power = maxPower;
			yield return new WaitForSeconds(0.2f);
		}
	}
	IEnumerator teslaBehaviour(){
		while(true){
			Collider2D col = Physics2D.OverlapCircle(transform.position, power, mask);
			if(col != null && col.GetComponent<Hull>() != null){
				lr.SetPosition(1, col.transform.position);
				col.GetComponent<Hull>().receiveDamage(gameObject, damage, Vector3.zero, Vector3.zero);
				yield return new WaitForSeconds(0.1f);
				lr.SetPosition(1, transform.position);
				power -= dischargeCost;
				yield return new WaitForSeconds(0.5f);
			}
			yield return new WaitForFixedUpdate();
		}
	}
}
