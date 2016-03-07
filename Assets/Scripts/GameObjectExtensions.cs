using UnityEngine;
using System.Collections;

public static class GameObjectExtensions {

	public static GameObject getNearestTo(this GameObject[] gameObjects, GameObject go){
		float distance = Mathf.Infinity;
		int index = 0;
		if(gameObjects.Length == 0) return null;
		for(int i=0;i<gameObjects.Length;i++){
			float newDistance = Vector3.Distance(go.transform.position, gameObjects[i].transform.position);
			if(newDistance < distance){
				index = i;
				distance = newDistance;
			}
		}
		return gameObjects[index];
	}
}
