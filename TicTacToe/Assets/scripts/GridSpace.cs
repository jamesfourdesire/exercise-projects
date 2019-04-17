using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridSpace : MonoBehaviour {
	
	public Button m_button;
	public Text m_text;
	string m_player;

	public void SetPlayer(string player) {
		m_player = player;
	}

	public void SetSpace() {
		m_text.text = m_player;
		m_button.interactable = false;
	}
	void Start () {
		m_text.text = "";
	}
	
}
