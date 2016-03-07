using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Fighter : ShipAI {
	
	public bool friendly;
	public bool debugging;

	int state = 0;
	enum states : int {busy = 0, enterBay, leaveBay};

	int homeType;
	enum homeTypes: int { ship };

	GameObject damageNumber;
	GameObject currentOrder;

	LayerMask mask = new LayerMask();
	LayerMask isoMask = 1 << 10;

	Stack<IEnumerator> tasks = new Stack<IEnumerator>();

	protected override void  Start () {
		base.Start();
		if(gameObject.layer == 9){
			mask = 1 << 8;
		}else if(gameObject.layer == 8){
			mask = 1 << 9;
		}
		if(friendly){
			hull.setGibFolder("Sprites/friendlyfighter/gibs");
			GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/friendlyfighter/friendlyfighter");
		}else{
			hull.setGibFolder("Sprites/enemyfighter/gibs");
			GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/enemyfighter/enemyfighter");
		}
		StartCoroutine(fighterBehaviour());
	}
	IEnumerator fighterBehaviour(){
		while(true){
			GameObject target = FindTarget();
			if(state != 0 || target != null || currentOrder != null){
				IEnumerator newTask = null;
				taskInProgress = true;
				if(state == (int)states.leaveBay){
					setSpriteLayer((int)spriteLayer.enterShip);
					newTask = goToTargetPos(bay, transform.up + transform.right * Random.Range(-1f, 1f), 0.5f);
				}else if(currentOrder != null){
					newTask = order();
				}if(target != null){
					newTask = attack(target);
				}else if(state == (int)states.enterBay){
					newTask = enterBay(bay);
				}
				while(tasks.Count > 0){
					StopCoroutine(tasks.Pop());
				}
				StartCoroutine(newTask);
				yield return new WaitWhile(() => taskInProgress);
				if(state == (int)states.leaveBay){
					setSpriteLayer((int)spriteLayer.def);
				}
				state = 0;
			}else if(tasks.Count == 0){
				tasks.Push(idle());
				StartCoroutine(tasks.Peek());
			}
			yield return new WaitForFixedUpdate();
		}
	}
	IEnumerator idle(){
		while(true){
			if(home == null)
				yield break;
			Rigidbody2D homerb = home.GetComponent<Rigidbody2D>();
			if(homerb != null){
				if(homerb.velocity.magnitude > 0.5f){
					taskInProgress = true;
					tasks.Push(goToTargetPos(home, transform.position - home.transform.position + home.transform.up * 1, 0.05f));
					StartCoroutine(tasks.Peek());
					yield return new WaitWhile(() => taskInProgress);
					tasks.Pop();
				}
			}

			taskInProgress = true;
			tasks.Push(goToTargetPos(home, Extensions.Random2DVector3(2f, 3f), 1f));
			StartCoroutine(tasks.Peek());
			yield return new WaitWhile(() => taskInProgress);
		}
	}
	IEnumerator order(){
		while(currentOrder != null){
			taskInProgress = true;
			tasks.Push(goToTargetPos(currentOrder, Extensions.Random2DVector3(2f, 3f), 1f));
			StartCoroutine(tasks.Peek());
			yield return new WaitWhile(() => taskInProgress);
			tasks.Pop();
		}
	}
	IEnumerator attack(GameObject target){
		while(target != null){
			//THIS BLOCK MOVES PLAYER TOWARDS ENEMY
			float dist = 1f;
			if(target.GetComponent<PlayerController>() != null){
				dist = 2f;
			}
			taskInProgress = true;
			StartCoroutine(goToTargetPos(target, Vector3.zero, dist));
			yield return new WaitWhile(() => taskInProgress);
		
			//THIS BLOCK REPOSITIONS
			if(target == null)
				yield break;
			if(target.GetComponent<PlayerController>() != null){
				dist = 3f;
			}else{
				dist = 2f;
			}
			taskInProgress = true;
			StartCoroutine(goToPos(transform.position + transform.up * dist + transform.right * Random.Range(-2f, 2f), 0.5f));
			yield return new WaitWhile(() => taskInProgress);
			target = FindTarget(target);
		}
	}

	bool nearbyEnemies(){
		Collider2D[] col = Physics2D.OverlapCircleAll(transform.position, 5, mask);
		return col.Length > 0;
	}
	GameObject FindTarget(GameObject target = null){
		Vector3 one = (transform.position + transform.up*2f - transform.right*1.5f);
		Vector3 two = (transform.position + transform.up + transform.right*1.5f);

		if(target != null && target.GetComponent<Hull>()){
			target.GetComponent<Hull>().layerIgnore();
		}
		Collider2D[] col = getNearbyEnemies(5f);
		if(target != null && target.GetComponent<Hull>() != null){
			target.GetComponent<Hull>().layerDefault();
		}
		if(col.Length == 0) return null;
		float[] targetVal = new float[col.Length];

		for(int i=0;i<col.Length-1;i++){
			if(col[i].GetComponent<Hull>())
				targetVal[i] = (1/Vector3.Distance(col[i].transform.position, transform.position)*Vector3.Dot(col[i].transform.position - transform.position, transform.up));
			if(col[i].GetComponent<Fighter>() != null){
				targetVal[i]++;
			}
		}
		int val = 0;
		for(int i=1;i<col.Length-1;i++){
			if(col[i].GetComponent<Hull>() == null) continue;
			if(distanceFrom(col[i].gameObject) < 1.5f) continue;
			if(targetVal[i] > targetVal[val]){
				val = i;
			}
		}
		if(col[val].gameObject == null){
			return target;
		}
		return col[val].gameObject;
	}
	Collider2D[] getNearbyEnemies(float range){
		return Physics2D.OverlapCircleAll(transform.position, range, mask);
	}

	float distanceFrom(GameObject go){
		go.GetComponent<Hull>().layerIsolate(); // isolate for mask
		float distance = Physics2D.Linecast(transform.position, go.transform.position, isoMask).distance;
		go.GetComponent<Hull>().layerDefault();
		return distance;
	}
	public void commandOrder(GameObject order){
		Debug.Log(order);
		currentOrder = order;
	}
	public void commandEnterBay(){
		state = (int)states.enterBay;
	}
	public void commandLeaveBay(){
		state = (int)states.leaveBay;
	}
	public void setHome(GameObject val){home = val;}

	//RETURN VALS
	public bool canShoot(){
		RaycastHit2D ray = Physics2D.CircleCast(transform.position, transform.lossyScale.x*2f, transform.up, 5, mask);
		return ray.collider != null && ray.collider.GetComponent<Hull>() != null;
	}
	public int 	getState(){return state;}
	public bool	getFriendly(){return friendly;}
	public void	setFriendly(bool friendly){this.friendly = friendly;}

}
