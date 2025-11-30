using System.Collections;
using TMPro;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField]
    private RectTransform uiGroup;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private GameObject[] itemObjects;
    [SerializeField]
    private int[] itemPrices;
    [SerializeField]
    private Transform[] itemSpawnPoints;
    [SerializeField]
    private string[] talkData;
    [SerializeField]
    private TextMeshProUGUI talkText;

    private Player enterPlayer;

    private Coroutine talkCo;
    // Start is called before the first frame update
    public void EnterShop(Player player)
    {
        enterPlayer = player;
        uiGroup.anchoredPosition = Vector3.zero;
    }

    // Update is called once per frame
    public void ExitShop()
    {
        animator.SetTrigger(AnimID.GreetHash);
        uiGroup.anchoredPosition = Vector3.down * 1000;

    }
    public void Buy(int index)
    {
        int price = itemPrices[index];

        if (enterPlayer.Coin < price)
        {
            if (talkCo != null)
            {
                StopCoroutine(talkCo);
            }
            talkCo = StartCoroutine(Talk());
            return;
        }
        enterPlayer.Coin -= price;
        Vector3 ranVec = (Vector3.right * Random.Range(-3, 3)) + (Vector3.forward * Random.Range(-3, 3));
        Instantiate(itemObjects[index], itemSpawnPoints[index].position + ranVec, itemSpawnPoints[index].rotation);
    }
    IEnumerator Talk()
    {
        talkText.text = talkData[1];
        yield return YieldCache.WaitForSeconds(2f);
        talkText.text = talkData[0];
    }
}
