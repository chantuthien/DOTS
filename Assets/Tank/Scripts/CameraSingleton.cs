using UnityEngine;

public class CameraSingleton : MonoBehaviour
{
    public static CameraSingleton Ins;

    void Awake()
    {
        Ins = this;
    }
}
