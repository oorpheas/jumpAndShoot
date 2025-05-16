using UnityEngine;
using UnityEngine.UI;

public class Pontuacao : MonoBehaviour
{
    private Text _self;

    // Start is called before the first frame update
    void Start()
    {
        _self = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        _self.text = "pontuação" + GameManager.score;
    }
}
