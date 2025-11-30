using System.Collections;
using UnityEngine;

public class BossRock : IDamageSource
{
    private Rigidbody rigid;
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        StartCoroutine(ChargeSpin());
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerID.Wall)
        {
            Destroy(gameObject);
            return;
        }
        DealDamageTo(collision.gameObject);
    }
    private IEnumerator ChargeSpin()
    {
        float chargeDuration = 2f;

        float currentTorque = 2f;
        float currentScale = 0.1f;

        while (chargeDuration > 0f && rigid != null)
        {
            currentTorque += 0.02f;
            currentScale += 0.005f;
            rigid.AddTorque(transform.right * currentTorque, ForceMode.Acceleration);
            transform.localScale = Vector3.one * currentScale;
            chargeDuration -= Time.deltaTime;

            yield return null;
        }
    }
}
