using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Material lineMaterial;
    [SerializeField] private BoardManager boardManager;
    [SerializeField] private LayerMask layerMask;
    [HideInInspector] public bool isFilling;
    private readonly List<BoardElement> _selectedElements = new();

    private readonly float _selectionRange = 1f;
    private bool _isDragging;
    private BoardElement _previousElement;
    private BoardElement _selectedElement;

    private void Update()
    {
        TouchController();
    }

    private void TouchController()
    {
        if (isFilling) return;

        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                _selectedElement = GetElementAtTouchPosition(touch.position);
                if (_selectedElement == null) return;

                SetLineRendererColor();
                _selectedElement.Select();
                _isDragging = true;
                lineRenderer.positionCount = 1;
                lineRenderer.SetPosition(0, _selectedElement.transform.position);
                _selectedElement.SetCollider(false);
                _selectedElements.Add(_selectedElement);
            }
            else if (_isDragging && touch.phase == TouchPhase.Moved)
            {
                var elementAtTouch = GetElementAtTouchPosition(touch.position);
                if (elementAtTouch == null) return;

                if (Vector2.Distance(mainCamera.ScreenToWorldPoint(touch.position), _selectedElements[_selectedElements.Count - 1].transform.position) <= _selectionRange)
                {
                    if (elementAtTouch != _previousElement && elementAtTouch.GetNumber() == _selectedElement.GetNumber())
                    {
                        lineRenderer.positionCount++;
                        lineRenderer.SetPosition(lineRenderer.positionCount - 1, elementAtTouch.transform.position);
                        _previousElement = elementAtTouch;
                        elementAtTouch.SetCollider(false);
                        elementAtTouch.Select();
                        _selectedElements.Add(elementAtTouch);
                    }
                }
            }
            else if (_isDragging && (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled))
            {
                _isDragging = false;
                EnableColliders();
                _selectedElement.Deselect(); // Deselect the first selected object
                DeselectAllElements();
                DestroyElements();
                lineRenderer.positionCount = 0;
            }
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
        var color = _selectedElement.GetColor(_selectedElement.GetNumber());
        lineRenderer.material.color = color;
    }

    private void EnableColliders()
    {
        foreach (var element in _selectedElements)
        {
            element.SetCollider(true);
        }

        // _selectedElements.Clear(); // Clear the list for the next use
    }

    private BoardElement GetElementAtTouchPosition(Vector2 touchPosition)
    {
        Vector2 worldPosition = mainCamera.ScreenToWorldPoint(touchPosition);
        var hit = Physics2D.Raycast(worldPosition, Vector2.zero, float.MaxValue, layerMask);
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

        var lastElement = GetElementAtPosition(lineRenderer.GetPosition(lineRenderer.positionCount - 1));
        var newNumber = CalculateThreshold(_selectedElements);

        // Destroy the elements at all positions except the last
        for (var i = 0; i < lineRenderer.positionCount - 1; i++) // -1 added to exclude the last position
        {
            var position = lineRenderer.GetPosition(i);
            var hit = Physics2D.Raycast(position, Vector2.zero, float.MaxValue, layerMask);
            if (hit.collider != null)
            {
                hit.collider.transform.DOMove(lastElement.transform.position, 0.20f).OnComplete(() =>
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

    private static int CalculateThreshold(List<BoardElement> selectedElements)
    {
        var total = 0;
        foreach (var element in selectedElements)
        {
            total += element.GetNumber();
        }

        int[] thresholds = { 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048 };
        var newNumber = thresholds[0];

        for (var i = 0; i < thresholds.Length; i++)
        {
            if (total >= thresholds[i])
            {
                newNumber = thresholds[i];
            }
            else
            {
                break;
            }
        }

        return newNumber;
    }

    private async void MoveDownAndFill()
    {
        isFilling = true;
        await Task.Delay(201); // Wait for before moving down the elements for animations
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
        var hit = Physics2D.Raycast(position, Vector2.zero, float.MaxValue, layerMask);
        if (hit.collider != null)
        {
            return hit.collider.TryGetComponent(out BoardElement element) ? element : null;
        }

        return null;
    }
}
