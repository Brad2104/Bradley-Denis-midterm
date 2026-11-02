using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ColorChangerController : MonoBehaviour
{
    //This is about as simple a script as you can imagine
    //It makes it so that if you hit the space bar, the attached sprite changes colors
    
    //This is the SpriteRenderer component in charge of drawing this object's sprite
    public SpriteRenderer SR;

    //Any code inside of Update's {} brackets runs once per frame
    void Update()
    {
        //This if statement can be read "If I hit space, change the sprite's color"
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            //Here we update the SpriteRenderer's color to be red
            SR.color = Color.red;
        }
    }
}
