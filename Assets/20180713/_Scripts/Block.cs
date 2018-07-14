﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using _20180713._Scripts;

public class Block : MonoBehaviour
{
    public float Weight = 10;
   
    private bool isFree = true;

    private List<BlockJoint> joints = new List<BlockJoint>();

    void Awake()
    {
        joints = new List<BlockJoint>(GetComponentsInChildren<BlockJoint>());
    }

    public bool IsFree()
    {
        return isFree;
    }

    public void SetHolder(GameObject holder)
    {
        transform.SetParent(holder.transform);
        isFree = false;
    }

    public void Release()
    {
        transform.SetParent(null);
        isFree = true;
    }

    public IEnumerable<BlockJoint> GetFreeJoints()
    {
        return joints.Where(joint => !joint.Connected);
    }

    private void OnTriggerStay(Collider collider)
    {
        var blockHolder = collider.GetComponent<BlockHolder>();
        if (isFree && blockHolder && blockHolder.IsTryingToPickUp())
        {
            blockHolder.SetHoldingBlock(this);
        }
    }
}