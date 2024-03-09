using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class BoardManager : MonoBehaviour
{
    private const float SpacingFactor = 0.85f;
    private static readonly List<int> ElementValues = new() { 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048 };

    [SerializeField] private Vector2 startCoordinate; // Start coordinate of the board

    private readonly GameObject[,] _gameBoard = new GameObject[5, 5]; // board size (5x5)
    private readonly Random _random = new();

    private void Start()
    {
        // _startCoordinate = transform.position;

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
                _gameBoard[i, j].transform.position = new Vector2(startCoordinate.x + i * SpacingFactor, startCoordinate.y + j * SpacingFactor);
            }
        }
    }

    private int GetNumber()
    {
        var index = ElementValues[_random.Next(ElementValues.Count)];
        return index;
    }
}
