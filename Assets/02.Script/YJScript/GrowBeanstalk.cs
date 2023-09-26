using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowBeanstalk : MonoBehaviour
{
    public float maxSize;
    public float growRate;
    public float scale;

    void Update()
    {
        if (scale < maxSize)
        {
            this.transform.localScale = Vector3.one * scale;
            scale += growRate * Time.deltaTime;
        }
    }
}
