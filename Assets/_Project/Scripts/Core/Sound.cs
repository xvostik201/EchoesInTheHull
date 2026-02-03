using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour
{
    private void OnEnable()
    {
        Enemy enemy = FindObjectOfType<Enemy>();
        enemy.GetNewSound(transform.position);
    }
}
