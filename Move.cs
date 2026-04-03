using UnityEngine;

public class Move : MonoBehaviour
{
    public float speed;

    public Vector3 Dir;

    private void Update()
    {
        transform.Translate(Dir * speed * Time.deltaTime);
    }
}