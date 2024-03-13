using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int targetFrameRate = 60;

    private void Awake()
    {
        Application.targetFrameRate = targetFrameRate;
        QualitySettings.vSyncCount = 0;
    }
}
