using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject menuCamera;
    [SerializeField]
    private GameObject ingameCamera;
    [SerializeField]
    private Player player;
    [SerializeField]
    private BossMonster bossMonster;
    [SerializeField]
    private GameObject itemShop;
    [SerializeField]
    private GameObject weaponShop;
    [SerializeField]
    private GameObject stageEntrance;
    [SerializeField]
    private int stage;
    [SerializeField]
    private float playTime;
    [SerializeField]
    private bool isBattling;
    [SerializeField]
    private int monsterACount;
    [SerializeField]
    private int monsterBCount;
    [SerializeField]
    private int monsterCCount;
    //private float[] stats = new float[(int)StatType.Count];
    //public float this[StatType e] { get => stats[(int)e]; set => stats[(int)e] = value; }

    [SerializeField]
    private GameObject menuPanel;
    [SerializeField]
    private GameObject ingamePanel;
    [SerializeField]
    private TextMeshProUGUI bestScoreText;
    [SerializeField]
    private TextMeshProUGUI scoreText;
    [SerializeField]
    private TextMeshProUGUI stageText;
    [SerializeField]
    private TextMeshProUGUI playTimeText;
    [SerializeField]
    private TextMeshProUGUI playerHealthText;
    [SerializeField]
    private TextMeshProUGUI playerAmmoText;
    [SerializeField]
    private TextMeshProUGUI playerCoinText;
    [SerializeField]
    private Image weapon1Image;
    [SerializeField]
    private Image weapon2Image;
    [SerializeField]
    private Image weapon3Image;
    [SerializeField]
    private Image weaponRImage;
    [SerializeField]
    private TextMeshProUGUI monsterAText;
    [SerializeField]
    private TextMeshProUGUI monsterBText;
    [SerializeField]
    private TextMeshProUGUI monsterCText;
    [SerializeField]
    private RectTransform bossHealthGroup;
    [SerializeField]
    private RectTransform bossHealthBar;

    private void Awake()
    {
        bestScoreText.text = string.Format("{0:n0}", PlayerPrefs.GetInt("BestScore"));
    }
    public void StartGame()
    {
        menuCamera.SetActive(false);
        ingameCamera.SetActive(true);

        menuPanel.SetActive(false);
        ingamePanel.SetActive(true);

        player.gameObject.SetActive(true);
    }
    public void StartStage()
    {
        itemShop.SetActive(false);
        weaponShop.SetActive(false);
        stageEntrance.SetActive(false);

        StartCoroutine(StartBattle());
    }
    public void EndStage()
    {
        player.transform.position = Vector3.up * 0.8f;

        itemShop.SetActive(true);
        weaponShop.SetActive(true);
        stageEntrance.SetActive(true);

        isBattling = false;
        stage++;
    }
    IEnumerator StartBattle()
    {
        isBattling = true;
        yield return YieldCache.WaitForSeconds(5f);

        EndStage();


    }
    private void Update()
    {
        if (isBattling)
        {
            playTime += Time.deltaTime;
        }
    }
    private void LateUpdate()
    {
        scoreText.text = string.Format("{0:n0}", player.Score);
        stageText.text = $"STAGE {stage}";

        int hour = (int)(playTime / 3600);
        int minute = (int)(playTime % 3600 / 60);
        int second = (int)(playTime % 60);
        playTimeText.text = $"{hour:00}:{minute:00}:{second:00}";

        playerHealthText.text = player.Health + " / " + Player.HealthCap;
        playerCoinText.text = string.Format("{0:n0}", player.Coin);

        if (player.CurrentWeapon == null)
        {
            playerAmmoText.text = $"- / {player.Ammo}";
        }
        else if (player.CurrentWeaponType == WeaponType.Hammer)
        {
            playerAmmoText.text = $"- / {player.Ammo}";
        }
        else
        {
            playerAmmoText.text = $"{player.CurrentWeapon.CurrentMagazine} / {player.Ammo}";
        }

        weapon1Image.color = new Color(1, 1, 1, player.HasWeapons[0] ? 1 : 0);
        weapon2Image.color = new Color(1, 1, 1, player.HasWeapons[1] ? 1 : 0);
        weapon3Image.color = new Color(1, 1, 1, player.HasWeapons[2] ? 1 : 0);
        weaponRImage.color = new Color(1, 1, 1, player.GrenadeCount > 0 ? 1 : 0);
        monsterAText.text = monsterACount.ToString();
        monsterBText.text = monsterBCount.ToString();
        monsterCText.text = monsterCCount.ToString();
        bossHealthBar.localScale = new Vector3((float)bossMonster.CurrentHealth / bossMonster.MaxHealth, 1, 1);
    }
}