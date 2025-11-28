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
    public int MonsterACount { get { return monsterACount; } }
    [SerializeField]
    private int monsterBCount;
    public int MonsterBCount { get { return monsterBCount; } }
    [SerializeField]
    private int monsterCCount;
    public int MonsterCCount { get { return monsterCCount; } }
    //private float[] stats = new float[(int)StatType.Count];
    //public float this[StatType e] { get => stats[(int)e]; set => stats[(int)e] = value; }
    [SerializeField]
    private Transform[] monsterSpawnPoints;
    [SerializeField]
    private MonsterBase[] monsterPrefabs;
    [SerializeField]
    private List<int> monsterList;

    [SerializeField]
    private GameObject mainMenuPanel;
    [SerializeField]
    private GameObject hudPanel;
    [SerializeField]
    private TextMeshProUGUI bestScoreText;

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
        hudPanel.SetActive(true);

        player.gameObject.SetActive(true);
    }
    public void StartStage()
    {
        itemShop.SetActive(false);
        weaponShop.SetActive(false);
        stageEntrance.SetActive(false);

        foreach (Transform spawnPoint in monsterSpawnPoints)
        {
            spawnPoint.gameObject.SetActive(true);
        }

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
        isBattling = false;
        stage++;
    }
    IEnumerator StartBattle()
    {
        isBattling = true;

        for (int i = 0; i < stage; i++)
        {
            int randomIndex = Random.Range(0, 3);
            monsterList.Add(randomIndex);
        }
        for (int i = 0; i < monsterList.Count; i++)
        {
            int randomIndex = Random.Range(0, 4);
            MonsterBase monster = Instantiate(monsterPrefabs[monsterList[i]], monsterSpawnPoints[randomIndex].position, monsterSpawnPoints[randomIndex].rotation);
            monster.Target = player;

            yield return YieldCache.WaitForSeconds(4f);

        }
    }
    private void Update()
    {
        if (isBattling)
        {
            playTime += Time.deltaTime;
        }
    }
}