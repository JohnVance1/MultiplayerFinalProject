using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    private GameObject lr;
    private LineRenderer renderer;
    private Transform[] points;

    private void Awake()
    {
        renderer = this.GetComponent<LineRenderer>();
    }

    public void SetUpLine(Transform[] points)
    {
        renderer.positionCount = points.Length;
        this.points = points;

    }

    private void Update()
    {
        for(int i = 0; i < points.Length; i++)
        {
            renderer.SetPosition(i, points[i].position);

        }
    }

}
