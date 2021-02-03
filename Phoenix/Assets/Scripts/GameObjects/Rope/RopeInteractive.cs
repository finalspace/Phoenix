using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeInteractive : MonoBehaviour
{
    public Transform StartPoint;
    public Vector2 forceGravity = new Vector2(0f, -1.5f);
    public float timeScale = 1;

    private LineRenderer lineRenderer;
    private List<RopeSegment> ropeSegments = new List<RopeSegment>();
    private float ropeSegLen = 0.25f;
    private int segmentLength = 15;
    private float lineWidth = 0.1f;

    private Vector2 pos;

    private float dragTimes = 0;

    // Use this for initialization
    void Start()
    {
        this.lineRenderer = this.GetComponent<LineRenderer>();
        Vector3 ropeStartPoint = StartPoint.position;

        for (int i = 0; i < segmentLength; i++)
        {
            this.ropeSegments.Add(new RopeSegment(ropeStartPoint));
            ropeStartPoint.y -= ropeSegLen;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            AddPulse();
        this.DrawRope();
    }

    private void FixedUpdate()
    {
        Drag();
        this.Simulate();
    }

    private void Simulate()
    {
        for (int i = 1; i < this.segmentLength; i++)
        {
            RopeSegment firstSegment = this.ropeSegments[i];
            Vector2 velocity = firstSegment.posNow - firstSegment.posOld;
            firstSegment.posOld = firstSegment.posNow;
            firstSegment.posNow += velocity * Time.fixedDeltaTime * 50 * timeScale;
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
        RopeSegment firstSegment = this.ropeSegments[0];
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
            RopeSegment firstSeg = this.ropeSegments[i];
            RopeSegment secondSeg = this.ropeSegments[i + 1];

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

    public struct RopeSegment
    {
        public Vector2 posNow;
        public Vector2 posOld;

        public RopeSegment(Vector2 pos)
        {
            this.posNow = pos;
            this.posOld = pos;
        }
    }

    private void AddPulse()
    {
        dragTimes = 20;
    }

    public void Drag()
    {
        if (dragTimes <= 0) return;
        dragTimes--;

        RopeSegment segment = ropeSegments[10];
        segment.posNow.x += 0.03f;
        ropeSegments[10] = segment;

        
        segment = this.ropeSegments[9];
        segment.posNow.x += 0.01f;
        ropeSegments[9] = segment;

        segment = this.ropeSegments[11];
        segment.posNow.x += 0.01f;
        ropeSegments[11] = segment;
        
    }
}
