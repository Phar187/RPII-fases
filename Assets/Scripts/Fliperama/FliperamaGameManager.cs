using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;

public class FliperamaGameManager : MonoBehaviour{
    public GameObject textoFichas;
    public GameObject parentLoja;
    public GameObject botaoPrefab;
    private int numItensLoja;
    // Start is called before the first frame update
    void Start(){
        //deleta os dados salvos toda vez que inicia. Remover essa linha quando terminar o jogo.
        PlayerPrefs.DeleteAll();
        GerarLoja();
    }

    // Update is called once per frame
    void Update(){
        
    }

    /*Lista de fazeres:
        método para acessar as fases -> jogador
        método de progressão de fases (playerprefs)
        sistema de diálogo
        loja de itens | feito
        colisões | feito
    */
    
    //loja de itens
    /// <summary>
    /// Método que armazena os itens da loja no PlayerPrefs.
    /// </summary>
    private void GerarLoja(){
        if(!PlayerPrefs.HasKey("multiplicaPontos")){
            PlayerPrefs.SetInt("multiplicaPontos", 0);
            numItensLoja++;
        }
    }

    /// <summary>
    /// Adiciona no estoque da loja.
    /// </summary>
    /// <param name="nomeItem">Nome do item que terá seu valor incrementado.</param>
    /// <param name="quantidade">Quantidade que será adicionada.</param>
    private void AdicionarItem(string nomeItem, int quantidade){
        if(PlayerPrefs.HasKey(nomeItem)){
            if(quantidade > 0){
                PlayerPrefs.SetInt(nomeItem, PlayerPrefs.GetInt(nomeItem) + quantidade);
                //caso estivesse esgotado trocar o texto para "comprar" e remover a transparência do botão.
            }
            else{
                Debug.LogError("Favor utilizar apenas valores maiores que 0.");
            }            
        }
        else{
            Debug.LogError("Nome de item inválido. Favor checar o método GerarLoja() para ver os nomes válidos." );
        }
    }

    /// <summary>
    /// Método que será chamado pelos botões de comprar para comprar um tipo de item.
    /// </summary>
    /// <param name="nomeItem">Nome do item que será comprado.</param>
    public void ComprarItem(string nomeItem){
        if(PlayerPrefs.HasKey(nomeItem)){
            if(PlayerPrefs.GetInt(nomeItem) > 0){
                PlayerPrefs.SetInt(nomeItem, PlayerPrefs.GetInt(nomeItem) - 1);
            }
            else{
                //modificar o botão para aparecer o texto "esgotado" e deixar mais transparente.
            }
        }
        else{
            Debug.LogError("Nome de item inválido. Favor checar o método GerarLoja() para ver os nomes válidos." );
        }
    }
    

}
