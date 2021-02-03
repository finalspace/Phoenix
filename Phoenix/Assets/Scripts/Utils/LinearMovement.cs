using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class LinearMovement : MonoBehaviour
{
    public Transform root;
    public Transform p1, p2;
    public float time;

    private float progress = 0;
    private Vector3 targetPosition;
    private void Awake()
    {
		if (root == null)
			root = transform;

		progress = 0;
        DOTween.To(() => progress, x => progress = x, 1, time).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
    }

    private void Update()
    {
        targetPosition = Vector3.Lerp(p1.position, p2.position, progress);
        root.transform.position = targetPosition;
    }


}
