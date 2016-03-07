using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Fighter3 : Hull {
	
	public float movementSpeed;
	public float rotationSpeed;

	public bool debugging;
	bool clearPath = false;
	
	float speedModifier = 1;

	float difference;
	Vector3 destination;
	List<Vector3> pathPoints;
	Vector3 followPos = Vector3.zero;

	bool turnObstructed = false;

	int state = 0;
	enum states : int {idle, flee, attack, travel, travelAway, returnHome, enterHome, enteringHome};


	int homeType = 0;
	enum homeTypes {ship, station};

	//0 = attack
	//1 = follow
	//int targetMode = 0;

	//int count = 0;
	
	public GameObject home;
	GameObject damageNumber;

	GameObject target;
	Rigidbody2D rb;
	LayerMask mask = new LayerMask();
	LayerMask isoMask = 1 << 10;

	void Awake () {
		//Time.timeScale = 0.1f;
		rb = GetComponent<Rigidbody2D>();
		if(gameObject.layer == 9){
			mask = 1 << 8;
		}else if(gameObject.layer == 8){
			mask = 1 << 9;
		}
		pathPoints = new List<Vector3>();
	}
	void FixedUpdate(){
		UpdateMovement();
		UpdateRotation();
	}
	void Update () {
		//Debug.Log(state);
		if(Input.GetKeyDown(KeyCode.UpArrow)){
			Time.timeScale += 0.05f;
		}else if(Input.GetKeyDown(KeyCode.DownArrow)){
			if(Time.timeScale > 0.05f){
				Time.timeScale -= 0.05f;
			}
		}
		//UpdateTarget();
		SetDestination();
		//SetCollideTest(true);
		FindPath();
		//SetCollideTest(false);
		//CheckStation();

		//Debug.DrawLine(transform.position, destination, Color.cyan);
		//Debug.DrawLine(transform.position, detourDestination, Color.blue);
		if(pathPoints.Count > 0){
			Debug.DrawLine(transform.position, pathPoints[pathPoints.Count-1], Color.cyan);
			for(int i=1;i<pathPoints.Count;i++){
				Debug.DrawLine(pathPoints[i-1], pathPoints[i], Color.cyan);
			}
		}

		//Debug.DrawLine(transform.position, currentDestination, Color.yellow);

	}
	void SetDestination(){
		//switches are gross in monodev
		if(state == (int)states.idle){
			GameObject newTarget = FindTarget();
			if(newTarget != null){
				target = newTarget;
				destination = target.transform.position;
				state = (int)states.attack;
			}else{
				if(nearbyEnemies()){
					state = (int)states.flee;
				}else if(homeType == (int)homeTypes.ship){
					if(home == null) return;
					float dist = Vector3.Distance(transform.position, home.transform.position);
					if(home.GetComponent<Rigidbody2D>().velocity.magnitude > 0){
						if(dist > 3){
							destination = home.transform.position;
						}else{
							if(followPos == Vector3.zero){
								followPos = home.transform.position - transform.position;
							}
							destination = (home.transform.position + followPos);
						}
					}else{
						dist = Vector3.Distance(transform.position, destination);
						if(dist < 0.5f || dist > 5){
							Vector3 randDir = new Vector3(Random.Range(-1f,1f),Random.Range(-1f,1f),0).normalized;
							destination = home.transform.position + randDir*Random.Range(2.2f, 4f);
						}
					}
				}
			}
		}
		if(state == (int)states.travel){
			if(Vector3.Distance(destination, transform.position) < 0.2f){
				state = (int)states.idle; 
			}
		}
		if(state == (int)states.travelAway){
			if(Vector3.Distance(target.transform.position, transform.position) > 2f){
				speedModifier = 1f;
				state = (int)states.idle;
				target = null;
			}else{
				speedModifier = 4f;
				destination = transform.position + home.transform.up;
			}
		}
		if(state == (int)states.attack){
			if(target == null){
				state = (int)states.idle;
			}else if(tooClose()){
				//Debug.Log(FindTarget());
				if(FindTarget() == null){
					GetComponent<Hull>().layerIgnore();
					if(Physics2D.CircleCast(transform.position, transform.lossyScale.x, transform.up, 1.5f).collider == null){
						destination = target.transform.position + transform.up*1f;
					}else if(Physics2D.CircleCast(transform.position, transform.lossyScale.x, transform.right, 1.5f).collider == null){
						destination = target.transform.position + transform.right*1f;
					}else if(Physics2D.CircleCast(transform.position, transform.lossyScale.x, -transform.right, 1.5f).collider == null){
						destination = target.transform.position - transform.right*1f;
					}else{
						destination = target.transform.position + transform.up*1f;
					}
					//Debug.Log(gameObject);
					//Debug.Break();
					GetComponent<Hull>().layerDefault();
					target = null;
					state = (int)states.travel;
					//Debug.Log("2close");
				}
			}else{
				if(target.GetComponent<Fighter>() == null){
					Collider2D[] nearby = getNearbyEnemies(5f);
					foreach(Collider2D col in nearby){
						if(col.GetComponent<Fighter>() != null){
							target = null;
							state = (int)states.idle;
						}
					}
				}else{
					destination = target.transform.position;
				}
			}

		}
		if(state == (int)states.flee){
			if(Vector3.Distance(transform.position, destination) > 5){
				state = (int)states.idle;
			}else{
				Collider2D[] col = Physics2D.OverlapCircleAll(transform.position, 5, mask);
				destination = transform.position - new Vector3().averageVector(col);
			}
		}
		if(state == (int)states.returnHome){
			if(home.GetComponent<Rigidbody2D>().velocity.magnitude > 0.5f){
				destination = home.transform.position + home.transform.up * (2f + Vector3.Dot(transform.up, home.transform.up)) + home.GetComponent<Rigidbody2D>().velocity.toVector3();
				Debug.Log("1: " + Mathf.Abs(1 - Vector3.Distance(transform.position, destination)));
				float dotForward = Vector3.Dot(transform.up, home.transform.up);
				float dotFront =  Vector3.Dot((transform.position - home.transform.position).normalized, home.transform.up);
				Debug.Log("2: " + dotForward);
				Debug.Log("3: " + dotFront);
				if(Mathf.Abs(1 - dotForward) < 0.01f && Mathf.Abs(1 - dotFront) < 0.01f ){
					state = (int)states.enterHome;
					Debug.Log("STATE: ENTER HOME");
				}
				if(Vector3.Distance(transform.position, destination) > 2f){
					speedModifier = 3f;
				}else if(Vector3.Distance(transform.position, destination) > 1f){
					speedModifier = 1f;
				}else{
					speedModifier = 0.5f;
				}
			}
		}
		if(state == (int)states.enterHome){
			float dotForward = Vector3.Dot(transform.up, home.transform.up);
			float dotFront =  Vector3.Dot((transform.position - home.transform.position).normalized, home.transform.up);
			if(Mathf.Abs(1 - dotForward) > 0.01f || Mathf.Abs(1 - dotFront) > 0.01f ){
				state = (int)states.returnHome;
			}
			destination = home.transform.position + home.transform.up * 4f;
			speedModifier = 0.3f;
			//Debug.Log("4: " + Vector3.Distance(transform.position, home.transform.position));
			if(Vector3.Distance(transform.position, home.transform.position) < 1f){
				gameObject.SetActive(false);
			}
		}
	}
	void OnTriggerEnter2D(Collider2D col){
		if(state == (int)states.enterHome){
			if(col.GetComponent<SpawnFriendlies>() != null){
				Physics2D.IgnoreCollision(GetComponent<Collider2D>(), home.GetComponent<Collider2D>());
			}
		}
	}

	bool nearbyEnemies(){
		Collider2D[] col = Physics2D.OverlapCircleAll(transform.position, 5, mask);
		return col.Length > 0;
	}
	GameObject FindTarget(){
		Vector3 one = (transform.position + transform.up*2f - transform.right*1.5f);
		Vector3 two = (transform.position + transform.up + transform.right*1.5f);

		if(target != null){
			target.GetComponent<Hull>().layerIgnore();
		}
		Collider2D[] col = getNearbyEnemies(5f);
		if(target != null){
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
		return col[val].gameObject;
	}
	Collider2D[] getNearbyEnemies(float range){
		return Physics2D.OverlapCircleAll(transform.position, range, mask);
	}
	GameObject CheckNear(){
		GetComponent<Hull>().layerIgnore();
		Collider2D nearCol = Physics2D.OverlapCircle(transform.position, transform.lossyScale.x);
		GetComponent<Hull>().layerDefault();
		if(nearCol == null){
			return null;
		}
		return nearCol.gameObject;
	}
	bool tooClose(){
		if(target == null) return false;

		float magnitude = Vector3.Dot(transform.up, rb.velocity) + Vector3.Dot(-transform.up, target.GetComponent<Rigidbody2D>().velocity);

		target.GetComponent<Hull>().layerIsolate();
		bool tooClose = Physics2D.Raycast(transform.position.toVector2(), transform.up.toVector2(), magnitude, isoMask).collider != null;
		Debug.DrawLine(transform.position, transform.position + transform.up*magnitude, Color.red);
		target.GetComponent<Hull>().layerDefault();
		return tooClose;
	}
	float distanceFrom(GameObject go){
		Debug.Log(go);
		go.GetComponent<Hull>().layerIsolate(); // isolate for mask
		float distance = Physics2D.Linecast(transform.position, go.transform.position, isoMask).distance;
		go.GetComponent<Hull>().layerDefault();
		return distance;
	}
	Vector3 detourPoint(Vector3 start, Vector3 fin){

		Vector3 pathDir = fin - start; //Direction from start to finish

		RaycastHit2D ray = new RaycastHit2D();

		Vector3 firstDir = Vector3.zero;
		Vector3 secondDir = Vector3.zero;
		int neg = 1;
		int angle = 5;
		do{
			firstDir = Quaternion.AngleAxis(angle*neg, Vector3.forward) * pathDir;
			firstDir.Normalize();
			ray = Physics2D.CircleCast(start, transform.lossyScale.x, firstDir, pathDir.magnitude);
			//if(ray.collider != null){
			//Debug.DrawRay(transform.position, firstDir*pathDir.magnitude, Color.grey);
			//}

			if(Mathf.Abs(angle) >= 360){
				Debug.Log("No route found.");
				return fin;
			}
			if(neg == -1){
				angle+=5;
			}
			neg*=-1;
		}while(ray.collider != null);

		firstDir = Quaternion.AngleAxis((angle + 5) * -neg, Vector3.forward) * pathDir;
		firstDir.Normalize();

		ray = new RaycastHit2D();
		float inc = 0f;
		do{
			inc+=0.5f;
			secondDir = fin - (start + firstDir*inc);
			secondDir.Normalize();

			ray = Physics2D.CircleCast(start + firstDir*inc, transform.lossyScale.x, secondDir, Vector3.Distance(start + firstDir*inc, fin));
			if(ray.collider != null){
				//Debug.DrawRay(start + firstDir*inc, secondDir*Vector3.Distance(start + firstDir*inc, fin), Color.grey);
			}


			if(inc > 30 || inc < -30){
//				Debug.Log(target);
//				Debug.Log(pathPoints[0]);
//				Debug.Log(state);
//				Debug.Log("OOPS");
				break;
			}
		}while(ray.collider != null);

		return start + firstDir*inc;
	}
	void checkTurnObstruction(Vector3 direction){
		Vector3 dir = rb.velocity.toVector3();

		Debug.DrawRay(transform.position, dir, Color.green);
		GameObject obstruction;
		float dotRight = (Vector3.Dot(direction.normalized, transform.right));
		float dotLeft = (Vector3.Dot(direction.normalized, -transform.right));
		float dotUp = (Vector3.Dot(direction.normalized, transform.up));
		RaycastHit2D ray = Physics2D.CircleCast(transform.position, transform.lossyScale.x/2, dir, dir.magnitude);

		if(dotRight > 0.7 || (dotRight > 0 && dotUp < 0 && dotUp > -1)){
			dir = rb.velocity.toVector3() + transform.right/2;

			if(ray.collider != null){
				if(ray.collider.tag.Equals("Attachement")){
					obstruction = ray.collider.transform.parent.gameObject;
				}else{
					obstruction = ray.collider.gameObject;
				}
				Debug.DrawRay(transform.position, dir, Color.green);
				if(obstruction != null){
					//pathPoints.Add(detourPoint());
				}
			}

		}else if(dotLeft > 0.7 || (dotLeft > 0 && dotUp < 0 && dotUp > -1)){
			dir = rb.velocity.toVector3() - transform.right/2;
			ray = Physics2D.CircleCast(transform.position, transform.lossyScale.x/2, dir, dir.magnitude);
			if(ray.collider != null){
				if(ray.collider.tag.Equals("Attachement")){
					obstruction = ray.collider.transform.parent.gameObject;
				}else{
					obstruction = ray.collider.gameObject;
				}
				Debug.DrawRay(transform.position, dir, Color.green);
				if(obstruction != null){
					//pathPoints.Add(detourPoint(-1));
				}
			}

		}
	}
	void FindPath(){
		pathPoints.Clear();
		pathPoints.Add(destination);
		if(state == (int)states.enterHome || state == (int)states.enteringHome) return;

		GameObject near = CheckNear();
		if(near != null){
			pathPoints.Add(transform.position + (transform.position - near.transform.position).normalized*0.3f);
			return;
		}
		GetComponent<Hull>().layerIgnore();
		if(target != null && state == (int)states.attack){
			target.GetComponent<Hull>().layerIgnore();
		}
		bool straight = Physics2D.CircleCast(transform.position, transform.lossyScale.x/2, transform.up, 1f).collider != null;
		bool left = Physics2D.CircleCast(transform.position, transform.lossyScale.x/2, transform.up - transform.right/2, 1f).collider != null;
		bool right = Physics2D.CircleCast(transform.position, transform.lossyScale.x/2, transform.up + transform.right/2, 1f).collider != null;
		GetComponent<Hull>().layerDefault();
		if(target != null && state == (int)states.attack){
			target.GetComponent<Hull>().layerDefault();
		}
		//Debug.DrawLine(transform.position, transform.position + transform.up, Color.green);
		//Debug.DrawLine(transform.position, transform.position + transform.up - transform.right/2, Color.green);
		//Debug.DrawLine(transform.position, transform.position + transform.up + transform.right/2, Color.green);
		if(debugging){
			Debug.Log("Straight: " + straight);
			Debug.Log("Left: " + left);
			Debug.Log("Right: " + right);
		}
		if(left){
			pathPoints.Add(transform.position + transform.right);
			return;
		}else if(right){
			pathPoints.Add(transform.position - transform.right);
			return;
		}else if(straight){
			pathPoints.Add(transform.position - transform.up);
			return;
		}

		GetComponent<Hull>().layerIgnore();
		if(target != null && state == (int)states.attack){
			target.GetComponent<Hull>().layerIgnore();
		}
		Vector3 direction =  destination - transform.position;
		RaycastHit2D ray = Physics2D.CircleCast(transform.position, transform.lossyScale.x/2, direction, direction.magnitude);
		Debug.DrawRay(transform.position, direction, Color.green);
		GameObject obstruction;
		if(ray.collider != null){
			if(ray.collider.tag.Equals("Attachement")){
				obstruction = ray.collider.transform.parent.gameObject;
			}else{
				obstruction = ray.collider.gameObject;
			}
			//Debug.Log("Collision Course: " + gameObject + " : " + obstruction);
			//float vDotLeft = (Vector3.Dot(obstruction.GetComponent<Rigidbody2D>().velocity.toVector3(), -transform.right));
			//float vDotRight = (Vector3.Dot(obstruction.GetComponent<Rigidbody2D>().velocity.toVector3(), transform.right));
			pathPoints.Add(detourPoint(transform.position, destination));
		}
		if(target != null && state == (int)states.attack){
			target.GetComponent<Hull>().layerDefault();
		}
		GetComponent<Hull>().layerDefault();
	}

	void UpdateRotation(){
		if(pathPoints.Count == 0) return;
		Vector3 direction;
		float difference;

		direction = pathPoints[pathPoints.Count-1] - transform.position;
		difference = (Vector3.Dot(direction.normalized, transform.right));

		if(difference > 0){
			rb.AddTorque(-rotationSpeed/60);
		}else{
			rb.AddTorque(rotationSpeed/60);
		}
	}
	void UpdateMovement(){
		if(pathPoints.Count == 0) return;
		rb.AddForce(transform.up*movementSpeed*speedModifier/60);
	}

	public void commandAway(GameObject target){
		state = (int)states.travelAway;
		this.target = target;
	}
	public void commandReturnHome(){
		state = (int)states.returnHome;
	}
	public void setHome(GameObject val){home = val;}

	//RETURN VALS
	public bool canShoot(){
		RaycastHit2D ray = Physics2D.CircleCast(transform.position, transform.lossyScale.x*2f, transform.up, 5, mask);
		return state == (int)states.attack && ray.collider != null;
	}
	public int 	getState(){return state;}
//	public float getDistance(){return distance;}
//	public GameObject getStation(){return station;}

}
