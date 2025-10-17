using System.Collections;
using UnityEngine;

public class BossRock : HitBox
{
    private Rigidbody rb;
    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(GainPower());
    }
    private IEnumerator GainPower()
    {
        float angularPower = 2f;
        float scale = 0.1f;
        float time = 0f;
        while (time < 2.2f)
        {
            angularPower += 0.02f;
            scale += 0.005f;
            rb.AddTorque(transform.right * angularPower, ForceMode.Acceleration);
            transform.localScale = Vector3.one * scale;
            time += Time.deltaTime;
            yield return null;
        }
    }
}
