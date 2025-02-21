using UnityEngine;
using System.Collections;

public class PathTileScript : MonoBehaviour
{

    private Color originalColor;

    public void lightUp(float r, float g, float b)
    {
        SpriteRenderer my_sprite =  GetComponent<SpriteRenderer>();
        Color my_newColor = new Color(r,g,b);
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
        SpriteRenderer my_sprite = GetComponent<SpriteRenderer>();
        originalColor = my_sprite.color;
    }

    // Update is called once per frame
    void Update()
    {
        //resetColor();
        //StartCoroutine(ExampleCoroutine());

    }
}
