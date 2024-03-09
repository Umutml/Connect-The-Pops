using System;
using TMPro;
using UnityEngine;

public class BoardElement : MonoBehaviour
{
    [HideInInspector] public int elementNumber;
    [SerializeField] private TextMeshProUGUI numberText;
    [SerializeField] private SpriteRenderer spriteRenderer;

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
        return value switch
        {
            2 => Color.red,
            4 => Color.blue,
            8 => Color.green,
            16 => Color.yellow,
            32 => Color.cyan,
            64 => Color.magenta,
            128 => Color.grey,
            256 => Color.magenta,
            512 => Color.black,
            1024 => new Color(1, 0.5f, 0), // Orange
            2048 => new Color(0.5f, 0, 1), // Purple
            _ => Color.white
        };
    }
}
