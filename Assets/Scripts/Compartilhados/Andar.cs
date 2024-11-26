using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Andar : MonoBehaviour{
    SpriteRenderer spriteRenderer;
    public float velocidade;
    // Start is called before the first frame update
    void Start(){
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        velocidade = 5f;
    }

    // FixedUpdate pois envolve colisão.
    void FixedUpdate(){
        if(Input.GetKey(KeyCode.UpArrow)){
            this.transform.position += new Vector3(0f, 1f, 0f) * Time.deltaTime * velocidade;
        }
        if(Input.GetKey(KeyCode.DownArrow)){
            this.transform.position += new Vector3(0f, -1f, 0f) * Time.deltaTime * velocidade;
        }
        if(Input.GetKey(KeyCode.RightArrow)){
            this.transform.position += new Vector3(1f, 0f, 0f) * Time.deltaTime * velocidade;
            spriteRenderer.flipX = true;
        }
        if(Input.GetKey(KeyCode.LeftArrow)){
            this.transform.position += new Vector3(-1f, 0f, 0f) * Time.deltaTime * velocidade;
            spriteRenderer.flipX = false;
        }
    }
}
