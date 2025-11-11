using UnityEngine;

public class OverlapCapsuleVisualizer : MonoBehaviour
{
    [Tooltip("Capsule Collider with dimentions to visualize.")]
    public CapsuleCollider2D Capsule;

    [Tooltip("The angle of the capsule")]
    public float angle;

    public bool ManualAngleInput = false;

    //[Tooltip("The radius of the capsule (half the smaller dimension of size)")]
    //public float radius;
    private Vector3 point1;
    private Vector3 point2;
    private float radius;

    void OnDrawGizmos()
    {
        Vector3 position = Capsule.transform.position + new Vector3(Capsule.offset.x, Capsule.offset.y);
        //Vector3 position = Capsule.transform.position;
        if (!ManualAngleInput) { angle = Capsule.transform.rotation.eulerAngles.z; }
        //Capsule.transform.rot
        if (Capsule.direction == CapsuleDirection2D.Vertical)
        {
            // Calculate the end points of the capsule
            //point1 = position + (Quaternion.Euler(0, 0, angle) * new Vector2(0, Capsule.size.y / 2));
            //point1 = position + new Vector3(Capsule.offset.x, Capsule.offset.y, 0);
            point1 = position + (Quaternion.Euler(0, 0, angle) * new Vector2(0, Capsule.size.y / 2));
            point2 = position + (Quaternion.Euler(0, 0, angle) * new Vector2(0, -Capsule.size.y / 2));
            radius = Capsule.size.x / 2;
            point1.y -= radius;
            point2.y += radius;
            //Capsule.
        }
        else
        {
            radius = Capsule.size.y / 2;

            //point1 = position + (Quaternion.Euler(0, 0, angle) * new Vector2((-Capsule.size.x / 2) + radius, 0));
            //point2 = position + (Quaternion.Euler(0, 0, angle) * new Vector2((Capsule.size.x / 2) - radius, 0));
            point1 = position + (Quaternion.Euler(0, 0, angle) * new Vector2(-Capsule.size.x / 2, 0));
            point2 = position + (Quaternion.Euler(0, 0, angle) * new Vector2(Capsule.size.x / 2, 0));

            point1.x += radius;
            point2.x -= radius;
        }


        // Draw the spheres at the end points
        Gizmos.color = Color.yellow; // You can change the color
        Gizmos.DrawWireSphere(point1, radius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(point2, radius);

        // Draw the line between the spheres
        Gizmos.color = Color.black;
        Gizmos.DrawLine(point1, point2);
    }
}
