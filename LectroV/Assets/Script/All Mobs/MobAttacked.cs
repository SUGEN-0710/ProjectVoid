using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobAttacked : MonoBehaviour
{
    

    public void Died()
    {
        gameObject.SetActive(false);
    }
}
