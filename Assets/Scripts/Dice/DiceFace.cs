using UnityEngine;

public class DiceFace : MonoBehaviour
{
    [SerializeField] private int faceValue;

    public int GetFace() => faceValue;
}
