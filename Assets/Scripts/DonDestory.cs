using System;
using UnityEngine;

public class DonDestory : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
