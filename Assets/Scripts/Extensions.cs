using UnityEngine;
using System.Collections;


public static class Extensions {
	//QUATERNION
	public static Quaternion ZeroQuaternion(){
		return new Quaternion(0,0,0,0);
	}
	//TRANSFORM
	public static void setX(this Transform transform, float x){
		transform.position = new Vector3(x, transform.position.y, transform.position.z);
	}
	public static void setY(this Transform transform, float y){
		transform.position = new Vector3(transform.position.x, y, transform.position.z);
	}
	public static void setZ(this Transform transform, float z){
		transform.position = new Vector3(transform.position.x, transform.position.y, z);
	}
	public static void setEulerX(this Transform transform, float x){
		transform.eulerAngles = new Vector3(x,transform.eulerAngles.y,transform.eulerAngles.z);
	}
	public static void setEulerY(this Transform transform, float y){
		transform.eulerAngles = new Vector3(transform.eulerAngles.x,y,transform.eulerAngles.z);
	}
	public static void setEulerZ(this Transform transform, float z){
		transform.eulerAngles = new Vector3(transform.eulerAngles.x,transform.eulerAngles.y,z);
	}
	public static void setLocalEulerX(this Transform transform, float x){
		transform.localEulerAngles = new Vector3(x,transform.localEulerAngles.y,transform.localEulerAngles.z);
	}
	public static void setLocalEulerY(this Transform transform, float y){
		transform.localEulerAngles = new Vector3(transform.localEulerAngles.x,y,transform.localEulerAngles.z);
	}
	public static void setLocalEulerZ(this Transform transform, float z){
		transform.localEulerAngles = new Vector3(transform.localEulerAngles.x,transform.localEulerAngles.y,z);
	}
	public static GameObject findNearestEnemy(this GameObject gameObject, float radius){
		LayerMask mask = new LayerMask();
		if(gameObject.layer == 9){
			mask = 1 << 8;
		}else if(gameObject.layer == 8){
			mask = 1 << 9;
		}
		Collider2D[] cols = Physics2D.OverlapCircleAll(gameObject.transform.position, radius, mask);

		float distance = Mathf.Infinity;
		GameObject closest = null;
		foreach(Collider2D col in cols){
			float test = Vector3.Distance(col.transform.position, gameObject.transform.position);
			if(test < distance){
				distance = test;
				closest = col.gameObject;
			}
		}
		return closest;
	}

	//VECTOR2
	public static Vector3 toVector3(this Vector2 vec){
		return new Vector3(vec.x,vec.y,0);
	}

	//VECTOR3
	public static Vector2 toVector2(this Vector3 vec){
		return new Vector2(vec.x,vec.y);
	}
	public static Vector3 setX(this Vector3 vec, float x){
		return new Vector3(x, vec.y, vec.z);
	}
	public static Vector3 setY(this Vector3 vec, float y){
		return new Vector3(vec.x, y, vec.z);
	}
	public static Vector3 setZ(this Vector3 vec, float z){
		return new Vector3(vec.x, vec.y, z);
	}

	public static Vector3 averageVector(this Vector3 vec, Vector3[] toAverage){
		Vector3 total = Vector3.zero;
		foreach(Vector3 newVec in toAverage){
			total += newVec;
		}
		return total / toAverage.Length;
	}
	public static Vector3 averageVector(this Vector3 vec, Collider2D[] toAverage){
		Vector3 total = Vector3.zero;
		foreach(Collider2D col in toAverage){
			total += col.transform.position;
		}
		return total / toAverage.Length;
	}
	public static Vector3 Random2DVector3(float min, float max){
		return new Vector3(Random.value * 2 - 1, Random.value * 2 - 1,0).normalized * Random.Range(min, max);
	}

/*	//NETWORK PLAYER
	public static Player getPlayer(this NetworkPlayer netPlayer){
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		foreach(GameObject p in players){
			if(p.GetComponent<Hull>().getNetPlayer() == netPlayer){
				return p.GetComponent<Player>();
			}
		}
		return null;
	}*/

	//COLOR
	public static Color setAlpha(this Color color, float alpha){
		return new Color(color.r,color.g,color.b,alpha);
	}

	//RaycastHit2D[]
	public static Collider2D[] getColliders(this RaycastHit2D[] rays){
		Collider2D[] cols = new Collider2D[rays.Length];
		for(int i=0;i<rays.Length;i++){
			cols[i] = rays[i].collider;
		}
		return cols;
	}

	/*public static void spawnMissile(this BulletWeapon bw, GameObject proj, Vector3 pos, NetworkViewID viewID, Vector3 ang){
		GameObject newMissile = (GameObject)Network.Instantiate(missile,transform.parent.position + offset,transform.parent.rotation,0);
		newMissile.GetComponent<Missile>().setOwner(viewID);
	}*/

}
