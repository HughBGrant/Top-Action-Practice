using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HudController : MonoBehaviour
{
    [SerializeField]
    private GameManager gameManager;
    [SerializeField]
    private Player player;
    [Header("Score")]
    [SerializeField]
    private TextMeshProUGUI scoreText;

    [Header("Boss")]
    [SerializeField]
    private RectTransform bossHealthGroup;
    [SerializeField]
    private RectTransform bossHealthBar;
    [SerializeField]
    private BossMonster bossMonster;

    [Header("Stage")]
    [SerializeField]
    private TextMeshProUGUI stageText;
    [SerializeField]
    private TextMeshProUGUI playTimeText;

    [Header("Status")]
    [SerializeField]
    private TextMeshProUGUI playerHealthText;
    [SerializeField]
    private TextMeshProUGUI playerAmmoText;
    [SerializeField]
    private TextMeshProUGUI playerCoinText;

    [Header("Equipment")]
    [SerializeField]
    private Image weapon1Image;
    [SerializeField]
    private Image weapon2Image;
    [SerializeField]
    private Image weapon3Image;
    [SerializeField]
    private Image weaponRImage;

    [Header("Monster")]
    [SerializeField]
    private TextMeshProUGUI monsterAText;
    [SerializeField]
    private TextMeshProUGUI monsterBText;
    [SerializeField]
    private TextMeshProUGUI monsterCText;

    private void Awake()
    {
        if (gameManager == null)
        {
            gameManager = GameManager.Instance;
        }
    }
    // Update is called once per frame
    void LateUpdate()
    {
        scoreText.text = string.Format("{0:n0}", player.Score);
        stageText.text = $"STAGE {gameManager.Stage}";

        int hour = (int)(gameManager.PlayTime / 3600);
        int minute = (int)(gameManager.PlayTime % 3600 / 60);
        int second = (int)(gameManager.PlayTime % 60);
        playTimeText.text = $"{hour:00}:{minute:00}:{second:00}";

        playerHealthText.text = player.Health + " / " + Player.HealthCap;
        playerCoinText.text = string.Format("{0:n0}", player.Coin);

        if (player.CurrentWeapon == null || player.CurrentWeaponType == WeaponType.Hammer)
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
        monsterAText.text = gameManager.MonsterACount.ToString();
        monsterBText.text = gameManager.MonsterBCount.ToString();
        monsterCText.text = gameManager.MonsterCCount.ToString();
        if (bossMonster != null)
        {
            bossHealthGroup.anchoredPosition = Vector3.down * 30;
            bossHealthBar.localScale = new Vector3((float)bossMonster.CurrentHealth / bossMonster.MaxHealth, 1, 1);
        }
        else
        {
            bossHealthGroup.anchoredPosition = Vector3.up * 200;
        }
    }
}
