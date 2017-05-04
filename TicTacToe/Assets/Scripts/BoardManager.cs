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
		UpdateBoard(instanceId);
		if(HasWinner())
		{
			Debug.Log("Match 3 found!");
		}
		else
		{
			_currentTurn = (int) Board.CurrentPlayer.PLAYER_CROSS - _currentTurn;
			NotifyCurrentTurn();
		}		
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

	private void UpdateBoard(int instanceId) 
	{
		for(var i = 0; i < BoardSize; ++i)
		{
			for(var j = 0; j < BoardSize; ++j)
			{
				BoardPosition boardPosition = _board[i][j];

				if(boardPosition.gameObjectInstanceId == instanceId)
				{
					if(boardPosition.currentPlayer != Board.CurrentPlayer.PLAYER_INVALID)
					{
						Debug.Log("This position was already filled, overwritting!");
					}

					boardPosition.currentPlayer = _currentTurn;
					_board[i][j] = boardPosition;
					break;
				}
			}
		}
	}

	private bool HasWinner() 
	{		
		bool hasRightDiagonalMatch = HasMatch(0, 0, 1, 1);
		bool hasLeftDiagonalMatch = HasMatch(BoardSize - 1, 0, -1, 1);

		if(hasLeftDiagonalMatch || hasRightDiagonalMatch) 
		{
			return true;
		}

		for(var i = 0; i < BoardSize; ++i)
		{
			bool hasColumnMatch = HasMatch(0, i, 1, 0);
			bool hasRowMatch = HasMatch(i, 0, 0, 1); 

			if(hasColumnMatch || hasRowMatch)
			{
				return true;
			}
		}	

		return false;
	}

	private bool HasMatch(int initialRow, int initialColumn, int rowIncrement, int columnIncrement) 
	{
		Board.CurrentPlayer expectedMatch = _board[initialRow][initialColumn].currentPlayer;

		if(expectedMatch != Board.CurrentPlayer.PLAYER_INVALID)
		{
			int itemsFound = 1;
			int currentColumn = initialColumn + columnIncrement;
			int currentRow = initialRow + rowIncrement;

			while(IsValidPosition(currentColumn) && IsValidPosition(currentRow))
			{	
				if(_board[currentRow][currentColumn].currentPlayer != expectedMatch)
				{
					return false;
				}

				currentColumn += columnIncrement;
				currentRow += rowIncrement;
				++itemsFound;
			}

			if(itemsFound >= BoardSize)
			{
				return true;
			}			
		}

		return false;
	}

	private bool IsValidPosition(int value)
	{
		return value >= 0 && value < BoardSize;
	}
	
}
