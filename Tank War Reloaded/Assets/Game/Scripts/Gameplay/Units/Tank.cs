using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : Unit
{
    protected HullType hullType;

    [Header("Components")]
    [SerializeField] List<MonoBehaviour> affectedByEMP;

    //public IEnumerator hitByEMP(float time)
    //{
    //    foreach (MonoBehaviour script in affectedByEMP) script.enabled = false;
    //    yield return new WaitForSeconds(time);
    //    foreach (MonoBehaviour script in affectedByEMP) script.enabled = true;
    //}

    //void ResetCompUponKilled()
    //{
    //    foreach (MonoBehaviour script in affectedByEMP)
    //    {
    //        script.enabled = true;

    //        AutoMovement moveComp = script as AutoMovement;
    //        if (moveComp) moveComp.ResetUponKilled();

    //        EnemyAttack eAtk = script as EnemyAttack;
    //        AllyAttack aAtk = script as AllyAttack;
    //        if (eAtk) eAtk.ResetUponKilled();
    //        if (aAtk) aAtk.ResetUponKilled();
    //    }
    //}

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.CompareTag(SHELL_TAG))
    //    {
    //        Bullet b = collision.gameObject.GetComponent<Bullet>();

    //        if (b)
    //        {
    //            int dmg = b.dmg;
    //            if (commController) commController.WhenGotHit();
    //            TakeDamage(dmg);
    //        }

    //        if (b && b.emp)
    //        {
    //            if (hullType <= Hull.Heavy) StartCoroutine(hitByEMP(b.empTime));
    //            else StartCoroutine(hitByEMP(b.empTime / 1));
    //        }
    //        //replace enemy tank with ally tank (same tank model with different color and different component)
    //        if (b && b.convert)
    //        {
    //            if (hullType > Hull.Heavy)
    //            {
    //                collision.gameObject.SetActive(false);
    //                return; 
    //            }

    //            string tankName = "Converted Heavy Tank";
    //            if (this.hullType == Hull.Light)
    //            {
    //                tankName = "Converted Light Tank";
    //            }
    //            if (this.hullType == Hull.Medium)
    //            {
    //                tankName = "Converted Medium Tank";
    //            }

    //            GameObject tankReplacement = ObjectPool.instance.GetObjectForType(tankName, false);
    //            tankReplacement.transform.position = gameObject.transform.position;
    //            tankReplacement.transform.rotation = gameObject.transform.rotation;
    //            Tank t = tankReplacement.GetComponent<Tank>();
    //            t.HP = this.HP;
    //            AllyHolder.instance.allyList.Add(tankReplacement);

    //            EnemyPool.instance.killCount++;
    //            gameObject.SetActive(false);
    //        }

    //        collision.gameObject.SetActive(false);
    //    }

    //    //handle missile collision and - HP
    //    if (collision.gameObject.CompareTag("Missile"))
    //    {
    //        int dmg = collision.gameObject.GetComponent<GuidedMissile>().dmg;
    //        TakeDamage(dmg);

    //        collision.gameObject.SetActive(false);
    //    }
    //}

    //void explodeUponKilled()
    //{
    //    GameObject e = ObjectPool.instance.GetObjectForType("Tank Explosion", false);
    //    if (e != null) e.transform.position = gameObject.transform.position;
    //}
}
