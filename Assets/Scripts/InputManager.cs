using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private LineRenderer _lineRenderer;
    private readonly List<BoardElement> _selectedElements = new();
    private bool _isDragging;
    private BoardElement _previousElement;
    private BoardElement _selectedElement;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _selectedElement = GetElementAtMousePosition();
            if (_selectedElement != null)
            {
                _isDragging = true;
                _lineRenderer.positionCount = 1;
                _lineRenderer.SetPosition(0, _selectedElement.transform.position);
                _selectedElement.SetCollider(false); // Disable the collider of the selected element
                _selectedElements.Add(_selectedElement); // Add the first selected element to the list
            }
        }
        else if (_isDragging && Input.GetMouseButton(0))
        {
            var elementAtMouse = GetElementAtMousePosition();
            if (elementAtMouse != null)
            {
                if (elementAtMouse == _previousElement)
                {
                    // If the mouse has moved back to the previous element, cancel the last step
                    _lineRenderer.positionCount--;
                    _previousElement = GetElementAtPosition(_lineRenderer.GetPosition(_lineRenderer.positionCount - 1));
                }
                else if (elementAtMouse != _previousElement && elementAtMouse.GetNumber() == _selectedElement.GetNumber())
                {
                    _lineRenderer.positionCount++;
                    _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, elementAtMouse.transform.position);
                    _previousElement = elementAtMouse;
                    elementAtMouse.SetCollider(false); // Disable the collider of the element at mouse position
                    _selectedElements.Add(elementAtMouse);
                }
            }
        }
        else if (_isDragging && Input.GetMouseButtonUp(0))
        {
            _isDragging = false;
            EnableColliders(); // Enable the colliders before destroying the elements
            DestroyElements();
            _lineRenderer.positionCount = 0;
        }
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
        Vector2 mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        var hit = Physics2D.Raycast(mousePosition, Vector2.zero);
        if (hit.collider != null)
        {
            return hit.collider.GetComponent<BoardElement>();
        }

        return null;
    }

    private void DestroyElements()
    {
        if (_lineRenderer.positionCount < 2)
        {
            return; // If there are less than 2 positions, we can't perform the operation
        }

        // Get the elements at the 0th and 1st positions
        var firstElement = GetElementAtPosition(_lineRenderer.GetPosition(0));
        var secondElement = GetElementAtPosition(_lineRenderer.GetPosition(1));

        if (firstElement != null && secondElement != null)
        {
            // Add the numbers of the first and second elements
            var newNumber = firstElement.GetNumber() + secondElement.GetNumber();

            // Get the element at the last position
            var lastElement = GetElementAtPosition(_lineRenderer.GetPosition(_lineRenderer.positionCount - 1));

            if (lastElement != null)
            {
                // Set the number of the last element to the new number
                lastElement.SetNumber(newNumber);
            }
        }

        // Destroy the elements at all positions except the last
        for (var i = 0; i < _lineRenderer.positionCount - 1; i++) // -1 added to exclude the last position
        {
            var position = _lineRenderer.GetPosition(i);
            var hit = Physics2D.Raycast(position, Vector2.zero);
            if (hit.collider != null)
            {
                Destroy(hit.collider.gameObject);
            }
        }
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
}
