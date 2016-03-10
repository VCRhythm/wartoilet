using UnityEngine;
using System.Collections.Generic;

public class JoystickInput : MonoBehaviour, IInput {
    public int inputAddendum;
    public int InputCount { get { return 2; } }
    public InputType inputType { get { return InputType.Joystick; } }
    public bool IsTouchInput { get { return false; } }

    public float minX, maxX, minY, maxY;
    public Vector2 speed = new Vector2(1f, 1f);
    public bool hasCursors = false;
    public Transform cursorPrefab;

    Dictionary<int, string> stickMapping = new Dictionary<int, string> { { 0, "Left" }, { 1, "Right" } };

    Vector2[] previousPosition = new Vector2[2] { Vector2.zero, Vector2.zero };
    Transform[] cursors = new Transform[2];

    private bool[] triggerHeld = new bool[4] { false, false, false, false };

    void Start()
    {
        if (hasCursors)
        {
            for (int i = 0; i < InputCount; i++)
            {
                cursors[i] = Instantiate(cursorPrefab);
            }
        }
    }

    public void SetPosition(int inputIndex, Vector2 position)
    {
        previousPosition[inputIndex] = position;
    }

    public Vector2 GetPosition(int inputIndex)
    {
        Vector2 currentPosition = GetAxes(inputIndex) + previousPosition[inputIndex];
        currentPosition = new Vector2(Mathf.Clamp(currentPosition.x, minX, maxX), Mathf.Clamp(currentPosition.y, minY, maxY));

        SetPosition(inputIndex, currentPosition);
        return currentPosition;
    }

    public bool HasInputStarted(int inputIndex)
    {
        if (inputIndex < 2)
        {
            if (IsInputOn(inputIndex) && !triggerHeld[inputIndex])
            {
                triggerHeld[inputIndex] = true;
                return true;
            }

            return false;
        }
        else
        {
            return Input.GetButtonDown("Controller " + inputAddendum + " " + stickMapping[inputIndex - 2] + " Bumper");
        }
    }

    public bool IsInputOn(int inputIndex)
    {
        bool isOn = Input.GetAxisRaw("Controller " + inputAddendum + " " + stickMapping[inputIndex] + " Trigger") > 0;
        if (!isOn) triggerHeld[inputIndex] = false;

        return isOn;
    }

    public Vector2 GetAxes(int index)
    {
        return new Vector2(Input.GetAxis("Controller " + inputAddendum + " " + stickMapping[index] + " Stick X Axis") * speed.x, Input.GetAxis("Controller " + inputAddendum + " " + stickMapping[index] + " Stick Y Axis") * speed.y);
    }

    public Vector2 GetAxesRaw(int index)
    {
        return new Vector2(Input.GetAxisRaw("Controller " + inputAddendum + " " + stickMapping[index] + " Stick X Axis") * speed.x, Input.GetAxisRaw("Controller " + inputAddendum + " " + stickMapping[index] + " Stick Y Axis") * speed.y);
    }


    private void UpdateCursorPositions()
    {
        for (int i = 0; i < cursors.Length; i++)
        {
            cursors[i].position = GetPosition(i);
        }
    }
}
