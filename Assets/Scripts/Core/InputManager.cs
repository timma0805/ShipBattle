using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InputAction
{
    UP,
    DOWN,
    LEFT,
    RIGHT,
    ROTATE,
    ACTION,
    JUMP
}
public class InputManager 
{
    private List<Dictionary<InputAction, KeyCode>> playerInputList;

    public void Init()
    {
        playerInputList = new List<Dictionary<InputAction, KeyCode>>();

        Dictionary<InputAction, KeyCode> player1Input = new Dictionary<InputAction, KeyCode>();
        player1Input.Add(InputAction.UP, KeyCode.W);
        player1Input.Add(InputAction.DOWN, KeyCode.S);
        player1Input.Add(InputAction.LEFT, KeyCode.A);
        player1Input.Add(InputAction.RIGHT, KeyCode.D);
        player1Input.Add(InputAction.JUMP, KeyCode.Space);
        player1Input.Add(InputAction.ACTION, KeyCode.E);

        Dictionary<InputAction, KeyCode> player2Input = new Dictionary<InputAction, KeyCode>();
        player2Input.Add(InputAction.UP, KeyCode.UpArrow);
        player2Input.Add(InputAction.DOWN, KeyCode.DownArrow);
        player2Input.Add(InputAction.LEFT, KeyCode.LeftArrow);
        player2Input.Add(InputAction.RIGHT, KeyCode.RightArrow);
        player2Input.Add(InputAction.JUMP, KeyCode.RightControl);
        player2Input.Add(InputAction.ACTION, KeyCode.KeypadEnter);

        playerInputList.Add(player1Input);
        playerInputList.Add(player2Input);
    }

    public Dictionary<InputAction, KeyCode> GetInputMapping(int playerIndex)
    {
        return playerInputList[playerIndex];
    }


}
