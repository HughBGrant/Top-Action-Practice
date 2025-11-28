//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;

//public class HudController : MonoBehaviour
//{
//    [SerializeField]
//    private Player player;
//    [Header("Score")]
//    [SerializeField]
//    private TextMeshProUGUI scoreText;

//    [Header("Boss")]
//    [SerializeField]
//    private RectTransform bossHealthGroup;
//    [SerializeField]
//    private RectTransform bossHealthBar;

//    [Header("Stage")]
//    [SerializeField]
//    private TextMeshProUGUI stageText;
//    [SerializeField]
//    private TextMeshProUGUI playTimeText;

//    [Header("Status")]
//    [SerializeField]
//    private TextMeshProUGUI playerHealthText;
//    [SerializeField]
//    private TextMeshProUGUI playerAmmoText;
//    [SerializeField]
//    private TextMeshProUGUI playerCoinText;

//    [Header("Equipment")]
//    [SerializeField]
//    private Image weapon1Image;
//    [SerializeField]
//    private Image weapon2Image;
//    [SerializeField]
//    private Image weapon3Image;
//    [SerializeField]
//    private Image weaponRImage;

//    [Header("Monster")]
//    [SerializeField]
//    private TextMeshProUGUI monsterAText;
//    [SerializeField]
//    private TextMeshProUGUI monsterBText;
//    [SerializeField]
//    private TextMeshProUGUI monsterCText;


//    // Update is called once per frame
//    void LateUpdate()
//    {
//        scoreText.text = string.Format("{0:n0}", player.Score);
//        stageText.text = $"STAGE {stage}";

//        int hour = (int)(playTime / 3600);
//        int minute = (int)(playTime % 3600 / 60);
//        int second = (int)(playTime % 60);
//        playTimeText.text = $"{hour:00}:{minute:00}:{second:00}";

//        playerHealthText.text = player.Health + " / " + Player.HealthCap;
//        playerCoinText.text = string.Format("{0:n0}", player.Coin);

//        if (player.CurrentWeapon == null)
//        {
//            playerAmmoText.text = $"- / {player.Ammo}";
//        }
//        else if (player.CurrentWeaponType == WeaponType.Hammer)
//        {
//            playerAmmoText.text = $"- / {player.Ammo}";
//        }
//        else
//        {
//            playerAmmoText.text = $"{player.CurrentWeapon.CurrentMagazine} / {player.Ammo}";
//        }

//        weapon1Image.color = new Color(1, 1, 1, player.HasWeapons[0] ? 1 : 0);
//        weapon2Image.color = new Color(1, 1, 1, player.HasWeapons[1] ? 1 : 0);
//        weapon3Image.color = new Color(1, 1, 1, player.HasWeapons[2] ? 1 : 0);
//        weaponRImage.color = new Color(1, 1, 1, player.GrenadeCount > 0 ? 1 : 0);
//        monsterAText.text = monsterACount.ToString();
//        monsterBText.text = monsterBCount.ToString();
//        monsterCText.text = monsterCCount.ToString();
//        bossHealthBar.localScale = new Vector3((float)bossMonster.CurrentHealth / bossMonster.MaxHealth, 1, 1);
//    }
//}
