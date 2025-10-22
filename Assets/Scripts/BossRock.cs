using System.Collections;
using UnityEngine;

public class BossRock : IDamageSource
{
    private Rigidbody rb;
    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(ChargeSpin());
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerIndex.Wall)
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

        while (chargeDuration > 0f && rb != null)
        {
            currentTorque += 0.02f;
            currentScale += 0.005f;
            rb.AddTorque(transform.right * currentTorque, ForceMode.Acceleration);
            transform.localScale = Vector3.one * currentScale;
            chargeDuration -= Time.deltaTime;

            yield return null;
        }
    }
}
