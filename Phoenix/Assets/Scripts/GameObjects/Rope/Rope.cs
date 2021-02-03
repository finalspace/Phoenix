using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct RopeSegmentData
{
    public Vector2 posNow;
    public Vector2 posOld;
    public Vector2 velocity;

    public RopeSegmentData(Vector2 pos)
    {
        this.posNow = pos;
        this.posOld = pos;
        this.velocity = Vector3.zero;
    }
}

/// <summary>
/// rope simulation(data)
/// no dependencies
/// </summary>
public class Rope : MonoBehaviour
{
    public Transform StartPoint;
    public Vector2 forceGravity = new Vector2(0f, -0.5f);
    public List<RopeSegmentData> ropeSegments = new List<RopeSegmentData>();

    [Header("Debug")]
    public bool debugDraw;

    private LineRenderer lineRenderer;
    private float ropeSegLen = 0.4f;
    private int segmentLength = 10;
    private float lineWidth = 0.1f;

    // Use this for initialization
    void Awake()
    {
        this.lineRenderer = this.GetComponent<LineRenderer>();
        Vector3 ropeStartPoint = StartPoint.position;

        for (int i = 0; i < segmentLength; i++)
        {
            this.ropeSegments.Add(new RopeSegmentData(ropeStartPoint));
            ropeStartPoint.y -= ropeSegLen;
        }
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.GetMouseButtonDown(0))
            Drag(segmentLength - 1, new Vector2(5f, -5f));
        */
        this.DrawRope();
    }

    private void FixedUpdate()
    {
        //apply forces to rope first then simulate. set code execute order
        this.Simulate();
    }

    private void Simulate()
    {
        for (int i = 1; i < this.segmentLength; i++)
        {
            RopeSegmentData firstSegment = this.ropeSegments[i];
            Vector2 velocity = (firstSegment.posNow - firstSegment.posOld) / Time.fixedDeltaTime;
            velocity += firstSegment.velocity;
            firstSegment.velocity *= 0.9f;
            firstSegment.posOld = firstSegment.posNow;
            firstSegment.posNow += velocity * Time.fixedDeltaTime;
            firstSegment.posNow += forceGravity * Time.fixedDeltaTime;
            this.ropeSegments[i] = firstSegment;
        }

        //CONSTRAINTS
        for (int i = 0; i < 50; i++)
        {
            this.ApplyConstraint();
        }
    }

    private void ApplyConstraint()
    {
        //Constrant to Mouse
        RopeSegmentData firstSegment = this.ropeSegments[0];
        firstSegment.posNow = this.StartPoint.position;
        this.ropeSegments[0] = firstSegment;

        /*
        //Constrant to Second Point
        RopeSegment endSegment = this.ropeSegments[this.ropeSegments.Count - 1];
        //endSegment.posNow = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        endSegment.posNow = pos;
        this.ropeSegments[this.ropeSegments.Count - 1] = endSegment;
        */

        for (int i = 0; i < this.segmentLength - 1; i++)
        {
            RopeSegmentData firstSeg = this.ropeSegments[i];
            RopeSegmentData secondSeg = this.ropeSegments[i + 1];

            float dist = (firstSeg.posNow - secondSeg.posNow).magnitude;
            float error = Mathf.Abs(dist - this.ropeSegLen);
            Vector2 changeDir = Vector2.zero;

            if (dist > ropeSegLen)
            {
                changeDir = (firstSeg.posNow - secondSeg.posNow).normalized;
            }
            else if (dist < ropeSegLen)
            {
                changeDir = (secondSeg.posNow - firstSeg.posNow).normalized;
            }

            Vector2 changeAmount = changeDir * error;
            if (i != 0)
            {
                firstSeg.posNow -= changeAmount * 0.5f;
                this.ropeSegments[i] = firstSeg;
                secondSeg.posNow += changeAmount * 0.5f;
                this.ropeSegments[i + 1] = secondSeg;
            }
            else
            {
                secondSeg.posNow += changeAmount;
                this.ropeSegments[i + 1] = secondSeg;
            }
        }
    }

    private void DrawRope()
    {
        float lineWidth = this.lineWidth;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        Vector3[] ropePositions = new Vector3[this.segmentLength];
        for (int i = 0; i < this.segmentLength; i++)
        {
            ropePositions[i] = this.ropeSegments[i].posNow;
        }

        lineRenderer.positionCount = ropePositions.Length;
        lineRenderer.SetPositions(ropePositions);
    }

    void OnDrawGizmos()
    {
        if (!debugDraw) return;

        for (int i = 0; i < this.ropeSegments.Count; i++)
        {
            Gizmos.DrawSphere(this.ropeSegments[i].posNow, 0.1f);
        }
    }

    private void AddPulse(int idx, Vector2 force)
    {
        RopeSegmentData segment = ropeSegments[idx];
        segment.velocity += force;
        ropeSegments[idx] = segment;
    }

    public void Drag(int idx, Vector2 force)
    {
        RopeSegmentData segment = ropeSegments[idx];
        //todo: add max limit
        segment.velocity += force;
        ropeSegments[ropeSegments.Count - 1] = segment;

        /*
        RopeSegment segment = ropeSegments[ropeSegments.Count - 1];
        segment.posNow += new Vector2(0.02f, 0) * direction;
        ropeSegments[ropeSegments.Count - 1] = segment;
        */

        /*
        RopeSegment segment = ropeSegments[14];
        segment.posNow.x += 0.02f * direction;
        ropeSegments[14] = segment;

        segment = this.ropeSegments[8];
        segment.posNow.x += 0.01f;
        ropeSegments[8] = segment;
        */
    }
}
