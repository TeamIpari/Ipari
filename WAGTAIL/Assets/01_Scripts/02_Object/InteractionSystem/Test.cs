using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Transform startPos;
    public Transform endPos;
    public Transform heightPos;
    [Range(-1,3)]
    public float value;
    private void FixedUpdate()
    {
        math.lerp(0f, 1f,value);
        transform.position = BezierCurve(startPos.position, endPos.position, heightPos.position, value);
    }

    private Vector3 BezierCurve(Vector3 startPos, Vector3 endPos, Vector3 height, float value)
    {
        var a = Vector3.Lerp(startPos, height, value);

        var b = Vector3.Lerp(height, endPos, value);

        var c = Vector3.Lerp(a, b, value);
 
        return c;
    }
}
