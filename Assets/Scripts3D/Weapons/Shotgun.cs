using Assets.Helpers;
using UnityEngine;

public class Shotgun : Weapon
{
    [SerializeField]
    private int PelletsPerShell = 10;

    [SerializeField]
    private float Spread = 2;

    private float _refractory;
    private float _reloadTime;
        
    void Update()
    {
        if (InputHelper.PrimaryFire)
        {
            Fire();
        }
    }

    protected override void Fire()
    {
        var direction = GetFireDirection();
        var bullets = GetBullets(direction);
        FireBullets(bullets);
    }

    Vector3 GetFireDirection()
    {
        var ray = Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        Vector3 targetPoint = Physics.Raycast(ray, out var hit) ? hit.point : ray.GetPoint(Range); // range -> ~75?
        return targetPoint - AttackPoint.position;
    }

    GameObject[] GetBullets(Vector3 centreDirection)
    {
        GameObject[] bullets = new GameObject[PelletsPerShell];
        for (var i = 0; i < PelletsPerShell; ++i)
        {
            var bullet = Instantiate(Round, AttackPoint.position, Quaternion.identity);
            var bulletDirection = ApplySpread(centreDirection);
            bullet.transform.forward = bulletDirection;
            bullets[i] = bullet;
        }

        return bullets;
    }

    private Vector3 ApplySpread(Vector3 directionWithoutSpread)
    {
        if (Spread == 0)
        {
            return directionWithoutSpread;
        }

        float xSpread = Random.Range(-Spread, Spread);
        float ySpread = Random.Range(-Spread, Spread);
        var shotSpread = new Vector3(xSpread, ySpread);

        return directionWithoutSpread + shotSpread;
    }

    private void FireBullets(GameObject[] bullets)
    {
        foreach (var bullet in bullets)
        {
            bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward.normalized * ShootForce, ForceMode.Impulse);
        }

    }
}
