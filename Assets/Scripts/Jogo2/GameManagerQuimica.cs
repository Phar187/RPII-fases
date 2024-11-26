using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem.iOS;
using UnityEngine.UI;
public class GameManagerQuimica : MonoBehaviour{
    /// <summary>
    /// Armazena qual é o orçamento inicial.
    /// </summary>
    private float orcamentoInicial; 
    /// <summary>
    /// Armazena o orçamento que o jogador tem restando ao longo do jogo.
    /// </summary>
    private float orcamentoRestante;
    /// <summary>
    /// Armazena o orçamento que será gasto no melhor caminho.
    /// </summary>
    private float orcamentoIdeal;
    /// <summary>
    /// Armazena o orçamento que será gasto no pior caminho.
    /// </summary>
    private float orcamentoMaximo; 
    public GameObject canvasComputador, canvasBancada, canvasCientista, canvasLivroReceitasP1, canvasLivroReceitasP2, slots, objectMensagem;
    public int numPedidos;
    public Queue<PedidoQuimica> filaPedidos = new Queue<PedidoQuimica>();
    public List<Composto> listaCompostos = new List<Composto>();
    //Variáveis relacionadas à criação do composto
    public List<Sprite> listaSpritesSubstancias= new List<Sprite>();
    private Stack<Substancias> substanciasNoPote = new Stack<Substancias>();
    private int[] estoque = new int[6];//estoque de substâncias compradas estoque[0] vermelho, 1 amarelo, 2 azul, 3 roxo, 4 laranja e 5 verde
    private Coroutine exibirMensagem;
    //*********************************************
   
    // Start is called before the first frame update
    void Start(){
        GerarPedidos();//gerar os pedidos na fila e os precos das substancias.
        CalculaOrcamento();//calcular melhor e pior orçamentos com base nos pedidos gerados e atribuir às variáveis "orcamentoIdeal" e "orcamentoMaximo"
        //calcular um orçamento mais flexível e atribuir à variável "orcamentoInicial" FAZER AINDA!!!
        orcamentoInicial = orcamentoMaximo * 1.5f;
        orcamentoRestante = orcamentoInicial;
        AtualizarCanvasComputador();//atualiza o canvas do computador com as informações de orçamento e preço das substâncias
        AtualizarCanvasBancada();//atualiza o canvas da bancada com as informações do estoque.
        AtualizarPedido(false);//faz o cientista pedir o próximo pedido da lista.
    }

    // Update is called once per frame
    void Update(){
        
    }
    /// <summary>
    /// Método que será chamado pelos botões na tela de comprar substâncias.
    /// </summary>
    /// <param name="codigoBotao">Int que será associado a determinado botão (mesmo código para botões diferentes significa mesmo comportamento, então atenção com repetidos.)</param>
    /// <param name="textoBotao">GameObject do texto associado àquele botão.</param>
    public void BotaoComprar(int codigoBotao){
        if(orcamentoRestante - filaPedidos.Peek().precoSubstancias[codigoBotao] >= 0){
            orcamentoRestante -= filaPedidos.Peek().precoSubstancias[codigoBotao];//compra a substância.
            canvasComputador.transform.GetChild(2).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "Orçamento restante: R$ " + orcamentoRestante.ToString("F2");
            estoque[codigoBotao]++;//adiciona no estoque.
            AtualizarCanvasBancada();            
        }            
        else{
            ExibirMensagem("Dinheiro insuficiente!");
        }
        if(ChecarDerrota())
            EncerrarFase(false);
    }

    /// <summary>
    /// Checa se o jogo está em um estado impossível de ser terminado.
    /// </summary>
    /// <returns>
    /// <c>true</c> se o jogador não possui mais dinheiro para terminar a fase;
    /// <c>false</c> caso contrário.
    /// </returns>
    public bool ChecarDerrota(){
        //primeiro ver se o estoque é o bastante para completar o pedido atual
        //rodar foreach no pedido atual e nos preços e ir comparando com o estoque
        if(filaPedidos.Count == 0){
            Debug.LogError("ERRO: A fila de pedidos está vazia, portanto não faz sentido checar a condição de derrota.");
            return false;            
        }
        int[] precisa = new int[6];
        float precoMinimo = 0f;
        PedidoQuimica pedidoAtual = filaPedidos.Peek();
        foreach(Substancias s in pedidoAtual.composto.substancias){
            switch(s){
                case Substancias.Roxo:
                    //já possui o necessário em estoque.
                    if(estoque[(int)Substancias.Roxo] > 0 || (estoque[(int)Substancias.Vermelho] > 0 && estoque[(int)Substancias.Azul] > 0)){
                        break;
                    }
                    //checar qual é a opção mais barata
                    if(estoque[(int)Substancias.Vermelho] <= 0 && estoque[(int)Substancias.Azul] <= 0){
                        if(pedidoAtual.precoSubstancias[(int)Substancias.Roxo] < pedidoAtual.precoSubstancias[(int)Substancias.Vermelho] + pedidoAtual.precoSubstancias[(int)Substancias.Azul]){
                            precisa[(int)Substancias.Roxo]++;
                        }
                        else{
                            precisa[(int)Substancias.Vermelho]++;
                            precisa[(int)Substancias.Azul]++;
                        }
                    }
                    else if(estoque[(int)Substancias.Vermelho] > 0 && estoque[(int)Substancias.Azul] <= 0){
                        if(pedidoAtual.precoSubstancias[(int)Substancias.Roxo] < pedidoAtual.precoSubstancias[(int)Substancias.Azul]){
                            precisa[(int)Substancias.Roxo]++;
                        }
                        else{
                            precisa[(int)Substancias.Azul]++;
                        }
                    }
                    else if(estoque[(int)Substancias.Vermelho] <= 0 && estoque[(int)Substancias.Azul] > 0){
                        if(pedidoAtual.precoSubstancias[(int)Substancias.Roxo] < pedidoAtual.precoSubstancias[(int)Substancias.Vermelho]){
                            precisa[(int)Substancias.Roxo]++;
                        }
                        else{
                            precisa[(int)Substancias.Vermelho]++;
                        }
                    }
                    break;
                case Substancias.Laranja:
                    //já possui o necessário em estoque.
                    if(estoque[(int)Substancias.Laranja] > 0 || (estoque[(int)Substancias.Vermelho] > 0 && estoque[(int)Substancias.Amarelo] > 0)){
                        break;
                    }
                    //checar qual é a opção mais barata
                    if(estoque[(int)Substancias.Vermelho] <= 0 && estoque[(int)Substancias.Amarelo] <= 0){
                        if(pedidoAtual.precoSubstancias[(int)Substancias.Laranja] < pedidoAtual.precoSubstancias[(int)Substancias.Vermelho] + pedidoAtual.precoSubstancias[(int)Substancias.Amarelo]){
                            precisa[(int)Substancias.Laranja]++;
                        }
                        else{
                            precisa[(int)Substancias.Vermelho]++;
                            precisa[(int)Substancias.Amarelo]++;
                        }
                    }
                    else if(estoque[(int)Substancias.Vermelho] > 0 && estoque[(int)Substancias.Amarelo] <= 0){
                        if(pedidoAtual.precoSubstancias[(int)Substancias.Laranja] < pedidoAtual.precoSubstancias[(int)Substancias.Amarelo]){
                            precisa[(int)Substancias.Laranja]++;
                        }
                        else{
                            precisa[(int)Substancias.Amarelo]++;
                        }
                    }
                    else if(estoque[(int)Substancias.Vermelho] <= 0 && estoque[(int)Substancias.Amarelo] > 0){
                        if(pedidoAtual.precoSubstancias[(int)Substancias.Laranja] < pedidoAtual.precoSubstancias[(int)Substancias.Vermelho]){
                            precisa[(int)Substancias.Laranja]++;
                        }
                        else{
                            precisa[(int)Substancias.Vermelho]++;
                        }
                    }
                    break;
                case Substancias.Verde:
                    //já possui o necessário em estoque.
                    if(estoque[(int)Substancias.Verde] > 0 || (estoque[(int)Substancias.Amarelo] > 0 && estoque[(int)Substancias.Azul] > 0)){
                        break;
                    }
                    //checar qual é a opção mais barata
                    if(estoque[(int)Substancias.Amarelo] <= 0 && estoque[(int)Substancias.Azul] <= 0){
                        if(pedidoAtual.precoSubstancias[(int)Substancias.Verde] < pedidoAtual.precoSubstancias[(int)Substancias.Amarelo] + pedidoAtual.precoSubstancias[(int)Substancias.Azul]){
                            precisa[(int)Substancias.Verde]++;
                        }
                        else{
                            precisa[(int)Substancias.Amarelo]++;
                            precisa[(int)Substancias.Azul]++;
                        }
                    }
                    else if(estoque[(int)Substancias.Amarelo] > 0 && estoque[(int)Substancias.Azul] <= 0){
                        if(pedidoAtual.precoSubstancias[(int)Substancias.Verde] < pedidoAtual.precoSubstancias[(int)Substancias.Azul]){
                            precisa[(int)Substancias.Verde]++;
                        }
                        else{
                            precisa[(int)Substancias.Azul]++;
                        }
                    }
                    else if(estoque[(int)Substancias.Amarelo] <= 0 && estoque[(int)Substancias.Azul] > 0){
                        if(pedidoAtual.precoSubstancias[(int)Substancias.Verde] < pedidoAtual.precoSubstancias[(int)Substancias.Amarelo]){
                            precisa[(int)Substancias.Verde]++;
                        }
                        else{
                            precisa[(int)Substancias.Amarelo]++;
                        }
                    }
                    break;
                case Substancias.Vermelho:
                    precisa[(int)Substancias.Vermelho]++;
                    break;
                case Substancias.Amarelo:
                    precisa[(int)Substancias.Amarelo]++;
                    break;
                case Substancias.Azul:
                    precisa[(int)Substancias.Azul]++;
                    break;
                default:
                    Debug.LogError("");
                    break;
            }
        }
        for(int i = 0; i < precisa.Length; i++){
            precoMinimo += precisa[i] * pedidoAtual.precoSubstancias[i];
        }
        if(orcamentoRestante < precoMinimo){
            for(int i = 0; i < estoque.Length; i++){
                if(estoque[i] < precisa[i]){
                    if(i == (int)Substancias.Roxo){
                        if(estoque[(int)Substancias.Vermelho] < 1 || estoque[(int)Substancias.Azul] < 1){
                            return true;
                        }
                        else{
                            return false;
                        }
                    }
                    if(i == (int)Substancias.Laranja){
                        if(estoque[(int)Substancias.Vermelho] < 1 || estoque[(int)Substancias.Amarelo] < 1){
                            return true;
                        }
                        else{
                            return false;
                        }
                    }
                    if(i == (int)Substancias.Verde){
                        if(estoque[(int)Substancias.Amarelo] < 1 || estoque[(int)Substancias.Azul] < 1){
                            return true;
                        }
                        else{
                            return false;
                        }
                    }
                    return true;
                }
            }
            return false;       
        }           
        else
            return false;//jogo continua normalmente.
    }

    /// <summary>
    /// Método chamado pelos botões de fechar dos canvas.
    /// </summary>
    public void BotaoFechar(){
        canvasComputador.SetActive(false);
        canvasBancada.SetActive(false);
    }

    /// <summary>
    /// Método para colocar os compostos no pote.
    /// </summary>
    /// <param name="codigoBotao">Código do botão (0 vermelho ... 5 verde)</param>
    public void BotaoUsar(int codigoBotao){
        if(substanciasNoPote.Count < 4){
            switch(codigoBotao){
                case 0:
                    AdicionarSubstancia(Substancias.Vermelho);
                    break;
                case 1:
                    AdicionarSubstancia(Substancias.Amarelo);
                    break;
                case 2:
                    AdicionarSubstancia(Substancias.Azul);
                    break;
                case 3:
                    AdicionarSubstancia(Substancias.Roxo);
                    break;
                case 4:
                    AdicionarSubstancia(Substancias.Laranja);
                    break;
                case 5:
                    AdicionarSubstancia(Substancias.Verde);
                    break;
                default:
                    Debug.LogError("ERRO: código de botão " + codigoBotao + " não foi configurado no switch do método BotaoUsar(int codigoBotao).");
                    break;
            }
        }
        else{
            ExibirMensagem("O pote já está cheio!");
        }        
    }

    /// <summary>
    /// Método usado para misturar os compostos.
    /// </summary>
    public void BotaoCriar(int codigoBotao){
        switch(codigoBotao){
            case 3://criar roxo
                if(estoque[(int)Substancias.Vermelho] > 0 && estoque[(int)Substancias.Azul] > 0){
                    estoque[(int)Substancias.Vermelho]--;
                    estoque[(int)Substancias.Azul]--;
                    estoque[(int)Substancias.Roxo]++;
                    AtualizarCanvasBancada();
                }
                else{
                    ExibirMensagem("Substâncias insuficientes.");
                }
                break;
            case 4://criar laranja
                if(estoque[(int)Substancias.Vermelho] > 0 && estoque[(int)Substancias.Amarelo] > 0){
                    estoque[(int)Substancias.Vermelho]--;
                    estoque[(int)Substancias.Amarelo]--;
                    estoque[(int)Substancias.Laranja]++;
                    AtualizarCanvasBancada();
                }
                else{
                    ExibirMensagem("Substâncias insuficientes.");
                }
                break;
            case 5://criar verde
                if(estoque[(int)Substancias.Amarelo] > 0 && estoque[(int)Substancias.Azul] > 0){
                    estoque[(int)Substancias.Amarelo]--;
                    estoque[(int)Substancias.Azul]--;
                    estoque[(int)Substancias.Verde]++;
                    AtualizarCanvasBancada();
                }
                else{
                    ExibirMensagem("Substâncias insuficientes.");
                }
                break;
            default:
                Debug.LogError("ERRO: código de botão " + codigoBotao + " não foi configurado no switch do método BotaoCriar(int codigoBotao).");
                break;
        }
        if(ChecarDerrota()){
            EncerrarFase(false);
        }
    }

    /// <summary>
    /// Remove os compostos do pote.
    /// </summary>
    public void BotaoRemover(){
        RemoverSubstancia(false);
    }

    public void BotaoConfirmar(){
        //checar se estiver certo o composto
        int numMaxSubstancias = 0;
        int numIguais = 0;
        foreach(Substancias s1 in filaPedidos.Peek().composto.substancias){
            numMaxSubstancias++;
            foreach(Substancias s2 in substanciasNoPote){
                if(s1 == s2){
                    numIguais++;
                }                
            }
        }
        if(substanciasNoPote.Count != numMaxSubstancias || numIguais != numMaxSubstancias){
            ExibirMensagem("Composto incorreto.");
            //se estiver errado joga fora o composto? DECIDIR EM REUNIÃO
        }
        else{
            AtualizarPedido(true);//se estiver certo próximo cientista
            AtualizarCanvasComputador();//Atualiza com os novos preços
            canvasBancada.SetActive(false);//"tira" o jogador da bancada
            for(int i = 0; i < numMaxSubstancias; i++){//atualiza a UI
                RemoverSubstancia(true);
            }
        }       
    }

    public void BotaoLivroReceitas(){
        canvasBancada.SetActive(false);
        canvasLivroReceitasP1.SetActive(true);
    }

    public void BotaoFecharLivroReceitas(){
        canvasLivroReceitasP1.SetActive(false);
        canvasLivroReceitasP2.SetActive(false);
        canvasBancada.SetActive(true);
    }

    public void BotaoMudarPagina(GameObject canvasAlvo){
        canvasLivroReceitasP1.SetActive(false);
        canvasLivroReceitasP2.SetActive(false);

        canvasAlvo.SetActive(true);
    }

    /// <summary>
    /// Método para Adicionar substâncias no pote de criação de compostos.
    /// </summary>
    /// <param name="substancia">Tipo da substância que será adicionada.</param>
    private void AdicionarSubstancia(Substancias substancia){
        if(substanciasNoPote.Count >= 0 && substanciasNoPote.Count < 4){
            if(estoque[(int)substancia] > 0){
                Texture2D textura = listaSpritesSubstancias[(int)substancia].texture;
                if(textura != null){
                    slots.transform.GetChild(substanciasNoPote.Count).GetComponent<RawImage>().texture = textura;
                }
                else{
                    Debug.LogError("ERRO: Textura não encontrada! Checar o GameManager no editor!");
                }
                substanciasNoPote.Push(substancia);
                estoque[(int)substancia]--;
                AtualizarCanvasBancada();
            }
            else{
                ExibirMensagem("Compre mais desse tipo de substância no computador para usá-la!.");
            }                   
        }
    }

    /// <summary>
    /// Método para remover substâncias do pote de criação de compostos.
    /// </summary>
    private void RemoverSubstancia(bool limparPote){
        if(substanciasNoPote.Count > 0){
            if(!limparPote)
                estoque[(int)substanciasNoPote.Pop()]++;
            else
                substanciasNoPote.Pop();
            slots.transform.GetChild(substanciasNoPote.Count).GetComponent<RawImage>().texture = null;
            AtualizarCanvasBancada();
        }
    }

    private void EncerrarFase(bool ganhou){
        ConcluirFaseManagerScript script;
        script = GetComponent<ConcluirFaseManagerScript>();
        if(ganhou){
            int IDMedalha = 0;
            float orcamentoUtilizado = orcamentoInicial - orcamentoRestante;
            int performance = (int)(orcamentoIdeal/orcamentoUtilizado * 100);//(int) serve para pegar apenas a parte inteira.
            if(performance >= 0 && performance < 40){
                IDMedalha = 0;//nada
            }
            else if(performance >= 40 && performance < 65){
                IDMedalha = 1;//bronze
            }
            else if(performance >= 65 && performance < 85){
                IDMedalha = 2;//prata
            }
            else if(performance >= 85 && performance < 100){
                IDMedalha = 3;//ouro
            }
            else if(performance == 100){
                IDMedalha = 4;//diamante
            }
            else{
                Debug.LogError("CÁLCULO ERRADO! REVISAR SEU MÉTODO DE DEFINIR AS MEDALHAS!");
            }

            script.PopularCanvas(
                "Vitória!", 
                "Você criou todos os compostos dentro do orçamento!", 
                "Orçamento: " + orcamentoInicial.ToString("F2") + "\nOrçamento utilizado: " + orcamentoUtilizado.ToString("F2") + "\nPerformance: " + performance + "%", 
                "",//alterar essa linha após implementar o sistema de save.
                IDMedalha
            );
            script.SalvarDados(performance, 2, IDMedalha, "Novo recorde!");
        }
        else{
            script.PopularCanvas(
                "Derrota!",
                "Você estourou o orçamento!",
                "Muito triste :(",
                "Tente calcular melhor o preço dos compostos da próxima vez!",
                0
            );
        }
    }

    private void AtualizarCanvasBancada(){
        canvasBancada.transform.GetChild(2).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "Orçamento restante: R$ " + orcamentoRestante.ToString("F2");
        for(int i = 0; i < 6; i++){
            canvasBancada.transform.GetChild(1).GetChild(i).GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "Usar " + estoque[i];
        }
    }	

    private void AtualizarCanvasComputador(){
        //GetChild(1): Frascos
        //GetChild(2): Saldo
        canvasComputador.transform.GetChild(2).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "Orçamento restante: R$ " + orcamentoRestante.ToString("F2");
        for(int i = 0; i < 6; i++){
            if(filaPedidos.Count > 0)
                canvasComputador.transform.GetChild(1).GetChild(i).GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "Comprar R$ " + filaPedidos.Peek().precoSubstancias[i].ToString("F2");
        }
    }

    private void GerarPedidos(){
        for(int i = 0; i < numPedidos; i++){
            filaPedidos.Enqueue(CriarPedidoAleatorio());//escolhe um composto aleatório e coloca na fila de pedidos
        }
    }

    private PedidoQuimica CriarPedidoAleatorio(){
        PedidoQuimica pedido = new PedidoQuimica();
        System.Random r1 = new System.Random();
        System.Random r2 = new System.Random();
        pedido.composto = listaCompostos[r1.Next(0, listaCompostos.Count)];//escolhe um composto aleatório da lista para ser o pedido do cientista em questão.
        for(int i = 0; i < 3; i++){
            pedido.precoSubstancias[i] = r2.Next(10, 26);//aleatoriza o preço das substâncias vermelha, amarela e azul de 10 a 25.
            pedido.precoSubstancias[i + 3] = r2.Next(30, 51);//aleatoriza o preço das substâncias roxa, laranja e verde de 10 a 25.
        }       
        return pedido;
    }

    /// <summary>
    /// Utiliza um algoritmo guloso para calcular o melhor e pior caminho em termos de orçamento e salva nas variáveis "orcamentoIdeal" E "orcamentoMaximo"
    /// </summary>
    private void CalculaOrcamento(){
        foreach(PedidoQuimica p in filaPedidos){
            foreach(Substancias s in p.composto.substancias){
                //para cada substancia na lista de receitas do composto do pedido p:
                switch(s){
                    case Substancias.Roxo:
                        if(p.precoSubstancias[(int)Substancias.Vermelho] + p.precoSubstancias[(int)Substancias.Azul] >= p.precoSubstancias[(int)Substancias.Roxo]){
                            orcamentoIdeal += p.precoSubstancias[(int)Substancias.Roxo];//Roxo é mais barato.
                            orcamentoMaximo += p.precoSubstancias[(int)Substancias.Vermelho] + p.precoSubstancias[(int)Substancias.Azul];//opção mais cara.
                        }
                        else{
                            orcamentoIdeal += p.precoSubstancias[(int)Substancias.Vermelho] + p.precoSubstancias[(int)Substancias.Azul];//Vermelho + Azul é mais barato.
                            orcamentoMaximo += p.precoSubstancias[(int)Substancias.Roxo];
                        }
                        break;
                    case Substancias.Laranja:
                        if(p.precoSubstancias[(int)Substancias.Vermelho] + p.precoSubstancias[(int)Substancias.Amarelo] >= p.precoSubstancias[(int)Substancias.Laranja]){
                            orcamentoIdeal += p.precoSubstancias[(int)Substancias.Laranja];//Laranja é mais barato.
                            orcamentoMaximo += p.precoSubstancias[(int)Substancias.Vermelho] + p.precoSubstancias[(int)Substancias.Amarelo];//opção mais cara.
                        }
                        else{
                            orcamentoIdeal += p.precoSubstancias[(int)Substancias.Vermelho] + p.precoSubstancias[(int)Substancias.Amarelo];//Vermelho + Amarelo é mais barato.
                            orcamentoMaximo += p.precoSubstancias[(int)Substancias.Laranja];
                        }
                        break;
                    case Substancias.Verde:
                        if(p.precoSubstancias[(int)Substancias.Amarelo] + p.precoSubstancias[(int)Substancias.Azul] >= p.precoSubstancias[(int)Substancias.Verde]){
                            orcamentoIdeal += p.precoSubstancias[(int)Substancias.Verde];//Verde é mais barato.
                            orcamentoMaximo += p.precoSubstancias[(int)Substancias.Amarelo] + p.precoSubstancias[(int)Substancias.Azul];//opção mais cara.
                        }
                        else{
                            orcamentoIdeal += p.precoSubstancias[(int)Substancias.Amarelo] + p.precoSubstancias[(int)Substancias.Azul];//Amarelo + Azul é mais barato.
                            orcamentoMaximo += p.precoSubstancias[(int)Substancias.Verde];
                        }
                        break;
                    case Substancias.Vermelho:
                        orcamentoIdeal += p.precoSubstancias[(int)Substancias.Vermelho];
                        orcamentoMaximo += p.precoSubstancias[(int)Substancias.Vermelho];
                        break;
                    case Substancias.Amarelo:
                        orcamentoIdeal += p.precoSubstancias[(int)Substancias.Amarelo];
                        orcamentoMaximo += p.precoSubstancias[(int)Substancias.Amarelo];
                        break;
                    case Substancias.Azul:
                        orcamentoIdeal += p.precoSubstancias[(int)Substancias.Azul];
                        orcamentoMaximo += p.precoSubstancias[(int)Substancias.Azul];
                        break;
                    default:
                        Debug.LogError("DEU MUITO ERRADO NO CÁLCULO DO ORÇAMENTO!");
                        break;
                }
            }
        }
    }

    private void AtualizarPedido(bool tirarDaFila){
        canvasCientista.SetActive(true);
        if(filaPedidos.Count == 0){
            canvasCientista.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = "Acabaram os pedidos!";
        }
        else{
            if(tirarDaFila){
                filaPedidos.Dequeue();
                if(filaPedidos.Count > 0)
                    canvasCientista.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = "Quero um " + filaPedidos.Peek().composto.nomeComposto;
            }                
            else
                canvasCientista.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = "Quero um " + filaPedidos.Peek().composto.nomeComposto;
        }
        if(filaPedidos.Count <= 0){
            canvasCientista.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = "Acabaram os pedidos!";
            canvasBancada.SetActive(false);
            EncerrarFase(true);
        }
            
    }

    private void ExibirMensagem(string mensagem){
        if(exibirMensagem != null)
            StopCoroutine(exibirMensagem);
        objectMensagem.transform.parent.transform.parent.gameObject.SetActive(true);
        objectMensagem.GetComponent<TextMeshProUGUI>().text = mensagem;
        exibirMensagem = StartCoroutine(Geral.Atraso(3f, ()=>{
            objectMensagem.GetComponent<TextMeshProUGUI>().text = "";
            objectMensagem.transform.parent.transform.parent.gameObject.SetActive(false);
        }));
    }
}