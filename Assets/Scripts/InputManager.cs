using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Material lineMaterial;
    [SerializeField] private BoardManager boardManager;
    private readonly List<BoardElement> _selectedElements = new();

    private readonly float _selectionRange = 1f;
    private bool _isDragging;
    private BoardElement _previousElement;
    private BoardElement _selectedElement;
    public bool isFilling;

    private void Update()
    {
        BallController();
    }

    private void BallController()
    {
        if (isFilling) return;
        if (Input.GetMouseButtonDown(0))
        {
            _selectedElement = GetElementAtMousePosition();
            if (_selectedElement == null) return;
            SetLineRendererColor();
            _selectedElement.Select(); // Select the element scale it up
            _isDragging = true;
            lineRenderer.positionCount = 1;
            lineRenderer.SetPosition(0, _selectedElement.transform.position);
            _selectedElement.SetCollider(false); // Disable the collider of the selected element
            _selectedElements.Add(_selectedElement); // Add the first selected element to the list
        }
        else if (_isDragging && Input.GetMouseButton(0))
        {
            var elementAtMouse = GetElementAtMousePosition();
            if (elementAtMouse == null) return;

            // If the distance between the mouse position and the last selected element is less than the range
            if (Vector2.Distance(mainCamera.ScreenToWorldPoint(Input.mousePosition), _selectedElements[_selectedElements.Count - 1].transform.position) <= _selectionRange)
            {
                if (elementAtMouse != _previousElement && elementAtMouse.GetNumber() == _selectedElement.GetNumber())
                {
                    lineRenderer.positionCount++;
                    lineRenderer.SetPosition(lineRenderer.positionCount - 1, elementAtMouse.transform.position);
                    _previousElement = elementAtMouse;
                    elementAtMouse.SetCollider(false); // Disable the collider of the element at mouse position
                    elementAtMouse.Select(); // Select the element scale it up
                    _selectedElements.Add(elementAtMouse);
                }
            }
        }
        else if (_isDragging && Input.GetMouseButtonUp(0))
        {
            _isDragging = false;
            EnableColliders(); // Enable the colliders before destroying the elements
            DeselectAllElements(); // Deselect all the elements scale them down
            DestroyElements();
            lineRenderer.positionCount = 0;
        }
    }
    
    private void DeselectAllElements()
    {
        foreach (var element in _selectedElements)
        {
            element.Deselect();
        }
    }
    
    private void SetLineRendererColor()
    {
        if (!lineRenderer.material)
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        Color color = _selectedElement.GetColor(_selectedElement.GetNumber());
        lineRenderer.material.color = color;
    }

    private void EnableColliders()
    {
        foreach (var element in _selectedElements)
        {
            element.SetCollider(true);
        }

        _selectedElements.Clear(); // Clear the list for the next use
    }

    private BoardElement GetElementAtMousePosition()
    {
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        var hit = Physics2D.Raycast(mousePosition, Vector2.zero);
        if (hit.collider != null)
        {
            return hit.collider.GetComponent<BoardElement>();
        }

        return null;
    }

    private void DestroyElements()
    {
        if (lineRenderer.positionCount < 2)
        {
            DeselectAllElements();
            ResetTemporaryValues(); // Reset temporary values before returning
            return; // If there are less than 2 positions, we can't perform the operation
        }

        // Get the elements at the 0th and 1st positions
        var firstElement = GetElementAtPosition(lineRenderer.GetPosition(0));
        var secondElement = GetElementAtPosition(lineRenderer.GetPosition(1));
        var lastElement = GetElementAtPosition(lineRenderer.GetPosition(lineRenderer.positionCount - 1));
        var newNumber = 0;
        if (firstElement != null && secondElement != null && lastElement != null)
        {
            newNumber = firstElement.GetNumber() + secondElement.GetNumber();
        }

        // Destroy the elements at all positions except the last
        for (var i = 0; i < lineRenderer.positionCount - 1; i++) // -1 added to exclude the last position
        {
            var position = lineRenderer.GetPosition(i);
            var hit = Physics2D.Raycast(position, Vector2.zero);
            if (hit.collider != null)
            {
                hit.collider.transform.DOMove(lastElement.transform.position, 0.25f).OnComplete(() =>
                {
                    lastElement.SetNumber(newNumber);
                    lastElement.Deselect(); // Deselect the last element scale it down
                    boardManager.SetCellToNull(position); // Set the cell to null before returning the object to the pool
                    ObjectPool.Instance.Return(hit.collider.gameObject);
                });
            }
        }
        MoveDownAndFill();
        ResetTemporaryValues();
    }
    
    private async void MoveDownAndFill()
    {
        isFilling = true;
        await Task.Delay(251); // Wait for before moving down the elements for animations
        boardManager.MoveDownElements();
    }

    private void ResetTemporaryValues()
    {
        _selectedElement = null;
        _previousElement = null;
        _selectedElements.Clear();
    }

    private BoardElement GetElementAtPosition(Vector3 position)
    {
        var hit = Physics2D.Raycast(position, Vector2.zero);
        if (hit.collider != null)
        {
            return hit.collider.TryGetComponent(out BoardElement element) ? element : null;
        }

        return null;
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Vector3 mousePosition = Input.mousePosition;
        Rect screenRect = new Rect(0, 0, Screen.width, Screen.height);
        if (screenRect.Contains(mousePosition))
        {
            Gizmos.color = Color.blue;
            // Draw the selection range
            Gizmos.DrawWireSphere(mainCamera.ScreenToWorldPoint(mousePosition), _selectionRange);
        }
    }
#endif
}
