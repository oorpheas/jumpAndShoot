using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FuncScene : MonoBehaviour
{
    [SerializeField] private string _cena;

    public void MudarCena()
    {
        SceneManager.LoadScene(_cena);
    }
}
