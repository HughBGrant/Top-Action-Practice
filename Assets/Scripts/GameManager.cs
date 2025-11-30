using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField]
    private GameObject mainMenuCamera;
    [SerializeField]
    private GameObject inGameCamera;
    [SerializeField]
    private Player player;
    [SerializeField]
    private GameObject itemShop;
    [SerializeField]
    private GameObject weaponShop;
    [SerializeField]
    private GameObject stageEntrance;
    [SerializeField]
    private int stage;
    public int Stage { get { return stage; } }
    [SerializeField]
    private float playTime;
    public float PlayTime { get { return playTime; } }
    [SerializeField]
    private bool isBattling;
    [SerializeField]
    private int monsterACount;
    public int MonsterACount { get { return monsterACount; } set { monsterACount = value; } }
    [SerializeField]
    private int monsterBCount;
    public int MonsterBCount { get { return monsterBCount; } set { monsterBCount = value; } }
    [SerializeField]
    private int monsterCCount;
    public int MonsterCCount { get { return monsterCCount; } set { monsterCCount = value; } }
    [SerializeField]
    private int monsterDCount;
    public int MonsterDCount { get { return monsterDCount; } set { monsterDCount = value; } }
    //private float[] stats = new float[(int)StatType.Count];
    //public float this[StatType e] { get => stats[(int)e]; set => stats[(int)e] = value; }
    [SerializeField]
    private Transform[] monsterSpawnPoints;
    [SerializeField]
    private MonsterBase[] monsterPrefabs;

    [SerializeField]
    private BossMonster bossMonster;

    [SerializeField]
    private GameObject mainMenuPanel;
    [SerializeField]
    private HudController hudController;
    [SerializeField]
    private TextMeshProUGUI bestScoreText;

    private List<int> monsterList;
    private bool isEndedStage;

    private void Awake()
    {
        Instance = this;

        bestScoreText.text = string.Format("{0:n0}", PlayerPrefs.GetInt("BestScore"));

        monsterList = new List<int>();
    }
    public void StartGame()
    {
        mainMenuCamera.SetActive(false);
        inGameCamera.SetActive(true);

        mainMenuPanel.SetActive(false);
        hudController.gameObject.SetActive(true);

        player.gameObject.SetActive(true);
    }
    public void EndGame()
    {
    }
    public void StartStage()
    {
        stage++;
        itemShop.SetActive(false);
        weaponShop.SetActive(false);
        stageEntrance.SetActive(false);

        foreach (Transform spawnPoint in monsterSpawnPoints)
        {
            spawnPoint.gameObject.SetActive(true);
        }
        isBattling = true;

        StartCoroutine(StartBattle());

    }

    public void EndStage()
    {
        player.transform.position = Vector3.up * 0.8f;

        itemShop.SetActive(true);
        weaponShop.SetActive(true);
        stageEntrance.SetActive(true);

        foreach (Transform spawnPoint in monsterSpawnPoints)
        {
            spawnPoint.gameObject.SetActive(false);
        }
        //isBattling = false;
    }
    IEnumerator StartBattle()
    {
        if (stage % 5 != 0)
        {
            for (int i = 0; i < stage; i++)
            {
                int randomMonsterTypeIndex = Random.Range(0, monsterPrefabs.Length - 1);
                switch (randomMonsterTypeIndex)
                {
                    case 0:
                        monsterACount++;
                        break;
                    case 1:
                        monsterBCount++;
                        break;
                    case 2:
                        monsterCCount++;
                        break;
                }
                int randomMonsterSpawnPointIndex = Random.Range(0, monsterSpawnPoints.Length);
                MonsterBase monster = Instantiate(monsterPrefabs[randomMonsterTypeIndex], monsterSpawnPoints[randomMonsterSpawnPointIndex].position, monsterSpawnPoints[randomMonsterSpawnPointIndex].rotation);
                monster.Target = player;
                yield return YieldCache.WaitForSeconds(3f);
            }
        }
        else
        {
            monsterDCount++;

            bossMonster = Instantiate((BossMonster)monsterPrefabs[monsterPrefabs.Length - 1], monsterSpawnPoints[0].position, monsterSpawnPoints[0].rotation);
            bossMonster.Target = player;
            hudController.SetBoss(bossMonster);

        }
        while (isBattling)
        {
            yield return YieldCache.WaitForSeconds(1f);

            if (monsterACount + monsterBCount + monsterCCount + monsterDCount <= 0)
            {
                isBattling = false;
            }
        }
        yield return YieldCache.WaitForSeconds(3f);
        EndStage();
    }
    private void Update()
    {
        if (isBattling)
        {
            playTime += Time.deltaTime;
        }
    }
}