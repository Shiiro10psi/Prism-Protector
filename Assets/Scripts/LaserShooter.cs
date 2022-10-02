using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserShooter : MonoBehaviour
{
    [SerializeField] Laser laser;
    ParticleSystem part;
    ParticleSystemRenderer partRender;

    [SerializeField] Vector2 aimDirection = Vector2.up;
    float fireStrength = 10f;
    float timer = -200;
    int strength = 5;
    int laserType = 0;

    GameState gs;
    AudioSource sound;

    private void Awake()
    {
        sound = GetComponent<AudioSource>();
        gs = FindObjectOfType<GameState>();
        part = GetComponent<ParticleSystem>();
        partRender = GetComponent<ParticleSystemRenderer>();
    }
    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0 && timer > -100)
        {
            Fire();
            timer = -200;
        }
    }

    void Fire()
    {
        part.Stop();
        var l = Instantiate(laser, transform.position, Quaternion.identity);
        l.SetType(laserType);
        l.SetStrength(strength);
        l.GetComponent<Rigidbody2D>().AddForce(aimDirection * fireStrength, ForceMode2D.Impulse);
        gs.LaserSurvived();
        sound.Play();
    }

    public void StartFiringSequence()
    {
        if (aimDirection.y == 0)
        {
            StartCoroutine(Move(new Vector2(transform.position.x,FindObjectOfType<HeartPiece>().transform.position.y)));
        }
        if (aimDirection.x == 0)
        {
            StartCoroutine(Move(new Vector2(FindObjectOfType<HeartPiece>().transform.position.x, transform.position.y)));
        }

        laserType = Random.Range(0, 4);

        switch (laserType)
        {
            case 0:
                partRender.material.color = Color.red;
                break;
            case 1:
                partRender.material.color = Color.blue;
                break;
            case 2:
                partRender.material.color = Color.yellow;
                break;
            case 3:
                partRender.material.color = Color.green;
                break;
        }

        part.Play();

        timer = 10f;
    }

    public void SetStrength(int i)
    {
        strength = i;
    }

    private IEnumerator Move(Vector2 target)
    {
        float delta = 0;
        Vector2 start = transform.position;

        do
        {
            delta += Time.fixedDeltaTime * 2f; if (delta > 1f) delta = 1f;
            transform.position = Vector2.Lerp(start, target, delta);
            yield return new WaitForFixedUpdate();
        } while ((Vector2)transform.position != target);
    }
}
