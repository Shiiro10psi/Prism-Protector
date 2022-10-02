using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireController : MonoBehaviour
{
    float timer = 0;
    int counter = 0;

    List<LaserShooter> lasers;

    private void Start()
    {
        lasers = new List<LaserShooter>(FindObjectsOfType<LaserShooter>());
    }
    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            int laser = Random.Range(0, lasers.Count);
            lasers[laser].SetStrength(5 + Mathf.FloorToInt(counter / lasers.Count));
            lasers[laser].StartFiringSequence();
            timer = 10f;
            counter++;
        }
    }

    public int GetStrength()
    {
        return (5 + Mathf.FloorToInt(counter / lasers.Count));
    }
}
