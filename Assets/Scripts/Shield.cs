using UnityEngine;
using System.Collections;

public class Shield : Hull {
	public float rechargeDelay;
	public float rechargeRate;
	Generator generator;
	float damageBuffer;

	Color dColor;
	SpriteRenderer sr;
	void Awake(){
		generator = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Generator>();
		sr = GetComponent<SpriteRenderer>();
	}
	protected override void Start(){
		base.Start();
		sr.color = sr.color.setAlpha(0.1f);
		dColor = sr.color;
		StartCoroutine("appearance");
	}
	public override bool receiveDamage(GameObject attacker, int damage, Vector3 pos, Vector3 dir){
		generator.drawPower(damage);
		healthChange(-damage);
		damageBuffer += damage;
		StopCoroutine("recharge");
		StartCoroutine("recharge");
		return true;
	}
	protected override void death(){
		if(playOnDeath.Length > 0){
			audioSource.clip = playOnDeath[Random.Range(0,playOnDeath.Length)];
			audioSource.Play();
		}
		StartCoroutine("recover");
	}
	IEnumerator appearance(){
		while(true){
			//set alpha
			if(damageBuffer > 0){
				if(sr.color.a < 0.5f){
					sr.color = sr.color.setAlpha(sr.color.a + 0.01f);
				}
				damageBuffer -= 1f;
			}else{
				if(sr.color.a > 0.1f){
					sr.color = sr.color.setAlpha(sr.color.a - 0.001f);
				}
			}
			//set color

			Color newColor = sr.color;
			newColor.r = dColor.r + (1f-dColor.r)*(1f - health/100f);
			newColor.g = dColor.g * (health/100f);
			newColor.b = dColor.b * (health/100f);
			sr.color = newColor;
			yield return new WaitForFixedUpdate();
		}
	}
	IEnumerator recharge(){
		yield return new WaitForSeconds(rechargeDelay);
		while(health < maxHealth){
			health++;
			yield return new WaitForSeconds(rechargeRate);
		}
	}
	IEnumerator recover(){
		yield return new WaitForSeconds(rechargeDelay + rechargeRate*maxHealth);
		health = maxHealth;
	}
}
