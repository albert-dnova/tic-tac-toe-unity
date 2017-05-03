using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Board {
	public enum CurrentPlayer {
		PLAYER_CIRCLE,
		PLAYER_CROSS,
		PLAYER_INVALID
	}
}

public class BoardManager : MonoBehaviour {

	public GameObject GroundObject;
	public int BoardSize = 3;

	public float UnitsPerGroundObject = 3f;
	public float MarginBetweenGroundTiles = 0.2f;

	private Board.CurrentPlayer _currentTurn = Board.CurrentPlayer.PLAYER_CIRCLE;

	private struct BoardPosition {
		public Board.CurrentPlayer currentPlayer;
		public long gameObjectInstanceId;

		public BoardPosition(long instanceId) 
		{
			currentPlayer = Board.CurrentPlayer.PLAYER_INVALID;
			gameObjectInstanceId = instanceId;
		}

	};

	private List<List<BoardPosition>> _board;

	private Transform _boardHolder;
	
	// Use this for initialization
	void Start () {
		CreateBoard();
		NotifyCurrentTurn();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnItemSelected(int instanceId) 
	{
		_currentTurn = (int) Board.CurrentPlayer.PLAYER_CROSS - _currentTurn;
		NotifyCurrentTurn();
	}

	private void CreateBoard() 
	{
		_boardHolder = new GameObject("BoardHolder").transform;
		_boardHolder.SetParent(transform);

		var leftOffset = 0f;
		var topOffset = 0f;

		_board = new List<List<BoardPosition>>();
		for(var i = 0; i < BoardSize; ++i)
		{
			leftOffset = 0f;
			List<BoardPosition> rowItems = new List<BoardPosition>();
			for(var j = 0; j < BoardSize; ++j)
			{
				var groundTile = Instantiate(GroundObject, 
										 new Vector3(leftOffset, topOffset, 0f), 
										 Quaternion.identity, 
										 _boardHolder);				
				leftOffset += UnitsPerGroundObject + MarginBetweenGroundTiles;			
				BoardPosition boardPosition = new BoardPosition(groundTile.GetInstanceID());
				rowItems.Add(boardPosition);
			}

			topOffset -= UnitsPerGroundObject + MarginBetweenGroundTiles;

			_board.Add(rowItems);
			
		}

		_boardHolder.position = new Vector2(-(UnitsPerGroundObject + MarginBetweenGroundTiles), 
											(UnitsPerGroundObject + MarginBetweenGroundTiles));
	}

	private void NotifyCurrentTurn() 
	{
		_boardHolder.BroadcastMessage("SetCurrentPlayerTurn", _currentTurn);
	}
}
