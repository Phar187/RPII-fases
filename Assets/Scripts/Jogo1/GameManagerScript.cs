using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    public GameObject precoPizza1, precoPizza2, precoPizza3;
    public float preco1, preco2, preco3;
    public GameObject NpcsParent;
    public Sprite spritePizza1, spritePizza2, spritePizza3;
    public GameObject canvasTemTroco, canvasCalculaTroco;

    private Queue<Pedido> pedidos = new Queue<Pedido>();

    private bool emEspera = false;
    /// <summary>
    /// Tempo padrão em segundos usado nas corrotinas de Espera() quando não especificado na chamada.
    /// </summary>
    public float tempoDeEspera;
    private bool primeiraVez = true;

    // Start is called before the first frame update
    void Start(){
        GerarPrecos(preco1, preco2, preco3);
        GerarPedidos();
        canvasCalculaTroco.SetActive(false);
        canvasTemTroco.SetActive(true);
        //função que controla a caixa de diálogo dos npcs 
            //aleatoriza o número de pizzas
            //quais pizzas
            //valor pago
    }    

    // Update is called once per frame
    void Update(){
        //função que controla o movimento dos npcs
        //função que controla as mecânicas de pagamento e troco
        
    }

    /// <summary>
    /// Coloca os preços das pizzas na tabela de valores de acordo com as variáveis.
    /// </summary>
    /// <param name="preco1">Preço da pizza 1</param>
    /// <param name="preco2">Preço da pizza 2</param>
    /// <param name="preco3">Preço da pizza 3</param>
    private void GerarPrecos(float preco1, float preco2, float preco3){
        precoPizza1.GetComponent<TextMeshProUGUI>().text = "R$ " + preco1.ToString("F2");
        precoPizza2.GetComponent<TextMeshProUGUI>().text = "R$ " + preco2.ToString("F2");
        precoPizza3.GetComponent<TextMeshProUGUI>().text = "R$ " + preco3.ToString("F2");
    }

    private void GerarPedidos(){
        foreach(Transform npc in NpcsParent.transform){
            Transform caixa = npc.GetChild(0);
            GameObject pizza1 = caixa.GetChild(0).gameObject;
            GameObject pizza2 = caixa.GetChild(1).gameObject;
            GameObject pagamento = caixa.GetChild(2).GetChild(1).gameObject;
            do{
                pizza1.GetComponent<SpriteRenderer>().sprite = GerarSpriteAleatorio();//tenta gerar um sprite de pizza até conseguir
            } while(pizza1.GetComponent<SpriteRenderer>().sprite == null);

            pizza2.GetComponent<SpriteRenderer>().sprite = GerarSpriteAleatorio();
            pagamento.GetComponent<TextMeshProUGUI>().text = "R$ " + GerarPagamentoAleatorio(caixa.gameObject, pizza1, pizza2).ToString("F2");
        }
    }

    private Sprite GerarSpriteAleatorio(){
        switch(UnityEngine.Random.Range(0,6)){//50% de chance de ser null
            case 0:
                return spritePizza1;
            case 1:
                return spritePizza2;
            case 2:
                return spritePizza3;
            default:
                return null;
        }        
    }

    private float GerarPagamentoAleatorio(GameObject caixa, params GameObject[] pizza){
        float pagamento = 0;
        foreach(GameObject g in pizza){
            Sprite sprite = g.GetComponent<SpriteRenderer>().sprite;
            if(sprite == spritePizza1){
                pagamento+= preco1;
            }
            else if(sprite == spritePizza2){
                pagamento+= preco2;
            }
            else if(sprite == spritePizza3){
                pagamento+= preco3;
            }
        }

        float extra = UnityEngine.Random.Range(0f, 0.2f * pagamento);//o cliente pode pagar até 20% a mais do preço total.
        extra = (float)Math.Round(extra * 10f)/10f;//pega apenas uma casa depois da vírgula.
        pedidos.Enqueue(new Pedido(pagamento, pagamento + extra, caixa));//adiciona na lista de pedidos a serem tratados.
        return pagamento + extra;
    }

    private void ResponderPedido(){
        Pedido pedidoAtual = pedidos.Peek();//primeiro pedido da fila.
        float extra = pedidoAtual.pagamento - pedidoAtual.precoTotal;//calcula o troco
        extra = (float)Math.Round(extra, 2);//remove imprecisões
        string entrada = canvasCalculaTroco.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text;
        entrada = entrada.Trim('0');//remove 0 extras.
        float entradaProcessada = 0;
        if (string.IsNullOrEmpty(entrada) || string.IsNullOrEmpty(entrada.Trim(','))){
            entrada = "0"; // Evitar erro caso todos os caracteres sejam zeros ou só tenha vírgulas
        }
        else{
            entradaProcessada = float.Parse(entrada, CultureInfo.GetCultureInfo("pt-BR"));//obtém o float do texto
        }
        if(pedidoAtual != null){
            if(extra == entradaProcessada){
                Debug.Log("Extra: " + extra + " Entrada: " + entradaProcessada);
                CalculaTrocoTexto("Troco correto! Próximo cliente!", Acao.Substituir);
                StartCoroutine(Pausa(()=>{
                    canvasCalculaTroco.SetActive(false);
                    CalculaTrocoTexto("Quanto de troco?", Acao.Substituir);
                    //animações() IMPORTANTE IMPORTANTE IMPORTANTE IMPORTANTE IMPORTANTE IMPORTANTE IMPORTANTE IMPORTANTE IMPORTANTE IMPORTANTE IMPORTANTE IMPORTANTE
                    canvasTemTroco.SetActive(true);
                    //remover pedido da fila
                    pedidoAtual.caixa.SetActive(false);//desabilita o pedido que foi tratado
                    pedidos.Dequeue();
                    pedidoAtual = pedidos.Peek();//próximo pedido da fila
                    //mostrar pedido do próximo cliente
                    pedidoAtual.caixa.SetActive(true);                                        
                }));
            }
            else{
                Debug.Log("Extra: " + extra + " Entrada: " + entradaProcessada);
                CalculaTrocoTexto("Troco Errado! Tente novamente!", Acao.Substituir);
                StartCoroutine(Pausa(()=>{
                    CalculaTrocoTexto("Quanto de troco?",Acao.Substituir);
                    primeiraVez = true;
                }));
            }
        }
    }
    public void Sim(){
        if(emEspera == false && pedidos.Count > 0){//impede que o botão funcione se for pressionado antes de acabar a corrotina ou se acabou a fila
            Pedido pedidoAtual = pedidos.Peek();
            if(pedidoAtual.precoTotal != pedidoAtual.pagamento){
                TemTrocoTexto("Correto! Agora calcule o troco.");                
                StartCoroutine(Pausa(()=>{
                    canvasTemTroco.SetActive(false);
                    canvasCalculaTroco.SetActive(true);
                    TemTrocoTexto("Tem troco?");
                    primeiraVez = true;
                }));            
            }
            else{
                //prosseguir com a interface de digitar o troco.
                TemTrocoTexto("Você errou! Na verdade não tem troco.");
                StartCoroutine(Pausa(()=>{
                    
                    TemTrocoTexto("Tem troco?");
                }));            
            }
        }        
    }

    public void Nao(){
        if(emEspera == false && pedidos.Count > 0){//impede que o botão funcione se for pressionado antes de acabar a corrotina ou se acabou a fila
            Pedido pedidoAtual = pedidos.Peek();
            if(pedidoAtual.precoTotal == pedidoAtual.pagamento){
                TemTrocoTexto("Correto! Não há troco.");
                pedidos.Dequeue();//tira pedido da lista
                StartCoroutine(Pausa(()=>{
                    pedidoAtual.caixa.SetActive(false);
                    //executar animações dos npcs().IMPORTANTE IMPORTANTE IMPORTANTE IMPORTANTE IMPORTANTE IMPORTANTE IMPORTANTE IMPORTANTE IMPORTANTE IMPORTANTE IMPORTANTE IMPORTANTE
                    pedidoAtual = pedidos.Peek();
                    pedidoAtual.caixa.SetActive(true);
                    TemTrocoTexto("Tem troco?");
                }));
            }
            else{
                TemTrocoTexto("Você errou! Na verdade tem troco sim.");
                StartCoroutine(Pausa(()=>{
                    TemTrocoTexto("Tem troco?");
                }));                
            }
        }        
    }

    public void PressionaTeclado(int codigoBotao){
        if(emEspera == false && pedidos.Count > 0){
            if(primeiraVez == true){
                CalculaTrocoTexto("", Acao.Substituir);
                primeiraVez = false;
            }
            switch(codigoBotao){
                case 0:
                    CalculaTrocoTexto("0", Acao.Adicionar);
                    break;
                case 1:
                    CalculaTrocoTexto("1", Acao.Adicionar);
                    break;
                case 2:
                    CalculaTrocoTexto("2", Acao.Adicionar);
                    break;
                case 3:
                    CalculaTrocoTexto("3", Acao.Adicionar);
                    break;
                case 4:
                    CalculaTrocoTexto("4", Acao.Adicionar);
                    break;
                case 5:
                    CalculaTrocoTexto("5", Acao.Adicionar);
                    break;
                case 6:
                    CalculaTrocoTexto("6", Acao.Adicionar);
                    break;
                case 7:
                    CalculaTrocoTexto("7", Acao.Adicionar);
                    break;
                case 8:
                    CalculaTrocoTexto("8", Acao.Adicionar);
                    break;
                case 9:
                    CalculaTrocoTexto("9", Acao.Adicionar);
                    break;
                case 10:
                    CalculaTrocoTexto(",", Acao.Adicionar);
                    break;
                case 11:
                    CalculaTrocoTexto("", Acao.Deletar);
                    break;
                case 12:
                    ResponderPedido();
                    break;
                default:
                    Debug.LogWarning("Código inválido, por favor use de 0 a 12.");
                    break;
            }
        }
    }

    private IEnumerator Pausa(float segundos, System.Action action){
        emEspera = true;
        yield return new WaitForSeconds(segundos);
        action?.Invoke();
        emEspera = false;
    }

    private IEnumerator Pausa(System.Action action){
        emEspera = true;
        yield return new WaitForSeconds(tempoDeEspera);
        action?.Invoke();
        emEspera = false;
    }

    private void TemTrocoTexto(string texto){
        canvasTemTroco.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = texto;
    }
    private void CalculaTrocoTexto(string texto, Acao acao){
        TextMeshProUGUI objetoTexto = canvasCalculaTroco.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        switch(acao){
            case Acao.Adicionar:
                objetoTexto.alignment = TextAlignmentOptions.Right;
                objetoTexto.text = objetoTexto.text + texto;
                break;
            case Acao.Deletar:
                objetoTexto.alignment= TextAlignmentOptions.Right;
                if(objetoTexto.text.Length > 0 && !objetoTexto.text.Equals("Quanto de troco?")){
                    objetoTexto.text = objetoTexto.text.Remove(objetoTexto.text.Length - 1);//remove o último caractere da string
                }                
                break;
            case Acao.Substituir:
                objetoTexto.alignment = TextAlignmentOptions.Center;
                objetoTexto.text = texto;
                break;
            default:
                Debug.LogWarning("Acão não encontrada, certifique-se que acao faz parte do enum.");
                break;
        }        
    }

}
