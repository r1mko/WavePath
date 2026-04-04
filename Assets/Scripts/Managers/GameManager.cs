using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public CameraFollow Camera;
    public PlayerController Player;

    private void Awake()
    {
        FindRefs();
        Instance = this;
    }

    private void FindRefs()
    {
        if (Camera == null)
        {
            var camera = FindFirstObjectByType<CameraFollow>();
            if (camera != null)
                Camera = camera;
            else
                Debug.LogError("Camera didn't find");
        }

        if (Player == null)
        {
            var player = FindFirstObjectByType<PlayerController>();
            if (player != null)
                Player = player;
            else
                Debug.LogError("Player didn't find");
        }
    }
}
