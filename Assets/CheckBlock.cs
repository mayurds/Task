using System;
using System.Collections.Generic;
using UnityEngine;
	
	public class CheckBlock : MonoBehaviour
	{
		[SerializeField]
		private List<Tile> containedTiles = new List<Tile>();

		[SerializeField]
		private Tile attachedTile;

		[SerializeField]
		private Tile containedLockedTile;
		public Tile ContainedLockedTile { get { return containedLockedTile; } }

		private void Awake()
		{
			attachedTile = GetComponentInParent<Tile>();
		}

		private void FixedUpdate()
		{
			if (attachedTile.isSet)
			{
				containedLockedTile = null;
				return;
			}

			if (containedLockedTile != null && containedLockedTile.isSet)
			{
				containedLockedTile = null;
			}

			foreach (var tile in containedTiles)
			{
				if (!tile.isSet)
				{
					containedLockedTile = tile;
					break;
				}
			}
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			var tile = collision.GetComponent<Tile>();
			if (tile != null)
			{
				containedTiles.Add(tile);
			}
		}

		private void OnTriggerExit2D(Collider2D collision)
		{
			var tile = collision.GetComponent<Tile>();
			if (tile != null)
			{
				containedTiles.Remove(tile);
			}
		}
		public void ClearTileInformation()
		{
			containedLockedTile = null;
			containedTiles.Clear();
		}
	}

