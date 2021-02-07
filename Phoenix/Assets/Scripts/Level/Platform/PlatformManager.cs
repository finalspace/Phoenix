using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : SingletonBehaviour<PlatformManager>
{
    public GameObject nextPlatformRowPrefab;
    public PlatformRow topRow;

    public Transform spawnTrigger;
    public float heightDiff = 1;
    
    //need to manually update this value
    public float widthDiff = 2;

    private int[] nextPlatformRowData;

    private void Start()
    {
        nextPlatformRowData = new int[11];
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            SpawnPlatformRow();
        }

        if (spawnTrigger.transform.position.y > topRow.transform.position.y + heightDiff)
        {
            SpawnPlatformRow();
        }
    }

    public void SpawnPlatformRow()
    {
        GenerateNextPlatformRow();
        GameObject platformRowObj = Instantiate(nextPlatformRowPrefab, Vector3.zero, Quaternion.identity);
        PlatformRow nextPlatformRow = platformRowObj.GetComponent<PlatformRow>();

        //calculate center position
        Transform closestPlatform1 = null;
        Transform closestPlatform2 = null;
        float dist1 = int.MaxValue;
        float dist2 = int.MaxValue;
        for (int i = 0; i < topRow.spots.Length; i++)
        {
            float dist = Mathf.Abs(spawnTrigger.position.x - topRow.spots[i].position.x);
            if (dist < dist1)
            {
                closestPlatform2 = closestPlatform1;
                dist2 = dist1;
                closestPlatform1 = topRow.spots[i];
                dist1 = dist;
            }
            else if (dist < dist2)
            {
                closestPlatform2 = topRow.spots[i];
                dist2 = dist;
            }
        }

        Vector2 spawnPos = new Vector2((closestPlatform1.position.x + closestPlatform2.position.x) / 2, topRow.transform.position.y + heightDiff);
        platformRowObj.transform.position = spawnPos;
        nextPlatformRow.Spawn(nextPlatformRowData);

        //update platform parent
        if (nextPlatformRow.transform.position.x > topRow.transform.position.x)
        {
            int diff = Mathf.RoundToInt((nextPlatformRow.transform.position.x - topRow.transform.position.x - widthDiff / 2) / widthDiff);
            if (diff < topRow.spots.Length)
            {
                PlatformBase topRowCurrent = null;
                PlatformBase nextRowLeft = null;
                PlatformBase nextRowRight = null;
                int idx0 = diff;
                int idx1 = 0;
                topRowCurrent = topRow.spots[idx0].childCount > 0 ? topRow.spots[idx0].GetChild(0).GetComponent<PlatformBase>() : null;
                nextRowRight = nextPlatformRow.spots[idx1].childCount > 0 ? nextPlatformRow.spots[idx1].GetChild(0).GetComponent<PlatformBase>() : null;
                if (topRowCurrent) topRowCurrent.rightParent = nextRowRight;
                if (nextRowRight) nextRowRight.leftChild = topRowCurrent;
                idx0++; idx1++;
                for (; idx0 < topRow.spots.Length; idx0++, idx1++)
                {
                    topRowCurrent = topRow.spots[idx0].childCount > 0 ? topRow.spots[idx0].GetChild(0).GetComponent<PlatformBase>() : null;
                    nextRowLeft = nextRowRight;
                    nextRowRight = nextPlatformRow.spots[idx1].childCount > 0 ? nextPlatformRow.spots[idx1].GetChild(0).GetComponent<PlatformBase>() : null;
                    if (topRowCurrent) topRowCurrent.leftParent = nextRowLeft;
                    if (topRowCurrent) topRowCurrent.rightParent = nextRowRight;
                    if (nextRowLeft) nextRowLeft.rightChild = topRowCurrent;
                    if (nextRowRight) nextRowRight.leftChild = topRowCurrent;
                }
            }
        }
        else
        {
            int diff = Mathf.RoundToInt((topRow.transform.position.x - nextPlatformRow.transform.position.x - widthDiff / 2) / widthDiff);
            if (diff < nextPlatformRow.spots.Length)
            {
                PlatformBase nextRowCurrent = null;
                PlatformBase topRowLeft = null;
                PlatformBase topRowRight = null;
                int idx0 = 0;
                int idx1 = diff;
                nextRowCurrent = nextPlatformRow.spots[idx1].childCount > 0 ? nextPlatformRow.spots[idx1].GetChild(0).GetComponent<PlatformBase>() : null;
                topRowRight = topRow.spots[idx0].childCount > 0 ? topRow.spots[idx0].GetChild(0).GetComponent<PlatformBase>() : null;
                if (nextRowCurrent) nextRowCurrent.rightChild = topRowRight;
                if (topRowRight) topRowRight.leftParent = nextRowCurrent;
                idx0++; idx1++;
                for (; idx1 < nextPlatformRow.spots.Length; idx0++, idx1++)
                {
                    nextRowCurrent = nextPlatformRow.spots[idx1].childCount > 0 ? nextPlatformRow.spots[idx1].GetChild(0).GetComponent<PlatformBase>() : null;
                    topRowLeft = topRowRight;
                    topRowRight = topRowRight = topRow.spots[idx0].childCount > 0 ? topRow.spots[idx0].GetChild(0).GetComponent<PlatformBase>() : null;
                    if (nextRowCurrent) nextRowCurrent.leftChild = topRowLeft;
                    if (nextRowCurrent) nextRowCurrent.rightChild = topRowRight;
                    if (topRowLeft) topRowLeft.rightParent = nextRowCurrent;
                    if (topRowRight) topRowRight.leftParent = nextRowCurrent;
                }
            }
        }



        //update manager
        topRow = nextPlatformRow;
    }


    /// <summary>
    /// strategy on what to generate next
    /// </summary>
    /// <returns></returns>
    public void GenerateNextPlatformRow()
    {
        //update platformRowPrefab;

        //update data
        for (int i = 0; i < nextPlatformRowData.Length; i++)
        {
            //test empty
            if (RandomGenerator.Test(0.2f))
            {
                nextPlatformRowData[i] = -1;
            }
            else if (RandomGenerator.Test(0.1f))
            {
                nextPlatformRowData[i] = 1;
            }
            else if (RandomGenerator.Test(0.1f))
            {
                nextPlatformRowData[i] = 2;
            }
            else
            {
                nextPlatformRowData[i] = 0;
            }
        }
    }
}
