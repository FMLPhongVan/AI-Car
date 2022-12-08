using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    [SerializeField] private Academy Academy;
    [SerializeField] private Transform target;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        gameObject.transform.position = new Vector3(0, 20, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (!target.gameObject.GetComponent<AIController>().Alive)
        {
            for (int i = 0; i < Academy._aiControllers.Length; ++i)
                if (Academy._aiControllers[i].Alive)
                {
                    target = Academy._aiControllers[i].transform;
                    break;
                }
        }

        gameObject.transform.position = new Vector3(target.position.x, 20, target.position.z);
    }
}
