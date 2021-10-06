using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuBlockController : MonoBehaviour
{
    void Start()
    {
        
    }
    void Update()
    {
        Move();
        Rotate();
        Kill();
    }
    private void Move()
    {
        transform.position -= new Vector3(0, 0.7f, 1) * Time.deltaTime;
    }
    void Rotate()
    {
        transform.eulerAngles -= new Vector3(0, 0, 30) * Time.deltaTime;
    }
    void Kill()
    {
        if(transform.position.y<4)
        {
            Destroy(this.gameObject);
        }
    }
}
