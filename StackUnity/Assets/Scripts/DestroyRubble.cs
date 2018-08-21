using UnityEngine;

public class DestroyRubble : MonoBehaviour
{

    private void OnCollisionEnter(Collision other)
    {
        Destroy(other.gameObject);
    }
}
