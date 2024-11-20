using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    public GameObject precoPizza1, precoPizza2, precoPizza3, precoPizza4, precoPizza5, precoPizza6;
    public float preco1, preco2, preco3, preco4, preco5, preco6;
    public GameObject NpcsParent;
    public Sprite spritePizza1, spritePizza2, spritePizza3, spritePizza4, spritePizza5, spritePizza6;
    public GameObject canvasTemTroco, canvasCalculaTroco;
    public TextMeshProUGUI trocoDisplay;

    private Queue<Pedido> pedidos = new Queue<Pedido>();
    private float totalTrocoCalculado = 0;
    private bool emEspera = false;
    public float tempoDeEspera;
    private bool primeiraVez = true;

    // variáveis para a o temporizador
    public TextMeshProUGUI timerText; // Objto de texto que exibirá o tempo
    public float tempoMaximo = 600f; //10 minutos em segundos
    public float tempoRestante;
    private float tempoDecorrido;
    private bool faseFinalizada = false;


    

    void Start()
    {
        
        //inicializa o temporizador
        tempoDecorrido = 0f;
        tempoRestante = tempoMaximo;
        AtualizaTimerVisual();


        if (trocoDisplay == null)
    {
        Debug.LogError("trocoDisplay não está associado. Verifique no Inspector.");
    }
        GerarPrecos(preco1, preco2, preco3, preco4, preco5, preco6);
        GerarPedidos();
        canvasCalculaTroco.SetActive(false);
        canvasTemTroco.SetActive(true);
        
    }
    void Update()
    {
        // Verifica se a fase ainda não foi finalizada
        if (!faseFinalizada)
        {
            tempoDecorrido += Time.deltaTime;
            tempoRestante = tempoMaximo - tempoDecorrido;

            if (tempoDecorrido >= tempoMaximo)
            {
                // Finaliza a fase por tempo esgotado
                FinalizaFase(false);
            }
            AtualizaTimerVisual();
        }
        //função que controla o movimento dos npcs
        //função que controla as mecânicas de pagamento e troco
    }

     private void AtualizaTimerVisual()
    {
        int minutos = Mathf.FloorToInt(tempoRestante / 60);
        int segundos = Mathf.FloorToInt(tempoRestante % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutos, segundos); // Formato MM:SS
    }

     private void FinalizaFase(bool completouTodosPedidos)
    {
        faseFinalizada = true;
        // Exibe o tempo que o jogador levou
        Debug.Log("Tempo total gasto: " + tempoDecorrido + "segundos");

        if (completouTodosPedidos)
        {
            Debug.Log("Fase completada com sucesso!");
        }
        else
        {
            Debug.Log("Fase não completada a tempo!");
        }
    }



    // Função para adicionar os preços das pizzas 
    private void GerarPrecos(float preco1, float preco2, float preco3, float preco4, float preco5, float preco6)
    {
        precoPizza1.GetComponent<TextMeshProUGUI>().text = "R$ " + preco1.ToString("F2");
        precoPizza2.GetComponent<TextMeshProUGUI>().text = "R$ " + preco2.ToString("F2");
        precoPizza3.GetComponent<TextMeshProUGUI>().text = "R$ " + preco3.ToString("F2");
        precoPizza4.GetComponent<TextMeshProUGUI>().text = "R$ " + preco4.ToString("F2");
        precoPizza5.GetComponent<TextMeshProUGUI>().text = "R$ " + preco5.ToString("F2");
        precoPizza6.GetComponent<TextMeshProUGUI>().text = "R$ " + preco6.ToString("F2");
    }

    // Função para gerar pedidos aleatórios para os NPCs
    private void GerarPedidos()
    {
        foreach (Transform npc in NpcsParent.transform)
        {
            Transform caixa = npc.GetChild(0);
            GameObject pizza1 = caixa.GetChild(0).gameObject;
            GameObject pizza2 = caixa.GetChild(1).gameObject;
            GameObject pagamento = caixa.GetChild(2).GetChild(1).gameObject;

            do
            {
                pizza1.GetComponent<SpriteRenderer>().sprite = GerarSpriteAleatorio();
            } while (pizza1.GetComponent<SpriteRenderer>().sprite == null);

            pizza2.GetComponent<SpriteRenderer>().sprite = GerarSpriteAleatorio();
            pagamento.GetComponent<TextMeshProUGUI>().text = "R$ " + GerarPagamentoAleatorio(caixa.gameObject, pizza1, pizza2).ToString("F2");
        }
    }

    // Gera um sprite de pizza aleatório
    private Sprite GerarSpriteAleatorio()
    {
        switch (UnityEngine.Random.Range(0, 12))
        {
            case 0: return spritePizza1;
            case 1: return spritePizza2;
            case 2: return spritePizza3;
            case 3: return spritePizza4;
            case 4: return spritePizza5;
            case 5: return spritePizza6;
            default: return null;
        }
    }

    // Calcula o pagamento total baseado nas pizzas no pedido
  private float GerarPagamentoAleatorio(GameObject caixa, params GameObject[] pizza)
{
    float valorTotal = 0;

    // 1. Calcular o valor total do pedido com base nos sprites das pizzas
    foreach (GameObject g in pizza)
    {
        Sprite sprite = g.GetComponent<SpriteRenderer>().sprite;
        if (sprite == spritePizza1) valorTotal += preco1;
        else if (sprite == spritePizza2) valorTotal += preco2;
        else if (sprite == spritePizza3) valorTotal += preco3;
        else if (sprite == spritePizza4) valorTotal += preco4;
        else if (sprite == spritePizza5) valorTotal += preco5;
        else if (sprite == spritePizza6) valorTotal += preco6;
    }

    // 2. Inicializar o valor pago e preencher com as maiores notas possíveis (abordagem gulosa)
    float valorPago = 0;
    List<int> notasUsadas = new List<int>();
    
    // Notas disponíveis em ordem decrescente
    int[] notas = { 100, 50, 20, 10, 5, 2 };

    foreach (int nota in notas)
    {
        // Adiciona a maior quantidade possível da nota atual até que o valor total seja ultrapassado
        while (valorPago + nota <= valorTotal)
        {
            valorPago += nota;
            notasUsadas.Add(nota);  // Armazena a nota usada
        }
    }

    // 3. Adicionar uma variação aleatória, com probabilidades, se ainda não alcançou o valor total
    if (valorPago < valorTotal)
    {
        int extra = EscolherNotaExtraComProbabilidade();
        valorPago += extra;
        notasUsadas.Add(extra);
    }

    // Enfileira o pedido e retorna o valor final que será mostrado como o total do pagamento
    pedidos.Enqueue(new Pedido(valorTotal, valorPago, caixa));

    Debug.Log("Valor Total do Pedido: R$" + valorTotal);
    Debug.Log("Valor Pago pelo Cliente: R$" + valorPago);
    Debug.Log("Notas Usadas: " + string.Join(", ", notasUsadas));

    return valorPago;
}

// Função para escolher uma nota extra com base nas probabilidades
private int EscolherNotaExtraComProbabilidade()
{
    float randomValue = UnityEngine.Random.Range(0f, 100f); // Gera um valor entre 0 e 100 para representar porcentagem

    if (randomValue < 40f)
        return 5;  // 40% de chance de R$ 5
    else if (randomValue < 70f)
        return 10; // 30% de chance de R$ 10
    else if (randomValue < 90f)
        return 20; // 20% de chance de R$ 20
    else if (randomValue < 100f)
        return 50; // 10% de chance de R$ 50
    else if (randomValue < 109f)
        return 100; // 9% de chance de R$ 100
    else
        return 2;  // 20% de chance de R$ 2
}


    // Adiciona valores ao total do troco baseado no código da nota/moeda
    public void AdicionarValorTroco(int codigoValor)
    
    {
        Debug.LogError("chegou aq1");
        float valorAdicionar = 0f;
        Debug.LogError("cheggou aq2");
        switch (codigoValor)
        {
            case 100: valorAdicionar = 100; break;
            case 50: valorAdicionar = 50; break;
            case 20: valorAdicionar = 20; break;
            case 10: valorAdicionar = 10; break;
            case 5: valorAdicionar = 5; break;
            case 2: valorAdicionar = 2; break;
            case 1: valorAdicionar = 1; break;
            case 950: valorAdicionar = 0.5f; break;
            case 925: valorAdicionar = 0.25f; break;
            case 910: valorAdicionar = 0.1f; break;
            case 95: valorAdicionar = 0.05f; break;
            case 91: valorAdicionar = 0.01f; break;
            default: Debug.LogWarning("Valor de nota ou moeda inválido."); break;
        }
        Debug.LogError("aq3.");

        totalTrocoCalculado = Mathf.Round((totalTrocoCalculado + valorAdicionar) * 100f) / 100f;

         Debug.LogError("aq4.");
        AtualizarTrocoDisplay();
    }


     public void teste(int teste){
      Debug.LogError("chegou aqqqqqqqqqqqqqqqqqqqqqq1");  
      teste = 2;

     }

    // Atualiza o display do troco no UI
    private void AtualizarTrocoDisplay()
    {
        trocoDisplay.text = "R$ " + totalTrocoCalculado.ToString("F2");
    }

    // Confirma se o troco calculado está correto
    public void ConfirmarTroco()
{
    Pedido pedidoAtual = pedidos.Peek();
    float extra = Mathf.Round((pedidoAtual.pagamento - pedidoAtual.precoTotal) * 100f) / 100f;
   float trocoCalculadoTruncado = Mathf.Floor(totalTrocoCalculado * 100f) / 100f;


    Debug.LogError("extra: " + extra);
    Debug.LogError("totalTrocoCalculado: " + trocoCalculadoTruncado);
    

    if (trocoCalculadoTruncado == extra)
    {
        Debug.Log("Troco correto!");
        CalculaTrocoTexto("Troco correto! Próximo cliente!", Acao.Substituir);
        StartCoroutine(Pausa(() =>
        {
            canvasCalculaTroco.SetActive(false);
            CalculaTrocoTexto("Quanto de troco?", Acao.Substituir);
            canvasTemTroco.SetActive(true);
            pedidoAtual.caixa.SetActive(false);
            pedidos.Dequeue();
            totalTrocoCalculado = 0;
            AtualizarTrocoDisplay();
            pedidoAtual = pedidos.Peek();
            pedidoAtual.caixa.SetActive(true);
           
            
            


        }));
    }
    else
    {
        Debug.Log("Troco incorreto!");
        CalculaTrocoTexto("Troco Incorreto! Tente novamente.", Acao.Substituir);
    }
}


    // Reseta o valor do troco atual
    public void ResetarTroco()
    {
        totalTrocoCalculado = 0;
        
        AtualizarTrocoDisplay();
    }

    // Configura o texto do troco de acordo com a ação especificada
    private void CalculaTrocoTexto(string texto, Acao acao)
{
    
   TextMeshProUGUI objetoTexto = canvasCalculaTroco.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();


    if (objetoTexto == null)
    {
        Debug.LogError("Erro: TextMeshProUGUI não encontrado no terceiro filho de canvasCalculaTroco.");
        return;
    }

    switch (acao)
    {
        case Acao.Adicionar:
            objetoTexto.text += texto;
            break;
        case Acao.Substituir:
            objetoTexto.text = texto;
            break;
    }
}


    // Método para iniciar uma pausa e executar uma ação após o tempo definido
    private IEnumerator Pausa(System.Action action)
    {
        emEspera = true;
        yield return new WaitForSeconds(tempoDeEspera);
        action?.Invoke();
        emEspera = false;
    }

    // Funções para responder "Sim" ou "Não" para o troco
    public void Sim()
    {
        if (emEspera == false && pedidos.Count > 0)
        {
            Pedido pedidoAtual = pedidos.Peek();
            if (pedidoAtual.precoTotal != pedidoAtual.pagamento)
            {
                TemTrocoTexto("Certo! Tem troco.");
                StartCoroutine(Pausa(() =>
                {
                    canvasTemTroco.SetActive(false);
                    canvasCalculaTroco.SetActive(true);
            
                    primeiraVez = true;
                }));
            }
            else
            {
                TemTrocoTexto("Errou! Não tem troco.");
                //StartCoroutine(Pausa(() => TemTrocoTexto(" ")));
            }
        }
    }

    public void Nao()
    {
        if (emEspera == false && pedidos.Count > 0)
        {
            Pedido pedidoAtual = pedidos.Peek();
            if (pedidoAtual.precoTotal == pedidoAtual.pagamento)
            {
                TemTrocoTexto("Certo, Não tem troco.");
                pedidos.Dequeue();
                StartCoroutine(Pausa(() =>
                {
                    pedidoAtual.caixa.SetActive(false);
                    pedidoAtual = pedidos.Peek();
                    pedidoAtual.caixa.SetActive(true);
         
                }));
            }
            else
            {
                TemTrocoTexto("Errou! Tem troco.");
                //StartCoroutine(Pausa(() => TemTrocoTexto(" ")));
            }
        }
    }

    private void TemTrocoTexto(string texto)
    {
        canvasTemTroco.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = texto;
    }
    
}
