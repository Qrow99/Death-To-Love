using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Rotation_fix : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //yeah this is a sloppy fix but it's just a bandaid for now
        this.gameObject.transform.rotation = Quaternion.Euler(0f,0f,0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
