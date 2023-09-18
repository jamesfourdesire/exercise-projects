using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

	private static int[,] idxCheck = new int[8, 3] {
		{0, 1, 2},
		{3, 4, 5},
		{6, 7, 8},
		{0, 3, 6},
		{1, 4, 7},
		{2, 5, 8},
		{0, 4, 8},
		{2, 4, 6},
	};

	public GridSpace[] spaces;
	public GameObject message;
	string m_Player;

	public void checkEndGame()
	{
		m_Player = m_Player == "O" ? "X" : "O";
		assignPlayers();
		int winner = checkWin();
		if (winner > 0)
		{
			message.SetActive(true);
			message.GetComponentInChildren<Text>().text = (winner == 1 ? "O" : "X") + " wins!";
			foreach (GridSpace space in spaces)
			{
				space.SetDisabled();
			}
		}
	}

	void Start()
	{
		m_Player = "O";
		message.SetActive(false);
		foreach (GridSpace space in spaces)
		{
			space.SetGameController(this);
			space.SetPlayer(m_Player);
		}
	}

	void Update()
	{

	}

	private void assignPlayers()
	{
		foreach (GridSpace space in spaces)
		{
			space.SetPlayer(m_Player);
		}
	}

	private int checkWin()
	{
		int[] numbers = new int[9];
		for (int i = 0; i < spaces.Length; i++)
		{
			numbers[i] = spaces[i].GetPlayerNum();
		}
		for (int i = 0; i < idxCheck.Length / 3; i++)
		{
			if (numbers[idxCheck[i, 0]] == numbers[idxCheck[i, 1]]
			&& numbers[idxCheck[i, 1]] == numbers[idxCheck[i, 2]]
			&& numbers[idxCheck[i, 0]] > 0)
			{
				return numbers[idxCheck[i, 0]];
			}
		}
		return 0;
	}

}
