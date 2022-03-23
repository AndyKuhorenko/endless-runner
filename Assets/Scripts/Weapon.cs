using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] GameObject body;
    [SerializeField] Player player;

    float currentRotation = 0f;
    float angleY = 2f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        body.transform.Rotate(0, angleY, 0, Space.Self);
    }

    private void OnTriggerEnter(Collider other)
    {

        Destroy(gameObject);

        print(1);

        player.PickWeapon();
    }
}
