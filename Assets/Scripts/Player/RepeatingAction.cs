using UnityEngine;

public class RepeatingAction
{
    private float repeatInterval;
    private float holdableFor;

    private float timeSinceLastRepeat;

    // This boolean prevents multiple triggers within the holdable duration.
    // Held presses will only count if they have been continually in progress.
    private bool inProgress = false;

    public RepeatingAction(float repeatDuration, float holdDuration = 0.0f)
    {
        repeatInterval = repeatDuration;
        holdableFor = holdDuration;

        timeSinceLastRepeat = repeatInterval; // Initialize to allow immediate action
    }

    public bool IsActing(bool input, bool allowed, float deltaTime)
    {
        timeSinceLastRepeat += deltaTime;

        if (input && allowed)
        {
            // The action is allowed and the input is being held, and we need to trigger a new repeat.
            if (timeSinceLastRepeat >= repeatInterval)
            {
                timeSinceLastRepeat = 0.0f;
                inProgress = true;
                return true;
            }
            // The action is still in progress and being held within the holdable duration.
            else if (inProgress && timeSinceLastRepeat <= holdableFor)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            inProgress = false;
            return false;
        }
    }
}
