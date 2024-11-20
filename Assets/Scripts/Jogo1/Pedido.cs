using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Pedido{
    public float precoTotal;
    public float pagamento;
    public GameObject caixa;

    public Pedido(float precoTotal, float pagamento, GameObject caixa){
        this.precoTotal = precoTotal;
        this.pagamento = pagamento;
        this.caixa = caixa;
    }
}

///a propriedade caixa representa a arte visual do pedido no jogo em si, um gameobject: unidae minima de onjeto no unity,
///sendo qualquer coisa que pode aparecer na tela do jogo.
///os gameobjects contem os componentes pre prontos do unity que garantes funcionliidades a mais aos objetos, como por exemplo 
///collider e rigidbody que cuidam de partes da movimentação masi especificamente da colisão e se ele atravessa ou nao outros itens
///como caixa pe um gameobject, todo pedido também é um componente visual do jogo, isso permite manipular esse elemento dentro do unity
/// esse gameobject não foi criado aqui, é uma referencias a um objeto que ja está presente na cena do unity
