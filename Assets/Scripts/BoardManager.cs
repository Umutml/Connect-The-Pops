using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Random = System.Random;

public class BoardManager : MonoBehaviour
{
    private const float SpacingFactor = 0.85f;
    private static readonly List<int> SpawnableElementValues = new() { 2, 4, 8, 16, 32, 64 };
    private static readonly List<int> ElementValues = new() { 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048 };

    private readonly Vector2 _startCoordinate = new(-1.7f, -1.7f); // Start coordinate of the board

    private readonly GameObject[,] _gameBoard = new GameObject[5, 5]; // board size (5x5)
    private readonly Random _random = new();

    private void Start()
    {
        // Loop through the 2D array
        for (var i = 0; i < _gameBoard.GetLength(0); i++)
        {
            for (var j = 0; j < _gameBoard.GetLength(1); j++)
            {
                // Instantiate a new GameObject from the boardElement prefab
                _gameBoard[i, j] = ObjectPool.Instance.Get();
                _gameBoard[i, j].GetComponent<BoardElement>().elementNumber = GetNumber();
                _gameBoard[i, j].SetActive(true);
                // Set the position of the GameObject using x and y coordinates
                _gameBoard[i, j].transform.position = new Vector2(_startCoordinate.x + i * SpacingFactor, _startCoordinate.y + j * SpacingFactor);
            }
        }
    }
    
    
    public void MoveDownElements()
    {
        var moved = true;
        while (moved)
        {
            moved = false;
            for (var i = 0; i < _gameBoard.GetLength(0); i++)
            {
                for (var j = 0; j < _gameBoard.GetLength(1); j++)
                {
                    if (_gameBoard[i, j] != null && _gameBoard[i, j].activeSelf) continue; // Check if the cell is null or inactive and continue
                    
                    // move all the elements above the empty cell down
                    for (var k = j; k < _gameBoard.GetLength(1) - 1; k++)
                    {
                        if (_gameBoard[i, k + 1] == null) continue; // Check if the cell is null
                            
                        _gameBoard[i, k] = _gameBoard[i, k + 1];
                        _gameBoard[i, k + 1] = null;
                        var targetPosition = new Vector2(_startCoordinate.x + i * SpacingFactor, _startCoordinate.y + k * SpacingFactor);
                        _gameBoard[i, k].transform.DOMove(targetPosition, 0.25f).SetEase(Ease.InOutQuad); // Use DOMove to animate the movement
                        moved = true;
                    }
                }
            }
        }
    }
    
    public async void FillEmptyCells()
    {
        await Task.Delay(300);
        for (var i = 0; i < _gameBoard.GetLength(0); i++)
        {
            for (var j = 0; j < _gameBoard.GetLength(1); j++)
            {
                // If the cell is null, create a new element
                if (_gameBoard[i, j] == null)
                {
                    _gameBoard[i, j] = ObjectPool.Instance.Get();
                    _gameBoard[i, j].GetComponent<BoardElement>().elementNumber = GetNumber();
                    _gameBoard[i, j].SetActive(true);
                    _gameBoard[i, j].transform.position = new Vector2(_startCoordinate.x + i * SpacingFactor, _startCoordinate.y + j * SpacingFactor);
                }
            }
        }
    }
    
    public void SetCellToNull(Vector2 position)
    {
        var x = Mathf.RoundToInt((position.x - _startCoordinate.x) / SpacingFactor);
        var y = Mathf.RoundToInt((position.y - _startCoordinate.y) / SpacingFactor);
        _gameBoard[x, y] = null;
    }

    private int GetNumber()
    {
        var index = SpawnableElementValues[_random.Next(SpawnableElementValues.Count)];
        return index;
    }
}
