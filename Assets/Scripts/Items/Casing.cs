using UnityEngine;

public class Casing : MonoBehaviour
{
    private Rigidbody rigid;
    public Rigidbody Rigid { get { return rigid; } }
    // Start is called before the first frame update
    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }
    void Start()
    {
        Destroy(gameObject, 3f);
    }
}
