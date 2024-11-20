using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Novo composto", menuName = "Composto")]
public class Composto : ScriptableObject{
    public string nomeComposto;
    public List<Substancias> substancias = new List<Substancias>();
}
