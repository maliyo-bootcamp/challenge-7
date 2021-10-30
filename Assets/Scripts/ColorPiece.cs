using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ColorPiece : MonoBehaviour
{

    public enum ColorType
    {
        YELLOW, 
        PURPLE,
        RED,
        BLUE,
        GREEN,
        PINK,
        ANY, 
        COUNT
    }


    [System.Serializable]
    public struct ColorSprite
    {
        public ColorType color;
        public Sprite sprite;
    }


    public ColorType Color
    {
        get { return color; }
        set { SetColor(value); }
    }

    public int NumColors
    {
        get { return colorSprites.Length; }
    }

    public ColorSprite[] colorSprites;
    private ColorType color;
    public Dictionary<ColorType, Sprite> colorSpriteDict;
    private SpriteRenderer sprite;
   

    private void Awake()
    {
        sprite = transform.Find("piece").GetComponent<SpriteRenderer>();
        colorSpriteDict = new Dictionary<ColorType, Sprite>();

        for (int i = 0; i < colorSprites.Length; i++)
        {
            if (!colorSpriteDict.ContainsKey(colorSprites[i].color))
            {
                colorSpriteDict.Add(colorSprites[i].color, colorSprites[i].sprite);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetColor (ColorType newColor)
    {
        color = newColor;
        if (colorSpriteDict.ContainsKey(newColor))
        {
            sprite.sprite = colorSpriteDict[newColor];
        }
    }
}
