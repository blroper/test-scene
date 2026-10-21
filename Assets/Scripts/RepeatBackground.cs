using UnityEngine;

public class RepeatingBackground : MonoBehaviour
{
    public Vector3 startPos;
    public float repeatWidth;
    public float backgroundWidth;
    public BoxCollider boxCollider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position;
        repeatWidth = boxCollider.size.x /2;
    }

    // Update is called once per frame
    void Update()
    {

        if (transform.position.x < startPos.x - repeatWidth)
        {
            Debug.Log("too far right");
            
            transform.position = startPos;
        }

        if (transform.position.x> startPos.x + repeatWidth)
        {
            Debug.Log("too far left");
        
            transform.position = new Vector3(
                startPos.x - repeatWidth,
                startPos.y,
                startPos.z 
            );  
        }
    }
}