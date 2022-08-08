using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSprite : MonoBehaviour
{
    public SpriteRenderer playerSpriteRenderer;
    public SpriteRenderer colliderSpriteRenderer;

    /// <summary>
    /// Color the player sprite, both the character and collider by default
    /// </summary>
    /// <param name="color">The RGBA color to use, transparent by default</param>
    /// <param name="spriteSelect">1: only the character; 2: only the collider; 0: both, default</param>
    public void ColorPlayerSprite(Color color = default, int spriteSelect = 0)
    {
        if(spriteSelect != 2)
            playerSpriteRenderer.color = color;
        if(spriteSelect != 1)
            colliderSpriteRenderer.color = color;
    }
}
