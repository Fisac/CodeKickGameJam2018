﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.AI;

namespace _20180713._Scripts
{
    public class Base : MonoBehaviour
    {
        [SerializeField] private Block pilotBlock;

        private readonly List<Block> baseBlocks = new List<Block>();

        [SerializeField] private const float snappingDistance = 1.5f;

        void Awake()
        {
            baseBlocks.Add(pilotBlock);
        }

        public void AttachBlock(Block block)
        {
            ConnectClosestBaseJointToClosestBlockJoint(block);
            block.SetHolder(gameObject);
        }

        public bool IsBlockCloseEnough(Block block)
        {
            return baseBlocks.Min(baseBlock =>
                       Vector3.Distance(block.transform.position, baseBlock.transform.position)) < snappingDistance;
        }

        public ClosestJointsPair GetClosestTwoJoints(Block block)
        {
            var blockJoints = block.GetFreeJoints();
            var baseJoints = baseBlocks.SelectMany(baseBlock => baseBlock.GetFreeJoints());
            BlockJoint closestBlockJoint = null;
            BlockJoint closestBaseJoint = null;
            float closestBaseJointDistance = -1;
            var blockJointsArray = blockJoints as BlockJoint[] ?? blockJoints.ToArray();
            foreach (var baseJoint in baseJoints)
            {
                foreach (var blockJoint in blockJointsArray)
                {
                    var distance = Vector3.Distance(blockJoint.GetEndPosition(),
                        baseJoint.GetEndPosition());
                    if (distance < closestBaseJointDistance || closestBaseJointDistance < 0)
                    {
                        closestBaseJoint = baseJoint;
                        closestBlockJoint = blockJoint;
                        closestBaseJointDistance = distance;
                    }
                }
            }

            return new ClosestJointsPair
            {
                BlockJoint = closestBlockJoint,
                BaseJoint = closestBaseJoint
            };
        }

        public IEnumerable<Block> GetBlocks()
        {
            return baseBlocks;
        }

        private void ConnectClosestBaseJointToClosestBlockJoint(Block block)
        {
            var joints = GetClosestTwoJoints(block);


            Align(block, joints);

            if (baseBlocks.Any(b => b.transform.position == block.transform.position))
            {
                throw new Exception("Block inside another block!");
            }

            baseBlocks.Add(block);
            joints.BaseJoint.Join(joints.BlockJoint);

            var jointsAtBlockPosition = GetFreeJointsAtPosition(block.transform.position);
            ConnectLooseJoints(block, jointsAtBlockPosition);
        }

        private static void Align(Block block, ClosestJointsPair joints)
        {
            var blockTransform = block.transform;
            var currentDir = blockTransform.position - joints.BlockJoint.GetEndPosition();
            var targetDir = joints.BaseJoint.GetEndPosition() - joints.BaseJoint.GetCenterPosition();
            blockTransform.rotation = Quaternion.FromToRotation(currentDir, targetDir) * blockTransform.rotation;
            block.transform.position += joints.BaseJoint.GetEndPosition() - joints.BlockJoint.GetEndPosition();
        }

        private IEnumerable<BlockJoint> GetFreeJointsAtPosition(Vector3 position)
        {
            return baseBlocks.SelectMany(baseBlock => baseBlock.GetFreeJoints())
                .Where(joint => joint.GetEndPosition() == position);
        }

        private void ConnectLooseJoints(Block block, IEnumerable<BlockJoint> otherJoints)
        {
            var freeBlockJoints = block.GetFreeJoints();
            var blockJoints = freeBlockJoints as BlockJoint[] ?? freeBlockJoints.ToArray();
            foreach (var looseJoint in otherJoints)
            {
                var closestBlockJoint = blockJoints.First(blockJoint =>
                    blockJoint.GetEndPosition() == looseJoint.GetCenterPosition());
                closestBlockJoint.Join(looseJoint);
            }
        }
    }

    public class ClosestJointsPair
    {
        public BlockJoint BaseJoint;
        public BlockJoint BlockJoint;
    }
}