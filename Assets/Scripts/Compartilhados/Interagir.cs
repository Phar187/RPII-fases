using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Interagir : MonoBehaviour{
    // Start is called before the first frame update
    public GameObject canvasComprarSub, canvasCriarComposto, canvasComputador;
    private bool isPertoComputador, isPertoBancada;
    void Start(){
        
    }

    // Update is called once per frame
    void Update(){
        if(isPertoComputador){
            if(Input.GetKey(KeyCode.Z)){
                Debug.Log("Interagiu com o computador.");
                canvasComputador.SetActive(true);
            }
            if(Input.GetKey(KeyCode.X)){
                canvasComputador.SetActive(false);
            }
        }
        if(isPertoBancada){
            if(Input.GetKey(KeyCode.Z)){
                Debug.Log("Interagiu com a bancada.");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other){
        //Debug.Log("Colidiu com: " + other.gameObject.name);
        if(other.gameObject.CompareTag("Dialogo")){
            other.gameObject.transform.GetChild(0).gameObject.SetActive(true);
        }
        if(other.gameObject.CompareTag("Computador")){
            canvasComprarSub.SetActive(true);
            isPertoComputador = true;
        }
        if(other.gameObject.CompareTag("Bancada")){
            canvasCriarComposto.SetActive(true);
            isPertoBancada = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other){
        if(other.gameObject.CompareTag("Dialogo")){
            other.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }
        if(other.gameObject.CompareTag("Computador")){
            canvasComprarSub.SetActive(false);
            isPertoComputador = false;
        }
        if(other.gameObject.CompareTag("Bancada")){
            canvasCriarComposto.SetActive(false);
            isPertoBancada = false;
        }
    }

    private void OnTriggerStay2D(Collider2D other){
        if(other.gameObject.CompareTag("Jogo1")){//serve para interagir com a máquina. Mudar essa função no futuro.
            if(Input.GetKey(KeyCode.Z)){//detecta o pressionamento da tecla 'Z'
                Debug.Log("Iniciando " + other.gameObject.name + ".");
                SceneManager.LoadSceneAsync("Jogo1");//carrega a scene do jogo 1 (pizzaria)
            }
        }
    }
}
