using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static public int score;
    static public string ammo;

    public Text scoreTxt;
    public Text ammoTxt;

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
        _spawns = GameObject.FindGameObjectsWithTag("spawn");
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log(_spawns[0]);
        //Debug.Log(_spawns[1]);

    }

    // Update is called once per frame
    void Update()
    {
        ammo = Player.ammo;

        scoreTxt.text = score.ToString(); // converte em string e mostra na caixa de texto
        ammoTxt.text = ammo;

        if (_player != null)
        {
            LookForZombies();

            if (!_zums)
            {
                //Debug.Log("Corrotina iniciada");
                StartCoroutine(Horde());
            }                
        }
    }

    void HordeManager (int howMuchZums, bool needRandom)
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
            for (int i = 0; i < howMuchZums; i++)
            {
                _spawnSelect = Random.Range(0, _spawns.Length);
                SpawnEnemy(_spawnSelect);
            }
        }
    }

    void SpawnEnemy(int selectSpawn)
    {
        Instantiate(enemy, _spawns[selectSpawn].transform.position, transform.rotation);
    }

    private bool LookForZombies()
    {
        _enemy = GameObject.FindGameObjectWithTag("enemy");
        _zums = (_enemy != null);

        return _zums;
    }

    IEnumerator Horde()
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

    IEnumerator SpawnZums(int Zums, int spawn)
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
}
