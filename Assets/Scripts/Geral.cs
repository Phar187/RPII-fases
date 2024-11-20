using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Geral{

    /// <summary>
    /// Método que executa o lambda no parâmetro após os segundos definidos.
    /// </summary>
    /// <param name="segundos"></param>
    /// <param name="lambda">Como usar: ()=>{trecho de código}</param>
    /// <returns></returns>
    public static IEnumerator Atraso(float segundos, System.Action lambda){
        yield return new WaitForSeconds(segundos);
        lambda?.Invoke();
    }   
}
