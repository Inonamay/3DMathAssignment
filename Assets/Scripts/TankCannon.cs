using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankCannon : MonoBehaviour
{
    [SerializeField]
    BallisticsProfile standardShot;
    // Start is called before the first frame update
    public GameObject player = null;
    //TODO: Testa att directional checken funkar, testa att skjuta mot något specifikt
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (player)
        {
            bool hittable = Ballistics.IsSpotHittable(standardShot, transform, player.transform.position, transform.forward);
            if(hittable)
            {
                Debug.Log("Hittable!");
            }
        }
       
    }
}
