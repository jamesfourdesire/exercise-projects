using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridSpace : MonoBehaviour
{

    public Button m_button;
    public Text m_text;
    GameController m_gameController;
    string m_player;
    bool isSet = false;

    public void SetGameController(GameController gameController)
    {
        m_gameController = gameController;
    }

    public int GetPlayerNum()
    {
        if (isSet)
        {
            return m_player == "O" ? 1 : 2;
        }
        return 0;
    }

    public void SetPlayer(string player)
    {
        if (isSet)
        {
            return;
        }
        m_player = player;
    }

    public void SetSpace()
    {
        if (isSet)
        {
            return;
        }
        isSet = true;
        m_text.text = m_player;
        m_button.interactable = false;
        m_gameController.checkEndGame();
    }

    public void SetDisabled()
    {
        m_button.interactable = false;
    }
	
    void Start()
    {
        m_text.text = "";
    }

}
