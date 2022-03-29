using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;

public class PlayerWeapon : MonoBehaviour
{

    [SerializeField] private int ammo = 0;
    [SerializeField] private Player player;
    [SerializeField] private UI ui;
    [SerializeField] private GameObject ammoShell;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private RigBuilder rigBuilder;

    private bool canShoot = true;

    private const int shootRange = 15;

    private float timeBetweenShots = 0.5f;

    private int killedEnemies = 0;

    private bool hasWeapon = false;

    void Start()
    {
        SetCurrentAmmo();
    }

    // Update is called once per frame
    void Update()
    {
        HandleFireInputs();
    }

    public void HandleFireInputs()
    {
        if (Input.GetMouseButtonDown(0) && canShoot && UI.currentState == GameState.Active)
        {
            StartCoroutine(Shoot());
        }
    }

    IEnumerator Shoot()
    {
        canShoot = false;

        if (ammo > 0)
        {
            ProcessRaycast();

            audioManager.PlayShot();

            GameObject gunShell = Instantiate(ammoShell, transform.position, Quaternion.Euler(90, 0, 0));

            gunShell.GetComponent<Rigidbody>().AddForce(new Vector3(0.02f, 0.003f, 0.002f));

            Destroy(gunShell, 1.5f);

            muzzleFlash.Play();

            ammo--;
            SetCurrentAmmo();
        }

        yield return new WaitForSeconds(timeBetweenShots);
        canShoot = true;
    }

    public void PickWeapon()
    {
        if (!hasWeapon)
        {
            hasWeapon = true;

            rigBuilder.enabled = true;

            gameObject.SetActive(true);
        }

        ammo++;

        SetCurrentAmmo();
    }


    private void ProcessRaycast()
    {

        Vector3 shotOrigin = transform.position;

        float rayThickness = 0.6f;

        // int layerMask = 1 << 2;

        // Collide with every layer except the second
        // layerMask = ~layerMask;

        int layerMask = 1 << 7; // Only with 7th layer

        RaycastHit[] hits = Physics.SphereCastAll(shotOrigin, rayThickness, transform.forward, shootRange, layerMask);

        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];

            // Debug.DrawLine(shotOrigin, hit.point, Color.red, 20f, false);

            SlowZombie slowZombie = hit.transform.GetComponent<SlowZombie>();
            FastZombie fastZombie = hit.transform.GetComponent<FastZombie>();

            if (slowZombie)
            {
                slowZombie.PlayDeathAnimation(hit.point);
                killedEnemies++;
            }
            else if (fastZombie)
            {
                fastZombie.PlayDeathAnimation(hit.point);
                killedEnemies++;
            }

            SetCurrentKills();
        }
    }

    private void SetCurrentKills()
    {
        ui.SetKillsText(killedEnemies);
    }

    private void SetCurrentAmmo()
    {
        ui.SetAmmoCount(ammo);
    }
}
