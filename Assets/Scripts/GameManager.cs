using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    static public int score;
    static public string ammo;

    public Text scoreTxt;
    public Text[] ammoTxt;

    public float spawnTimeMin, spawnTimeMax;
    public float waitHorde;
    public int zombies;
    public int initialHorde;
    public GameObject enemy;

    private GameObject[] _spawns;
    private GameObject _player;
    private GameObject _enemy;

    private bool _zums;
    private bool _random;
    private int _hordeCount = 0;
    private int _spawnSelect;
    private float _spawnTime;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        _spawns = GameObject.FindGameObjectsWithTag("spawn"); // encontra os spawn
        _player = GameObject.FindGameObjectWithTag("Player"); // encontra o player
    }

    // Update is called once per frame
    void Update()
    {
        scoreTxt.text = score.ToString(); // converte em string e mostra na caixa de texto

        if (_player != null)
        {
            LookForZombies(); // procura por zumbis

            if (!_zums)
            {
                //Debug.Log("Corrotina iniciada");
                StartCoroutine(Horde()); // chama a a horda
            }                
        }
    }

    // ATUALIZAÇÃO DE MUNIÇÃO DOS PLAYERS;

    public void UpdateAmmo(int pId, string ammo)
    {
        ammoTxt[pId].text = ammo; // atualiza o texto da munição;
    }

    private void OnEnable()
    {
        Player.OnAmmoChanged += UpdateAmmo;
    }

    private void OnDisable()
    {
        Player.OnAmmoChanged -= UpdateAmmo;
    }

    // ORGANIZAÇÃO DE HORDAS E SPAWN DE ZOMBIES;
    private bool LookForZombies() // procura por zumbis para encerrar a horda;
    {
        _enemy = GameObject.FindGameObjectWithTag("enemy");
        _zums = (_enemy != null);

        return _zums;
    }

    IEnumerator Horde() // chama horda;
    {
        _hordeCount++;
        Debug.Log("O número de horda é:" + _hordeCount);

        if (_hordeCount <= 2)
        {
            HordeManager(initialHorde, !_random);
        }
        else
        {
            zombies += zombies / 4;
            //Debug.Log(zombies);
            HordeManager(zombies, _random);
        }

        yield break;
    }

    void HordeManager (int howMuchZums, bool needRandom) // organiza as hordas de zumbis;
    {
        if (needRandom)
        {
            if (_hordeCount == 1)
            {
                StartCoroutine(SpawnZums(howMuchZums, 0));
            }
            if (_hordeCount == 2)
            {
                StartCoroutine(SpawnZums(howMuchZums, 1));
            }
        }
        else 
        {
            _spawnSelect = Random.Range(0, _spawns.Length);
            StartCoroutine(SpawnZums(howMuchZums, 3));
        }
    }

    IEnumerator SpawnZums(int Zums, int spawn) // spawn de zumbies por tempo;
    {
        if (spawn == 3) {
            for (int i = 0; i < Zums; i++) {
                _spawnTime = Random.Range(spawnTimeMin, spawnTimeMax);
                _spawnSelect = Random.Range(0, _spawns.Length);
                SpawnEnemy(_spawnSelect);
                yield return new WaitForSeconds(_spawnTime);
            }
        } else {
            _spawnTime = 0.2f;
            for (int i = 0; i < Zums; i++) {
                SpawnEnemy(spawn);
                yield return new WaitForSeconds(_spawnTime);
            }
        }

        yield break;
    }
    void SpawnEnemy(int selectSpawn) // spawna zumbies;
    {
        Instantiate(enemy, _spawns[selectSpawn].transform.position, transform.rotation);
    }
}
