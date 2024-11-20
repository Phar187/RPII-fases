using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConcluirFaseManagerScript : MonoBehaviour{

    public GameObject canvasConcluirFase;
    public GameObject fundoEscuro;
    public GameObject imagemFundo;
    /// <summary>
    /// Texto mais ao topo.
    /// </summary>
    public GameObject texto1;
    /// <summary>
    /// Texto abaixo do texto de topo.
    /// </summary>
    public GameObject texto2;
    /// <summary>
    /// Texto à esquerda da medalha.
    /// </summary>
    public GameObject texto3;
    /// <summary>
    /// Texto à direita da medalha.
    /// </summary>
    public GameObject texto4;
    /// <summary>
    /// Slot da medalha.
    /// </summary>
    public GameObject texto5;
    public GameObject slotMedalha;
    /// <summary>
    /// Lista contendo todos os sprites de medalha (incluindo sem medalha).
    /// </summary>
    public List<Sprite> spritesMedalhas = new List<Sprite>();

    private bool podeEncerrar;

    // Start is called before the first frame update
    void Start(){
        //PopularCanvas("Derrota!", "Você é horrível. Encontre outro hobby!", "Pontuação: 0 HAHAHAHA", "Melhore.", spritesMedalhas[0]);
    }

    // Update is called once per frame
    void Update(){
        
    }

    /// <summary>
    /// Método chamado ao clicar em qualquer lugar da tela de conclusão.
    /// </summary>
    public void PressionarTela(){
        //salvar dados do jogo em JSON, vou fazer isso mais tarde.
        //carregar cena do fliperama após 1 segundo.
        if(podeEncerrar){
            StartCoroutine(Geral.Atraso(1f, ()=>{
                SceneManager.LoadScene(0);
            }));  
        }              
    }
    /// <summary>
    /// Método que popula o canvas de conclusão de fase.
    /// </summary>
    /// <param name="textoTopo"></param>
    /// <param name="textoAbaixoDoTopo"></param>
    /// <param name="textoEsquerdaMedalha"></param>
    /// <param name="textoDireitaMedalha"></param>
    /// <param name="spriteMedalhaID">0- sem medalha | 1- bronze | 2- prata | 3- ouro | 4- diamante</param>
    public void PopularCanvas(string textoTopo, string textoAbaixoDoTopo, string textoEsquerdaMedalha, string textoDireitaMedalha, int spriteMedalhaID){
        float tempo = 0.8f;
        canvasConcluirFase.SetActive(true);
        fundoEscuro.SetActive(true);
        StartCoroutine(Geral.Atraso(tempo, ()=>{
            imagemFundo.SetActive(true);
            StartCoroutine(Geral.Atraso(tempo, ()=>{
                texto1.GetComponent<TextMeshProUGUI>().text = textoTopo;
                texto1.SetActive(true);
                StartCoroutine(Geral.Atraso(tempo, ()=>{
                    texto2.GetComponent<TextMeshProUGUI>().text = textoAbaixoDoTopo;
                    texto2.SetActive(true);
                    StartCoroutine(Geral.Atraso(tempo, ()=>{
                        texto3.GetComponent<TextMeshProUGUI>().text = textoEsquerdaMedalha;
                        texto3.SetActive(true);

                        texto4.GetComponent<TextMeshProUGUI>().text = textoDireitaMedalha;
                        texto4.SetActive(true);

                        StartCoroutine(Geral.Atraso(tempo, ()=>{
                            try{
                                slotMedalha.GetComponent<RawImage>().texture = spritesMedalhas[spriteMedalhaID].texture;
                            }
                            catch(IndexOutOfRangeException){
                                Debug.LogError("ERRO! Você provavelmente esqueceu de atribuir os sprites das medalhas no editor, na variável spritesMedalhas. Ou então colocou um índice inválido (menor que 0 ou maior que 5)");
                            }
                            StartCoroutine(Geral.Atraso(tempo, ()=>{
                                texto5.SetActive(true);
                                podeEncerrar = true;
                            }));
                        }));
                    }));
                }));
            }));
        }));
    }
}
