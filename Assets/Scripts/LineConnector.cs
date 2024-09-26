using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineConnector : MonoBehaviour
{
    public GameObject[] obj;

    private LineRenderer line;                   
    
    // Start is called before the first frame update
    void Start()
    {
        line = this.gameObject.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < obj.Length; i++)
        {
            line.SetPosition(i, obj[i].transform.position);
        }
    }
}
