using System.Timers;
using UnityEngine;

public class BufferedAction
{
    private float bufferTime;
    private float lastInputTime;
    private bool inputPressed;

    // How long the input can be held, even after it becomes invalid
    // A value of 0.0 will act as a single use action
    private float holdableFor;
    // The last time an action was successfully started
    private float lastActionStart;
    // Whether the action is currently being activated
    // Used so that you can't release and retrigger the action within this window
    private bool isBeingActivated;

    public BufferedAction(float bufferDuration, float holdDuration = 0.0f)
    {
        bufferTime = bufferDuration;
        lastInputTime = -bufferDuration; // Initialize to allow immediate input
        inputPressed = false;

        holdableFor = holdDuration;
        lastActionStart = -holdDuration;
    }

    private void ProcessInput(bool input)
    {
        if (!inputPressed && input)
        {
            lastInputTime = Time.time;
        }
        inputPressed = input;
    }

    // Should be called each physics update to update internal states
    public bool IsActing(bool input, bool allowed)
    {
        ProcessInput(input);

        // The initial input is valid if:
        // 1. The input is still being pressed
        // 2. The action activation is not currently in progress
        // 3. The input was just pressed within the buffer time
        bool initialInputValid = inputPressed && !isBeingActivated && (Time.time - lastInputTime) <= bufferTime;
        // The held input is valid if:
        // 1. The input is currently being pressed
        // 2. The action activation is ongoing, i.e. it was not interrupted
        // 3. The action has not been held for longer than the holdable duration
        bool heldInputValid = inputPressed && isBeingActivated && (Time.time - lastActionStart) <= holdableFor;

        if (initialInputValid && allowed)
        {
            lastActionStart = Time.time;
            isBeingActivated = true;
            return true;
        }
        else if (heldInputValid)
        {
            return true;
        }
        else
        {
            isBeingActivated = false;
            return false;
        }
    }
}
