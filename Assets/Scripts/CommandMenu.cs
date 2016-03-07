using UnityEngine;
using System.Collections;

public class CommandMenu : MonoBehaviour {
	public GameObject returnIcon;
	public GameObject attackIcon;
	public GameObject defendIcon;

	GameObject currentOrder;

	SpriteRenderer sr;
	LineRenderer lr;
	void Awake(){
		sr = GetComponent<SpriteRenderer>();
		lr = GetComponent<LineRenderer>();
	}
	void Start () {
		sr.enabled = false;
		lr.enabled = false;
		StartCoroutine(run());
	}
	
	IEnumerator run(){
		while(true){
			while(!Input.GetKey(KeyCode.Mouse2)){
				yield return new WaitForFixedUpdate();
			}
			sr.enabled = true;
			lr.enabled = true;
			transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition).setZ(0);
			lr.SetPosition(0, transform.position);
			while(Input.GetKey(KeyCode.Mouse2)){
				lr.SetPosition(1, Camera.main.ScreenToWorldPoint(Input.mousePosition).setZ(0));
				yield return new WaitForFixedUpdate();
			}
			float returnDot = Vector3.Dot(-transform.up, (Camera.main.ScreenToWorldPoint(Input.mousePosition).setZ(0) - transform.position).normalized);
			float attackDot = Vector3.Dot((transform.right + transform.up).normalized, (Camera.main.ScreenToWorldPoint(Input.mousePosition).setZ(0) - transform.position).normalized);
			float defendDot = Vector3.Dot((-transform.right + transform.up).normalized, (Camera.main.ScreenToWorldPoint(Input.mousePosition).setZ(0)  - transform.position).normalized);

			Destroy(currentOrder);
			if(returnDot > attackDot && returnDot > defendDot){
				currentOrder = (GameObject)Instantiate(returnIcon, transform.position, transform.rotation);
				StartCoroutine(returner(currentOrder));
			}else if(attackDot > defendDot){
				currentOrder = (GameObject)Instantiate(attackIcon, transform.position, transform.rotation);
				StartCoroutine(attack(currentOrder));
			}else{
				currentOrder = (GameObject)Instantiate(defendIcon, transform.position, transform.rotation);
			}

			sr.enabled = false;
			lr.enabled = false;
		}
	}
	IEnumerator returner(GameObject order){
		SpriteRenderer orderSR = order.GetComponent<SpriteRenderer>();
		while(orderSR.color.a > 0){
			orderSR.color = orderSR.color.setAlpha(orderSR.color.a - 0.03f);
			yield return new WaitForFixedUpdate();
		}
		Destroy(order);
	}
	IEnumerator attack(GameObject order){
		Collider2D col = Physics2D.OverlapCircle(order.transform.position, 5f, 1 << 9);
		while(col != null){
			yield return new WaitForSeconds(0.5f);
			col = Physics2D.OverlapCircle(order.transform.position, 5f, 1 << 9);
		}

		SpriteRenderer orderSR = order.GetComponent<SpriteRenderer>();
		while(orderSR.color.a > 0){
			orderSR.color = orderSR.color.setAlpha(orderSR.color.a - 0.03f);
			yield return new WaitForFixedUpdate();
		}
		Destroy(order);
	}
	public GameObject getCurrentOrder(){
		return currentOrder;
	}
}
