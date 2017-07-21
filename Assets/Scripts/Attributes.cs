using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attributes : MonoBehaviour {

    private Animator anim;
    private float maxHP;

    [SerializeField]
    private float lifeAmount;


    void Awake()
    {
        maxHP = lifeAmount;
    }
    

    public void RecieveDamage(float damage)
    {
        lifeAmount = -damage;
        if (lifeAmount <= 0)
        {
            anim.SetFloat("life", lifeAmount);
        }
    }

}
