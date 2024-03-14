using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Random = System.Random;

public class BoardManager : MonoBehaviour
{
    private const float SpacingFactor = 0.85f;
    public static readonly List<int> ElementValues = new();
         
    [SerializeField] private PlayerController playerController;

    private readonly GameObject[,] _gameBoard = new GameObject[5, 5]; // board size (5x5)
    private readonly Random _random = new();
    private readonly List<int> _spawnableElementValues = new() { 2, 4, 8, 16, 32, 64 };

    private readonly Vector2 _startCoordinate = new(-1.7f, -1.7f); // Start coordinate of the board

    private void Start()
    {
        // Fill the ElementValues list with powers of 2 from 2^1 to 2^20 = 1.048.576
        for (int i = 1; i <= 20; i++)
        {
            ElementValues.Add((int)Mathf.Pow(2, i));
        }   
        
        // Loop through the 2D array
        for (var row = 0; row < _gameBoard.GetLength(0); row++)
        {
            for (var column = 0; column < _gameBoard.GetLength(1); column++)
            {
                // Instantiate a new GameObject from the boardElement prefab
                _gameBoard[row, column] = ObjectPool.Instance.Get();
                _gameBoard[row, column].GetComponent<BoardElement>().elementNumber = GetNumber();
                _gameBoard[row, column].SetActive(true);
                // Set the position of the GameObject using x and y coordinates
                _gameBoard[row, column].transform.position = new Vector2(_startCoordinate.x + row * SpacingFactor, _startCoordinate.y + column * SpacingFactor);
            }
        }
    }


    public async void MoveDownElements()
    {
        for (var row = 0; row < _gameBoard.GetLength(0); row++)
        {
            var emptyCellIndex = -1;

            for (var column = 0; column < _gameBoard.GetLength(1); column++)
            {
                if (_gameBoard[row, column] == null || !_gameBoard[row, column].activeSelf)
                {
                    if (emptyCellIndex == -1)
                    {
                        emptyCellIndex = column;
                    }
                }
                else if (emptyCellIndex != -1)
                {
                    _gameBoard[row, emptyCellIndex] = _gameBoard[row, column];
                    _gameBoard[row, column] = null;
                    var targetPosition = new Vector2(_startCoordinate.x + row * SpacingFactor, _startCoordinate.y + emptyCellIndex * SpacingFactor);
                    _gameBoard[row, emptyCellIndex].transform.DOMove(targetPosition, 0.25f).SetEase(Ease.OutBack);
                    emptyCellIndex++;
                }
            }
        }

        await Task.Delay(251); // Wait for 251 milliseconds for animations
        FillEmptyCells();
    }

    private void FillEmptyCells()
    {
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

        playerController.isFilling = false;
    }

    public void SetCellToNull(Vector2 position)
    {
        var x = Mathf.RoundToInt((position.x - _startCoordinate.x) / SpacingFactor);
        var y = Mathf.RoundToInt((position.y - _startCoordinate.y) / SpacingFactor);
        _gameBoard[x, y] = null;
    }

    private int GetNumber()
    {
        var index = _spawnableElementValues[_random.Next(_spawnableElementValues.Count)];
        return index;
    }
}
