using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeControl : MonoBehaviour
{
    public Rope rope;
    public List<GameObject> segments;
    public SwingTrigger swingTrigger;

    public int total = 0;
    private bool occupied = false;

    private void Awake()
    {
        total = rope.ropeSegments.Count;
    }


    private void FixedUpdate()
    {
        UpdateView();
    }

    private void UpdateView()
    {
        for (int i = 0; i < total - 1; i++)
        {
            segments[i].transform.position = rope.ropeSegments[i].posNow;

            Vector3 dir = rope.ropeSegments[i + 1].posNow - rope.ropeSegments[i].posNow;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 90;
            segments[i].transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        segments[total - 1].transform.position = rope.ropeSegments[total - 1].posNow;
        segments[total - 1].transform.rotation = segments[total - 2].transform.rotation;
    }

    public void TryGrab(int idx, Vector3 vel)
    {
        if (occupied) return;

        occupied = true;
        Player.Instance.playerMovement.Land(segments[idx].transform);
        Player.Instance.playerMovement.StopSimulation();
        RopeCarrier ropeCarrier = Player.Instance.gameObject.AddComponent<RopeCarrier>();
        ropeCarrier.Grab(this, idx, vel);
    }

    public void Release()
    {
        Player.Instance.playerMovement.StartSimulation();
        StopSwing();
        Invoke(nameof(ReleaseOccupation), 1);
    }

    private void ReleaseOccupation()
    {
        occupied = false;
    }


    public void TryDrag()
    {

    }

    public void StartSwing(int dir = 1)
    {
        swingTrigger.EnableTrigger(dir);
    }

    public void StopSwing()
    {
        swingTrigger.DisableTrigger();
    }
}
