using UnityEngine;
using System.Collections;

public class RotateNullup : MonoBehaviour
{
    public float turnSpeed = 50f;
    
    
    void Update ()
    {

            transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime);
        
    }
}