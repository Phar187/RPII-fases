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
    /// Método chamado ao clicar em qualquer lugar da tela de conclusão. Salva os dados do jogo e carrega o fliperama.
    /// </summary>
    /// <param name="fase">Número da fase para salvar o progresso do jogo (fase1 = 1 | fase2 = 2 | fase3 = 3).</param>
    public void PressionarTela(){
        //carregar cena do fliperama após 1 segundo.
        if(podeEncerrar){
            StartCoroutine(Geral.Atraso(1f, ()=>{
                SceneManager.LoadScene(0);
            }));  
        }              
    }

    /// <summary>
    /// Salva os dados da jogatina.
    /// </summary>
    /// <param name="pontuacao">Pontuação atingida pelo jogador.</param>
    /// <param name="numFase">Número da fase atual (1, 2 ou 3).</param>
    /// <param name="idMedalha">id da medalha que foi .</param>
    /// <param name="textoExibido">Texto que será exibido à direita da medalha caso seja um novo recorde para aquela fase.</param>
    public void SalvarDados(int pontuacao, int numFase, int idMedalha, string textoExibido){
        if(!PlayerPrefs.HasKey("melhorMedalha1")){
            PlayerPrefs.SetInt("melhorMedalha1", 0);
        }
        if(!PlayerPrefs.HasKey("melhorMedalha2")){
            PlayerPrefs.SetInt("melhorMedalha2", 0);
        }
        if(!PlayerPrefs.HasKey("melhorMedalha3")){
            PlayerPrefs.SetInt("melhorMedalha3", 0);
        }
        if(!PlayerPrefs.HasKey("recordeFase1")){
            PlayerPrefs.SetInt("recordeFase1", 0);
        }
        if(!PlayerPrefs.HasKey("recordeFase2")){
            PlayerPrefs.SetInt("recordeFase2", 0);
        }
        if(!PlayerPrefs.HasKey("recordeFase3")){
            PlayerPrefs.SetInt("recordeFase3", 0);
        }
        if(!PlayerPrefs.HasKey("numFichas")){
            PlayerPrefs.SetInt("numFichas", 0);
        }

        if(idMedalha < 0 || idMedalha > 4){
            Debug.LogError("ID de medalha inválido. Favor usar 0 a 4.");
            return;
        }
        switch (numFase){
            case 1:
                if(PlayerPrefs.GetInt("recordeFase1") < pontuacao){
                    PlayerPrefs.SetInt("recordeFase1", pontuacao);
                    PlayerPrefs.SetInt("melhorMedalha1", idMedalha);
                    texto4.GetComponent<TextMeshProUGUI>().text = textoExibido;
                }
                break;
            case 2:
                if(PlayerPrefs.GetInt("recordeFase2") < pontuacao){
                    PlayerPrefs.SetInt("recordeFase2", pontuacao);
                    PlayerPrefs.SetInt("melhorMedalha2", idMedalha);
                    texto4.GetComponent<TextMeshProUGUI>().text = textoExibido;
                }
                break;
            case 3:
                if(PlayerPrefs.GetInt("recordeFase3") < pontuacao){
                    PlayerPrefs.SetInt("recordeFase3", pontuacao);
                    PlayerPrefs.SetInt("melhorMedalha3", idMedalha);
                    texto4.GetComponent<TextMeshProUGUI>().text = textoExibido;
                }
                break;
            default:
                Debug.LogError("Número de fase inválido. Favor usar 1, 2 ou 3.");
                return;
        }       

        //aumenta o número de fichas de acordo com a medalha obtida (bronze 10, prata 20, ...).
        PlayerPrefs.SetInt("numFichas", PlayerPrefs.GetInt("numFichas") + idMedalha * 10);
    }

    /// <summary>
    /// Deleta os dados salvos.
    /// </summary>
    public void DeletarDados(){
        PlayerPrefs.DeleteAll();
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
        switch(spriteMedalhaID){
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
            default:
                Debug.LogError("Código da medalha inválido. Use 0 a 5.");
                break;
        }
    }
}
