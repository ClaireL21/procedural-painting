using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turntable : MonoBehaviour
{

    public float rotationSpeed = 1.0f;
    public float maxTime = 2.0f;
    float currTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currTime += Time.deltaTime;
        if (currTime >= maxTime)
        {
            rotationSpeed *= -1.0f;
            currTime = 0.0f;
        }
        this.transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
}
