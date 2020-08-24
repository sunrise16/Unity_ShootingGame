using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDatabase : MonoBehaviour
{
    public int hitCount;
    public int grazeCount;
    
    void Start()
    {
        hitCount = 0;
        grazeCount = 0;
	}
}
