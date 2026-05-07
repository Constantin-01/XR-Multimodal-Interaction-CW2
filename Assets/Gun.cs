using UnityEngine;

public class Gun : MonoBehaviour

{
    public GameObject bulletPrefab;

    public Transform firePoint;

    public ParticleSystem muzzleFlash;

    public float bulletForce = 20f;

    private bool isHeld = false;

    public void SetHeldTrue()
    {
        isHeld = true;
    }

    public void SetHeldFalse()
    {
        isHeld = false;
    }

    void Update()
    {
        if (isHeld && Input.GetKeyDown(KeyCode.F))
        {
            Shoot();
        }
    }

    void Shoot()

    {
        GetComponent<AudioSource>().Play();

        muzzleFlash.Play();

        GameObject bullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            firePoint.rotation
        );

        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        rb.AddForce(firePoint.forward * bulletForce, ForceMode.Impulse);

        Destroy(bullet, 5f);
    }
}
