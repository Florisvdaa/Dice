using System.Linq;
using UnityEngine;

public class DiceRoller : MonoBehaviour
{
    public void RollDice(Vector3 force, Vector3 torque)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.AddForce(force, ForceMode.Impulse);
        rb.AddTorque(torque, ForceMode.Impulse);
    }

    public int GetTopFace()
    {
        DiceFace[] faces = GetComponentsInChildren<DiceFace>();
        DiceFace top = faces.OrderByDescending(f => Vector3.Dot(f.transform.up, Vector3.up)).First();
        return top.GetFace();
    }
}
