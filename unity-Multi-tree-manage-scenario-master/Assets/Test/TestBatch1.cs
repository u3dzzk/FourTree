using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBatch1 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StaticBatchingUtility.Combine(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
