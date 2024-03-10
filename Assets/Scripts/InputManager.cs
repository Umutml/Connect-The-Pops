using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private BoardManager boardManager;
    private bool _isDragging;
    private LineRenderer _lineRenderer;
    private Camera _mainCamera;
    private BoardElement _previousElement;
    private BoardElement _secondElement;
    private BoardElement _selectedElement;


    private void Awake()
    {
        _mainCamera = Camera.main;
        _lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var hit = Physics2D.Raycast(_mainCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null)
            {
                var hitElement = hit.collider.GetComponent<BoardElement>();
                if (hitElement != null)
                {
                    _selectedElement = hitElement;
                    _isDragging = true;
                    _lineRenderer.positionCount = 1;
                    _lineRenderer.SetPosition(0, _selectedElement.transform.position);
                }
            }
        }

        if (_isDragging)
        {
            Vector2 mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            var hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider != null)
            {
                var hitElement = hit.collider.GetComponent<BoardElement>();
                if (hitElement != null && hitElement != _previousElement && hitElement.GetNumber() == _selectedElement.GetNumber())
                {
                    _lineRenderer.positionCount++;
                    _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, hitElement.transform.position);
                    _secondElement = _selectedElement;
                    _selectedElement = hitElement;
                }

                _previousElement = hitElement;
            }

            _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, mousePosition);
        }

        if (Input.GetMouseButtonUp(0) && _selectedElement != null)
        {
            if (_isDragging)
            {
                MergeIfSameNumber();
                _isDragging = false;
                _lineRenderer.positionCount = 0;
            }

            _selectedElement = null;
            _previousElement = null;
            _secondElement = null;
        }
    }

    private void MergeIfSameNumber()
    {
        // If the second element's number is the same as the selected element's number, merge them
        if (_secondElement != null && _secondElement.GetNumber() == _selectedElement.GetNumber())
        {
            _lineRenderer.SetPosition(1, _secondElement.transform.position);
            MergeElements(_selectedElement, _secondElement);
        }
    }

    private BoardElement GetElementAtPosition(Vector2 position)
    {
        var hit = Physics2D.Raycast(position, Vector2.zero);
        if (hit.collider != null)
        {
            return hit.collider.GetComponent<BoardElement>();
        }

        return null;
    }

    private void MergeElements(BoardElement element1, BoardElement element2)
    {
        // Merge the elements (this could be as simple as adding their numbers together, or you could implement more complex merging behavior)
        var newNumber = element1.GetNumber() + element2.GetNumber();
        element1.SetNumber(newNumber);

        // Destroy the second element
        Destroy(element2.gameObject);
    }
}
