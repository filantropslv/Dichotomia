using UnityEngine;

public class AnyCollider : MonoBehaviour
{
    // on 3d OnCollisionEnter 
    void OnCollisionEnter2D(Collision2D collision)
    {

        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.collider.name.IndexOf("Enemy") > -1) {
                // Destroy(contact.collider); -- can not do directly
                Debug.Log(contact.collider.bounds.center.y);
            }
        }
    }
}
