using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BoardElement : MonoBehaviour
{
    [HideInInspector] public int elementNumber;
    [SerializeField] private TextMeshProUGUI numberText;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private readonly Dictionary<int, Color> _numberToColor = new()
    {
        { 2, new Color(0.91f, 0.4f, 0.56f) },
        { 4, new Color(0.88f, 0.35f, 0.23f) },
        { 8, new Color(1f, 0.73f, 0.19f) },
        { 16, new Color(0.9f, 0.51f, 0.23f) },
        { 32, new Color(0.45f, 0.8f, 0.32f) },
        { 64, new Color(0.31f, 0.82f, 0.65f) },
        { 128, new Color(0.41f, 0.74f, 0.91f) },
        { 256, new Color(0.42f, 0.57f, 0.86f) },
        { 512, new Color(0.82f, 0.57f, 0.81f) },
        { 1024, new Color(0.6f, 0.43f, 0.82f) },
        { 2048, new Color(0.91f, 0.4f, 0.56f) }
    };

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        spriteRenderer.color = GetColor(elementNumber);
        numberText.text = elementNumber.ToString();
    }

    private Color GetColor(int value)
    {
        if (_numberToColor.TryGetValue(value, out var color))
        {
            return color;
        }

        return Color.white; // Default color
    }
    
    public int GetNumber()
    {
        return elementNumber;
    }
    
    public void SetNumber(int value)
    {
        elementNumber = value;
        UpdateText();
    }
    
    public void SetPosition(Vector2 position)
    {
        transform.position = position;
    }
    
    private void UpdateText()
    {
        numberText.text = elementNumber.ToString();
    }
    
    
}
