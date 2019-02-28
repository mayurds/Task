using System;
using System.Collections.Generic;
using UnityEngine;

	public class Tile : MonoBehaviour
	{
		[SerializeField]
		private GameObject destroyEffect;
		public float destroyTime = 1;

        public SpriteRenderer spriterenderer;
	
		public bool isSet = true;
		[SerializeField]
		private CheckBlock[] neighborColliders = new CheckBlock[4]; 
		public CheckBlock[] Neighbors { get { return neighborColliders; } }

		private Color tileColor = Color.blue;
		public Color TileColor { get { return tileColor; } protected set { tileColor = value; } }

		private float velocity;
		private float lastY;
		private float destroyTimer = 0;

		private void Awake()
		{
        TileColor = spriterenderer.color; 
		}
		private void OnEnable()
		{
			lastY = transform.position.y;
			destroyTimer = destroyTime;
		}
		private void FixedUpdate()
		{
			velocity = lastY - transform.position.y;
			if (velocity < 0.01)
			{
				isSet = false;
			}
			else
			{
				isSet = true;
			}
			lastY = transform.position.y;
			if (!isSet)
			{
				destroyTimer -= Time.fixedDeltaTime;
				var vertMatches = 1 + CheckMatchesInDirection(Direction.Up) + CheckMatchesInDirection(Direction.Down);
				var horizMatches = 1 + CheckMatchesInDirection(Direction.Right) + CheckMatchesInDirection(Direction.Left);

				if ((vertMatches > 2 || horizMatches > 2))
				{
					if (destroyTimer <= 0)
					{
						var tilesToDestroy = new Stack<Tile>();
						tilesToDestroy.Push(this);
						if (horizMatches > 2)
						{
							if (CheckMatchesInDirection(Direction.Right) > 0)
								neighborColliders[(int)Direction.Right].ContainedLockedTile?.checkanddestroy(Direction.Right, tilesToDestroy);
								
							if (CheckMatchesInDirection(Direction.Left) > 0)
								neighborColliders[(int)Direction.Left].ContainedLockedTile?.checkanddestroy(Direction.Left, tilesToDestroy);
						}

						if (vertMatches > 2)
						{
							if (CheckMatchesInDirection(Direction.Up) > 0)
								neighborColliders[(int)Direction.Up].ContainedLockedTile?.checkanddestroy(Direction.Up, tilesToDestroy);

							if (CheckMatchesInDirection(Direction.Down) > 0)
								neighborColliders[(int)Direction.Down].ContainedLockedTile?.checkanddestroy(Direction.Down, tilesToDestroy);
						}


						while (tilesToDestroy.Count > 0)
						{
							var tile = tilesToDestroy.Pop();
							Instantiate(destroyEffect).transform.position = tile.transform.position;
							tile.Destroy();
						}
					}
				}
				else
				{
					destroyTimer = destroyTime;
				}
			}
		}

		public int CheckMatchesInDirection(Direction direction, int currentMatches = 0)
		{
			if (neighborColliders[(int)direction].ContainedLockedTile == null)
				return currentMatches;

			if (this.TileColor ==  neighborColliders[(int)direction].ContainedLockedTile.TileColor)
			{
				currentMatches++;
				return neighborColliders[(int)direction].ContainedLockedTile.CheckMatchesInDirection(direction, currentMatches);
			}

			return currentMatches;
		}

		public void checkanddestroy(Direction direction, Stack<Tile> tiles)
		{
			Direction cross1 = Direction.Right;
			Direction cross2 = Direction.Left;

			if (direction == Direction.Right || direction == Direction.Left)
			{
				cross1 = Direction.Up;
				cross2 = Direction.Down;
			}

			int cross1Matches = 0;
			if (!tiles.Contains(neighborColliders[(int)cross1]?.ContainedLockedTile))
			{
				cross1Matches = CheckMatchesInDirection(cross1);
			}

			int cross2Matches = 0;
			if (!tiles.Contains(neighborColliders[(int)cross2]?.ContainedLockedTile))
			{
				cross2Matches = CheckMatchesInDirection(cross2);
			}

			if ((1 + cross1Matches + cross2Matches) >= 3)
			{
				if (cross1Matches > 0)
				{
					neighborColliders[(int)cross1].ContainedLockedTile?.checkanddestroy(cross1, tiles);
				}
				if (cross2Matches > 0)
				{
					neighborColliders[(int)cross2].ContainedLockedTile?.checkanddestroy(cross2, tiles);
				}
			}
			tiles.Push(this);
		}

		private void RemoveTileFromNeighbors(Tile tile)
		{
			foreach (var neighbor in neighborColliders)
			{
				if (neighbor.ContainedLockedTile == tile)
				{
					neighbor.ClearTileInformation();
				}
			}
		}
		public void ClearNeighbors()
		{
			foreach (var neighbor in neighborColliders)
			{
				if (neighbor.ContainedLockedTile != null)
				{
					neighbor.ContainedLockedTile.RemoveTileFromNeighbors(this);
				}
				neighbor.ClearTileInformation();
			}
		}


		public virtual void Destroy()
		{
        Destroy(this.gameObject);
		}

		public enum Direction
		{
			Up,
			Right,
			Down,
			Left
		}
	}
