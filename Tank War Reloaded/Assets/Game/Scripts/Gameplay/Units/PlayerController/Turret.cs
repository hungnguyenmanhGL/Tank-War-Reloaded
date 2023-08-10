using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//must be seprated from attack component ShootBullet since it gots disables when using skill
public class Turret : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        //rotate turret based on mouse position on screen
        FollowMouseDirection();
    }

    void FollowMouseDirection()
    {
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        Vector3 lookPos = Camera.main.ScreenToWorldPoint(mousePos);
        lookPos = lookPos - transform.position;
        float angle = Mathf.Atan2(lookPos.y, lookPos.x) * Mathf.Rad2Deg;

        Quaternion to = Quaternion.Euler(transform.position.x, transform.position.y, angle - 90);
        //transform.rotation = Quaternion.Lerp(transform.rotation, to, rotateSpeed);
        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }
}
