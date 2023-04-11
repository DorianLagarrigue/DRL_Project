using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ML_Carrot : MonoBehaviour
{
    public float reward;
    [SerializeField] BoxCollider2D boxCollider;
    [SerializeField] SpriteRenderer spriteRenderer;

    public void InitCarrot()
    {
        boxCollider.enabled = true;
        spriteRenderer.enabled = true;
    }

    public void OnGetCarrot()
    {
        boxCollider.enabled = false;
        spriteRenderer.enabled = false;
    }
}
