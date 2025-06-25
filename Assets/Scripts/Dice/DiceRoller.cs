using System.Linq;
using UnityEngine;
using System.Collections;

public class DiceRoller : MonoBehaviour
{
    // Reference to the normal visual state of the dice (default appearance)
    [SerializeField] private GameObject diceVisual;

    // Reference to the highlighted or selected visual state (e.g. brighter material)
    [SerializeField] private GameObject diceSelectedVisual;

    /// <summary>
    /// Applies a physical roll to the dice by resetting its velocity and adding force and torque.
    /// </summary>
    public void RollDice(Vector3 force, Vector3 torque)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.AddForce(force, ForceMode.Impulse);
        rb.AddTorque(torque, ForceMode.Impulse);
    }

    /// <summary>
    /// Determines the face of the die that is most upward-facing after settling.
    /// Uses dot product to compare alignment with world up vector.
    /// </summary>
    public int GetTopFace()
    {
        DiceFace[] faces = GetComponentsInChildren<DiceFace>();
        DiceFace top = faces.OrderByDescending(f => Vector3.Dot(f.transform.up, Vector3.up)).First();
        return top.GetFace();
    }

    /// <summary>
    /// Checks if the dice has stopped moving and is now considered settled.
    /// </summary>
    public bool IsDiceSettled()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        return rb.linearVelocity.sqrMagnitude < 0.01f && rb.angularVelocity.sqrMagnitude < 0.01f;
    }

    /// <summary>
    /// Initiates a flashing effect by toggling between normal and selected visual states.
    /// </summary>
    public void FlashDice(float duration = 1f, int flashCount = 4)
    {
        StartCoroutine(FlashVisualsCoroutine(duration, flashCount));
    }

    /// <summary>
    /// Coroutine that alternates between dice visuals to create a flash effect.
    /// Useful for highlighting a dice result in the scene.
    /// </summary>
    private IEnumerator FlashVisualsCoroutine(float duration, int flashCount)
    {
        if (diceVisual == null || diceSelectedVisual == null) yield break;

        float interval = duration / (flashCount * 2); // Half-duration on/off cycles

        for (int i = 0; i < flashCount; i++)
        {
            diceVisual.SetActive(false);
            diceSelectedVisual.SetActive(true);
            yield return new WaitForSeconds(interval);

            diceVisual.SetActive(true);
            diceSelectedVisual.SetActive(false);
            yield return new WaitForSeconds(interval);
        }
    }
}