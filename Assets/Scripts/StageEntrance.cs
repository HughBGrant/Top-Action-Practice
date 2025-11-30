using UnityEngine;

public class StageEntrance : MonoBehaviour
{
    [SerializeField]
    private GameManager gameManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerID.Player)
        {
            gameManager.StartStage();
        }
    }
}
