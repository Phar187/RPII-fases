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
