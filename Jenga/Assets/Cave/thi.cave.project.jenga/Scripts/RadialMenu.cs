﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RadialMenu : MonoBehaviour {

	public RadialButton buttonPrefab;
	public RadialButton selected;
	public float radius;

	public void SpawnButtons (Interactable obj) {
		for (int i = 0; i< obj.options.Length; i++){
			RadialButton newButton = Instantiate(buttonPrefab) as RadialButton;
			newButton.transform.SetParent(transform, false);
			float theta = (2 * Mathf.PI / obj.options.Length) * i;
			float xPos = Mathf.Sin (theta);
			float yPos = Mathf.Cos (theta);
			newButton.transform.localPosition = new Vector3 (xPos, yPos, 0f) * radius;
			newButton.circle.color = obj.options [i].color;
			newButton.icon.sprite = obj.options [i].sprite;
			newButton.title.text = obj.options [i].title;
			newButton.myMenu = this;
		}
	}

	void Update(){
		if (Input.GetKeyUp(KeyCode.Escape)) {
			if (selected) {
				selected.ButtonPressed ();
			}
			Destroy (gameObject);
		}

		GameObject flystick = GameObject.Find ("FlystickSim");

		RaycastHit hit;

		if (Physics.Raycast (flystick.transform.position + flystick.transform.up, flystick.transform.up, out hit, 10)) {
			if (hit.collider.gameObject.name == "Button(Clone)") {
				GameObject hitObj = hit.collider.gameObject;
				RadialButton hitButton = hitObj.GetComponent<RadialButton> ();

				selected = hitButton;
			}
		}
		else {
			selected = null;
		}
	}
}