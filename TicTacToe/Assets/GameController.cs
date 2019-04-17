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
    string player;

    public void checkEndGame()
    {
        player = player == "O" ? "X" : "O";
        assignPlayers();
        int winner = checkWin();
        if (winner > 0)
        {
            Debug.Log((winner == 1 ? "O" : "X") + " wins!");
            foreach (GridSpace space in spaces)
            {
                space.SetDisabled();
            }
        }
    }

    void Start()
    {
        player = "O";
        foreach (GridSpace space in spaces)
        {
            space.SetGameController(this);
            space.SetPlayer(player);
        }
    }

    void Update()
    {

    }

    private void assignPlayers()
    {
        foreach (GridSpace space in spaces)
        {
            space.SetPlayer(player);
        }
    }

    private int checkWin()
    {
        int[] nums = new int[9];
        for (int i = 0; i < spaces.Length; i++)
        {
            nums[i] = spaces[i].GetPlayerNum();
        }
        for (int i = 0; i < idxCheck.Length / 3; i++)
        {
            if (nums[idxCheck[i, 0]] == nums[idxCheck[i, 1]]
            && nums[idxCheck[i, 1]] == nums[idxCheck[i, 2]]
            && nums[idxCheck[i, 0]] > 0)
            {
                return nums[idxCheck[i, 0]];
            }
        }
        return 0;
    }

}
