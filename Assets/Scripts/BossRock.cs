using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRock : HitBox
{
    private Rigidbody rb;
    private float angularPower = 2f;
    private float scale = 0.1f;
    private bool isShooting;
    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(GainPowerTimer());
        StartCoroutine(GainPower());
    }

    private IEnumerator GainPowerTimer()
    {
        yield return YieldCache.WaitForSeconds(2.2f);
        isShooting = true;

    }
    private IEnumerator GainPower()
    {
        while (!isShooting)
        {
            angularPower += 0.02f;
            scale += 0.005f;
            rb.AddTorque(transform.right * angularPower, ForceMode.Acceleration);
            transform.localScale = Vector3.one * scale;
            yield return null;
            Debug.Log("sfsdf");

        }
    }
}
