using UnityEngine;
using System.Collections;

public class RotateNullright : MonoBehaviour
{
    public float turnSpeed = 50f;
    
    
    void Update ()
    {

            transform.Rotate(Vector3.right, turnSpeed * Time.deltaTime);
        
    }
}