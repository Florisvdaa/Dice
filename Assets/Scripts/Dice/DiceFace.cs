using UnityEngine;

public class DiceFace : MonoBehaviour
{
    [SerializeField] private int faceValue; // The number value this face represents (e.g. 1-6)

    // Returns the face value assigned to this side of the die
    public int GetFace() => faceValue;

}
