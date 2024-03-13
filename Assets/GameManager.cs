using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int targetFrameRate = 60;
    void Awake()
    {
        Application.targetFrameRate = targetFrameRate;
    }

    void Update()
    {
        
    }
}
