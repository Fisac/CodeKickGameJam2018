﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _20180713._Scripts;

public class Explodable : MonoBehaviour
{
    public float ExplodeInSeconds = 10;
    public float Radius;
    public float Force;
    [SerializeField] private float time;

    void Update()
    {
        time += Time.deltaTime;
        if (time > ExplodeInSeconds)
        {
            var colldiers = Physics.OverlapSphere(transform.position, Radius);
            foreach (var collider in colldiers)
            {
                var block = collider.GetComponent<Block>();
                var holder = block.GetHolder();
                if (holder != null)
                {
                    var @base = holder.GetComponent<Base>();
                    if (@base != null)
                    {
                        @base.DetachBlock(block);
                    }
                }

                var body = collider.GetComponent<Rigidbody>();
                if (body != null)
                {
                    body.AddExplosionForce(Force, transform.position, Radius);
                }
            }

            Destroy(gameObject);
        }
    }
}