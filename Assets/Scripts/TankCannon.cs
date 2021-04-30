using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankCannon : MonoBehaviour
{
    [SerializeField]
    BallisticsProfile standardShot;
    [SerializeField] GameObject bullet = null;
    [SerializeField] float fireRate = 1;
    float fireCounter = 0;
    // Start is called before the first frame update
    public GameObject target = null;
    //TODO: Testa att directional checken funkar, testa att skjuta mot något specifikt
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject createdBullet = Instantiate(bullet);
            createdBullet.GetComponent<Collider>().enabled = false;
            createdBullet.transform.position = transform.position;
            createdBullet.transform.rotation = transform.rotation;
            createdBullet.GetComponent<Rigidbody>().AddForce(transform.forward * standardShot.Force);
            fireCounter = 0;
        }
        if (target)
        {
            bool hittable = Ballistics.IsSpotHittable(standardShot, transform, target.transform.position, transform.forward);
            if(hittable)
            {
                if (bullet && fireCounter >= fireRate)
                {
                    GameObject createdBullet = Instantiate(bullet);
                    createdBullet.transform.position = transform.position + transform.forward * 2;
                    createdBullet.transform.rotation = transform.rotation;
                    createdBullet.GetComponent<Rigidbody>().AddForce(transform.forward * standardShot.Force);
                    createdBullet.GetComponent<BulletScript>().target = target;
                    fireCounter = 0;
                }
                else
                {
                    Debug.Log("Hittable!");
                }
            }
            Ballistics.ShowCorrectTrajectory(standardShot, transform, target.transform.position, true);
        }
        if(fireCounter < fireRate)
        {
            fireCounter += Time.deltaTime;
        }
       
    }
}
