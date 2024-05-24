using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class WeaponAnimScript : MonoBehaviour
{
    public SpriteRenderer weaponAttackPosSR;
    private Animator anim;

    private Sprite PlayeroriginalWeaponSprite;

    void Start()
    {
        anim = this.GetComponent<Animator>(); 
    }
    public void StartPosition(string animName)
    {
        try
        {
            PlayeroriginalWeaponSprite = weaponAttackPosSR.sprite;
            weaponAttackPosSR.sprite = null;
            anim.Play(animName);
        }
        catch(Exception ex)
        {
            Debug.LogException(ex); 
        }

    }
    public void Reposition()
    {
        weaponAttackPosSR.sprite = PlayeroriginalWeaponSprite;
    }

}
