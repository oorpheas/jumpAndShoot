using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public static event Action OnGameOver;

    static public int score;
    static public string ammo;
    static public bool gameOver;

    public Text scoreTxt;
    public Text[] ammoTxt;
    public Text[] ammoTxt2;

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

    void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        _spawns = GameObject.FindGameObjectsWithTag("spawn"); // encontra os spawn
        gameOver = false;
    }

    // Update is called once per frame
    void Update()
    {
        scoreTxt.text = score.ToString(); // converte em string e mostra na caixa de texto

        CheckPlayer();

        if (_player != null)
        {
            LookForZombies(); // procura por zumbis

            if (!_zums)
            {
                //Debug.Log("Corrotina iniciada");
                StartCoroutine(Horde()); // chama a a horda
            }                
        } else {
            OnGameOver?.Invoke();
            gameOver = true;
        }
    }

    // ATUALIZAÇÃO DE MUNIÇÃO DOS PLAYERS;

    public void UpdateAmmo(int pId, string ammo)
    {
        ammoTxt[pId].text = ammo; // atualiza o texto da munição;
    }

    public void UpdateAmmo2(int wId, string ammo)
    {
        Debug.Log(ammo);
        ammoTxt2[wId].text = ammo;
    }

    private void OnEnable()
    {
        Player.OnAmmoChanged += UpdateAmmo;
        HeavyGun.OnAmmoChanged2 += UpdateAmmo2;
    }

    private void OnDisable()
    {
        Player.OnAmmoChanged -= UpdateAmmo;
        HeavyGun.OnAmmoChanged2 -= UpdateAmmo2;
    }

    // ORGANIZAÇÃO DE HORDAS E SPAWN DE ZOMBIES;
    private bool LookForZombies() // procura por zumbis para encerrar a horda;
    {
        _enemy = GameObject.FindGameObjectWithTag("enemy");
        _zums = (_enemy != null);

        return _zums;
    }

    private void CheckPlayer()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
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
            zombies += zombies / 3;
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
            _spawnSelect = UnityEngine.Random.Range(0, _spawns.Length);
            StartCoroutine(SpawnZums(howMuchZums, 3));
        }
    }

    IEnumerator SpawnZums(int Zums, int spawn) // spawn de zumbies por tempo;
    {
        if (spawn == 3) {
            for (int i = 0; i < Zums; i++) {
                _spawnTime = UnityEngine.Random.Range(spawnTimeMin, spawnTimeMax);
                _spawnSelect = UnityEngine.Random.Range(0, _spawns.Length);
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
