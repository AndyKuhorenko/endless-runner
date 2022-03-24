using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] GameObject body;
    private Player player;

    float angleY = 2f;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        body.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
    }

    // Update is called once per frame
    void Update()
    {
        body.transform.Rotate(0, angleY, 0, Space.Self);
    }

    private void OnTriggerEnter(Collider other)
    {

        Destroy(gameObject);

        player.PickWeapon();
    }
}
