using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static public int score;

    public Text scoreTxt;

    public float spawnTime;
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
    private float _timer, _spawnTimer;

    void Awake()
    {
        _spawns = GameObject.FindGameObjectsWithTag("spawn");
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(_spawns[0]);
        Debug.Log(_spawns[1]);

    }

    // Update is called once per frame
    void Update()
    {
        _timer += Time.deltaTime;
        _spawnTimer += Time.deltaTime;  

        scoreTxt.text = score.ToString(); // converte em string e mostra na caixa de texto

        if (_player != null)
        {
            LookForZombies();

            if (!_zums)
            {
                Debug.Log("Corrotina iniciada");
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
                for (int i = 0; i < howMuchZums; i++)
                {
                    SpawnEnemy(0);
                }
            }
            if (_hordeCount == 2)
            {
                for (int i = 0; i < howMuchZums; i++)
                {
                    SpawnEnemy(1);
                }
            }
        }
        else 
        {         
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
        if (_enemy != null)
        {
            _zums = true;
        }
        else
        {
            _zums = false;
        }
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
            zombies += zombies / 2;
            Debug.Log(zombies);
            HordeManager(zombies, _random);
        }

        yield return new WaitForSeconds(spawnTime);

    }
}
