using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassModel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
       TestLoader testLoader =  gameObject.GetComponent<TestLoader>();
       testLoader.Model = PlayerPrefs.GetString("model");
    }
}
