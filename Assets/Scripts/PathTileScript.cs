using UnityEngine;
using System.Collections;

public class PathTileScript : MonoBehaviour
{

    private Color originalColor;

    public void lightUp(float r, float g, float b)
    {
        SpriteRenderer my_sprite =  GetComponent<SpriteRenderer>();

        float new_r = r + my_sprite.color.r;
        float new_g = g + my_sprite.color.g;
        float new_b = b + my_sprite.color.b;
        
        Color my_newColor = new Color(new_r,new_g,new_b);
        my_sprite.color = my_newColor;
        //StartCoroutine(ExampleCoroutine());
    }

    public void resetColor()
    {
        SpriteRenderer my_sprite =  GetComponent<SpriteRenderer>();
        my_sprite.color = originalColor;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    
    IEnumerator ExampleCoroutine()
    {
        //yield on a new YieldInstruction that waits for 1 seconds.
        yield return new WaitForSeconds(1);

        resetColor();

    }

    void Start()
    {
        SpriteRenderer parent_sprite = GetComponentsInParent<SpriteRenderer>()[1];
        
        SpriteRenderer my_sprite = GetComponent<SpriteRenderer>();
        my_sprite.color = parent_sprite.color;
        originalColor = my_sprite.color;
    }

    // Update is called once per frame
    void Update()
    {
        //resetColor();
        //StartCoroutine(ExampleCoroutine());

    }
}
