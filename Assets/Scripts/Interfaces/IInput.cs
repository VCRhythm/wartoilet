using UnityEngine;

public enum InputType
{
    Touch,
    Mouse,
    Joystick
}

public interface IInput {
    InputType inputType { get; }
    int InputCount { get; }
	bool HasInputStarted(int inputIndex);
	bool IsInputOn(int inputIndex);	
    void SetPosition(int inputIndex, Vector2 postion);
    Vector2 GetPosition(int inputIndex);
    Vector2 GetAxes(int inputIndex);
    Vector2 GetAxesRaw(int inputIndex);
}