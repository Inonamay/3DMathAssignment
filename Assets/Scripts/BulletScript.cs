using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    [SerializeField] float timer = 5;
    float timerCounter = 0;
    public GameObject target = null;
    // Start is called before the first frame update
    void Start()
    {
        timerCounter = timer;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (target && collision.collider.gameObject == target)
        {
            Debug.Log("Hit " + target.name + "!");
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(timerCounter < timer - 0.5f)
        {
            GetComponent<Collider>().enabled = true;
        }
        if(timerCounter > 0)
        {
            timerCounter -= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
