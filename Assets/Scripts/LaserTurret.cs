using UnityEngine;
using System.Collections;

public class LaserTurret : MonoBehaviour {
	public GameObject bullet;
	public float rotationSpeed;
	public float cooldown;
	public float range;
	public bool turnRestricted;

	GameObject target;

	LayerMask mask = new LayerMask();
	bool canShoot;

	void Start () {
		if(gameObject.layer == 9){
			mask = 1 << 8;
		}else if(gameObject.layer == 8){
			mask = 1 << 9;
		}
		StartCoroutine(targetManagement());
		StartCoroutine(rotationManagement());
		StartCoroutine(shooting());
	}
	GameObject findTarget(){
		Collider2D[] col = Physics2D.OverlapCircleAll(transform.position, range, mask);
		if(col.Length == 0) return null;
		int closest = 0;
		float distance = Mathf.Infinity;
		for(int i=0;i<col.Length;i++){
			if(turnRestricted && Vector3.Dot(transform.up, col[i].transform.position - transform.position) < 0) continue;
			if(Vector3.Distance(transform.position, col[i].transform.position) < distance){
				closest = i;
			}
		}
		return col[closest].gameObject;
	}
	IEnumerator targetManagement(){
		while(true){
			GameObject oldTarget = target;
			target = findTarget();
			if(target != oldTarget){
				//yield return new WaitForSeconds(1f);
			}
			yield return new WaitForFixedUpdate();
			if(target == null){
				continue;
			}
			if(turnRestricted && Vector3.Dot(transform.up, target.transform.position - transform.position) < 0){
				target = null;
				continue;
			}
			if(Vector3.Distance(transform.position, target.transform.position) > range){
				target = null;
				continue;
			}

		}
	}
	IEnumerator rotationManagement(){
		while(true){
			yield return new WaitForFixedUpdate();
			canShoot = false;
			if(target == null){
				continue;
			}
			Vector3 direction = target.transform.position - transform.position;
			float difference = (Vector3.Dot(direction.normalized, transform.right));

			if(difference > 0.1){
				transform.Rotate(new Vector3(0,0, -rotationSpeed));
			}else if(difference < -0.1){
				transform.Rotate(new Vector3(0,0, rotationSpeed));
			}else{
				canShoot = true;
			}
		}
	}
	IEnumerator shooting(){
		while(true){
			yield return new WaitUntil(() => canShoot);
			Bullet.spawnBullet(gameObject);
			yield return new WaitForSeconds(cooldown);
		}
	}
}
