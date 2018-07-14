﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace _20180713._Scripts
{
	public class MountShip : MonoBehaviour {

		private bool canMount;
		private bool mounting;

		private PlayerMovement playerMovement;
		private MeshRenderer playerMesh;
		private Collider playerCollider;
		private Base baseBlock;

		private ShipMovement playerShip;

		private void Awake()
		{
			playerMovement = GetComponent<PlayerMovement>();
			playerMesh = GetComponentInChildren<MeshRenderer>();
			playerCollider = GetComponent<Collider>();
			baseBlock = GetComponent<ShipOwner>().OwnBase;
			playerShip = baseBlock.gameObject.GetComponentInChildren<ShipMovement>();

			SetShipInputs();
		}

		private void SetShipInputs()
		{
			playerShip.HorizontalInput = playerMovement.HorizontalInput;
			playerShip.VerticalInput = playerMovement.VerticalInput;
			playerShip.InteractInput = playerMovement.InteractInput;
			playerShip.SecondaryInput = playerMovement.SecondaryInput;
		}

		private void Update()
		{
			if (!mounting)
			{
				TryMounting();
			}

			else if (mounting)
			{
				tryDismounting();
			}
		}


		private void TryMounting()
		{
			if(canMount && Input.GetButtonDown(playerMovement.SecondaryInput))
			{
				HidePlayer();
				GainShipControl();
				mounting = true;
			}
		}

		private void tryDismounting()
		{
			if (Input.GetButtonDown(playerMovement.SecondaryInput))
			{
				ShowPlayer();
				LoseShipControl();
				TeleportToShipPosition();
				mounting = false;
				canMount = false;
			}
		}

		#region Dismounted

		private void HidePlayer()
		{
			playerMesh.enabled = false;
			playerCollider.enabled = false;
		}

		private void GainShipControl()
		{
			playerShip.isMounted = true;
		}

		#endregion

		#region Mounted

		private void ShowPlayer()
		{
			playerMesh.enabled = true;
			playerCollider.enabled = true;
		}

		private void LoseShipControl()
		{
			playerShip.isMounted = false;
		}

		private void TeleportToShipPosition()
		{
			transform.position =
				new Vector3(baseBlock.transform.position.x, transform.position.y, baseBlock.transform.position.z);
		}

		#endregion

		#region Triggers

		private void OnTriggerStay(Collider other)
		{
			if (other.transform.parent == null || 
				other.transform.parent.GetComponent<Base>() == false)
			{
				Debug.Log("CANT MOUNT");
				canMount = false;
				return;
			}

			else if (other.transform.parent.GetComponent<Base>())
			{
				Debug.Log("NOW I CAN MOUNT");
				canMount = true;
			}
		}
		#endregion
	}
}