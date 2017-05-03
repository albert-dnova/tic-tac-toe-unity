using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTileComponent : MonoBehaviour {

	public Sprite GroundSprite;
	public Sprite CrossSprite;
	public Sprite CircleSprite;
	private bool _isDisplayingOver = false;

	private Board.CurrentPlayer _currentPlayerTurn;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		var isOver = IsInsideObject(mousePosition);
		DisplayActiveTileBackground(isOver);
		
		if(isOver && Input.GetMouseButtonDown(0))
		{
			//BoardManager boardManager = GameObject.Find("BoardManager").GetComponent<BoardManager>();
			BoardManager boardManager = GetComponentInParent<BoardManager>();
			boardManager.OnItemSelected(GetInstanceID());
		}
	}

	public void SetCurrentPlayerTurn(Board.CurrentPlayer currentPlayerTurn) 
	{
		_currentPlayerTurn = currentPlayerTurn;
	}

	private bool IsInsideObject(Vector3 mousePosition) 
	{
		SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
		Vector3 minBounds = spriteRenderer.bounds.min;
		Vector3 maxBounds = spriteRenderer.bounds.max;

		return IsBetween(minBounds.x, maxBounds.x, mousePosition.x) &&
		       IsBetween(minBounds.y, maxBounds.y, mousePosition.y);
	}

	private bool IsBetween(float firstNumber, float secondNumber, float value)
	{
		return (value >= firstNumber) && (value <= secondNumber);		
	} 

	private void DisplayActiveTileBackground(bool isMouseOver) 
	{		
		SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
		if(isMouseOver && !_isDisplayingOver)
		{
			_isDisplayingOver = true;
			if(_currentPlayerTurn == Board.CurrentPlayer.PLAYER_CIRCLE)
			{
				spriteRenderer.sprite = CircleSprite;
			}
			else 
			{
				spriteRenderer.sprite = CrossSprite;
			}
			
		}
		else if(!isMouseOver && _isDisplayingOver)
		{
			_isDisplayingOver = false;
			spriteRenderer.sprite = GroundSprite;
		}
	}
}
