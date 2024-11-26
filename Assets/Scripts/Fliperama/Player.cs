using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour{
    public GameObject entradaParent;
    public GameObject fundosParent;
    SpriteRenderer spriteRenderer;
    public float velocidade;
    private bool trava, trava1, trava2, trava3;
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

        RegularTamanho();
    }

    private void RegularTamanho(){
        
    }

    private void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.CompareTag("PortaEntrada") && !trava){
            
            this.transform.position = new Vector3(-5.54f, -4.16f, 0f);
            entradaParent.SetActive(false);
            fundosParent.SetActive(true);
            trava = true;
        }
        if(other.gameObject.CompareTag("PortaSaida") && !trava){
            this.transform.position = new Vector3(-3.71f, -1.91f, 0f);
            entradaParent.SetActive(true);
            fundosParent.SetActive(false);
            trava = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other){
        if(other.gameObject.CompareTag("PortaEntrada") || other.gameObject.CompareTag("PortaSaida")){
            trava = false;
        }
        if(other.gameObject.CompareTag("Jogo1")){
            trava1 = false;
        }
        if(other.gameObject.CompareTag("Jogo2")){
            trava2 = false;
        }
        if(other.gameObject.CompareTag("Jogo3")){
            trava3 = false;
        }
    }

    private void OnTriggerStay2D(Collider2D other){
        if(other.gameObject.CompareTag("Jogo1") && !trava2 && !trava3){
            if(Input.GetKey(KeyCode.Z)){//detecta o pressionamento da tecla 'Z'
                Debug.Log("Iniciando " + other.gameObject.name + ".");
                SceneManager.LoadSceneAsync(1);//carrega a scene do jogo 1 (pizzaria)
                trava1 = true;
            }
        }
        if(other.gameObject.CompareTag("Jogo2") && !trava1 && !trava3){
            if(Input.GetKey(KeyCode.Z)){//detecta o pressionamento da tecla 'Z'
                Debug.Log("Iniciando " + other.gameObject.name + ".");
                SceneManager.LoadSceneAsync(2);//carrega a scene do jogo 2 (química)
                trava2 = true;
            }
        }
        if(other.gameObject.CompareTag("Jogo3") && !trava1 && !trava2){
            if(Input.GetKey(KeyCode.Z)){//detecta o pressionamento da tecla 'Z'
                Debug.Log("Iniciando " + other.gameObject.name + ".");
                //SceneManager.LoadSceneAsync(3);//carrega a scene do jogo 3 ()
                trava3 = true;
            }
        }
    }
}
