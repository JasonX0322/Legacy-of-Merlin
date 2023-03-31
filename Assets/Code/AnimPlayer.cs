using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimPlayer : MonoBehaviour
{
    public Sprite[] sprites;
    Image img;
    // Start is called before the first frame update
    void Start()
    {
        img=GetComponent<Image>();
    }
    public void PlayOnce(Army.AfterAttack aa,int nDamage)
    {
        StartCoroutine(ienuPlayOnce(aa,nDamage));
    }

    IEnumerator ienuPlayOnce(Army.AfterAttack aa,int nDamage)
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            yield return 0;
            img.sprite = sprites[i];
        }
        aa(nDamage);
    }


}
