using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure; // Required in C#


public class Gun : MonoBehaviour
{
    public enum GunType
    {
        Semi,
        Burst,
        Auto
    }

    [Header("System")]
    private float seconds_Between_Shots;
    private float next_posible_shoot_time;
    private int currentAmmo;

    [Header("Components")]
    public float GunID;
    public GunType gunType;
    public float damage = 1;
    public float rpm;
    public int totalAmmo = 40;
    public int ammoPerMag = 10;
    public Transform spawn;
    public Transform Shell_Ejection_Point;
    public Rigidbody Shell;
    public LayerMask collisionMask;
    private LineRenderer Tracer;

    [Header("Reloading")]
    private bool reloading;

    [Header("UI")]
    private GameUI GameUI;

    [Header("Audio")]
    public AudioClip shootingSound;
    public AudioClip reloadingSound;
    public AudioClip noAmmoSound;

    AudioSource shooting_AudioSource;
    AudioSource reloading_AudioSource;
    AudioSource noAmmo_AudioSource;

    private void Start()
    {
        seconds_Between_Shots = 60 / rpm;
        if (GetComponent<LineRenderer>())
        {
            Tracer = GetComponent<LineRenderer>();
        }

        currentAmmo = ammoPerMag;

        GameUI = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameUI>();
        GameUI.Show_Ammo(currentAmmo, totalAmmo);

        shooting_AudioSource = AddAudio(shootingSound, false, false, 0.5f);
        reloading_AudioSource = AddAudio(reloadingSound, false, false, 0.5f);
        noAmmo_AudioSource = AddAudio(noAmmoSound, false, false, 0.5f);
    }

    public AudioSource AddAudio(AudioClip clip, bool loop, bool playAwake, float vol)
    {
        AudioSource newAudio = gameObject.AddComponent<AudioSource>();
        newAudio.clip = clip; 
        newAudio.loop = loop;
        newAudio.playOnAwake = playAwake;
        newAudio.volume = vol;
        return newAudio;
    }

    public void shoot()
    {
        if (Can_Shoot())
        {
            Ray ray = new Ray(spawn.position, spawn.forward);
            RaycastHit hit;
            float shot_distance = 20;
            if (Physics.Raycast(ray, out hit, shot_distance, collisionMask))
            {
                shot_distance = hit.distance;

                if (hit.collider.GetComponent<Entity>())
                {
                    hit.collider.GetComponent<Entity>().Take_Damage(damage);
                }
            }

            next_posible_shoot_time = Time.time + seconds_Between_Shots;
            currentAmmo--;

            GameUI.Show_Ammo(currentAmmo, totalAmmo);

            //audio
            shooting_AudioSource.Play();

            //this.GetComponent<AudioSource>().Play();



            //tracer
            if (Tracer)
            {
                StartCoroutine("Render_Tracer", ray.direction * shot_distance);
            }

            //shell
            Rigidbody new_shell = Instantiate(Shell, Shell_Ejection_Point.position, Quaternion.identity) as Rigidbody;
            new_shell.AddForce(Shell_Ejection_Point.forward * Random.Range(150, 200) + spawn.forward * Random.Range(-10, 10));

            //Vibration
            StartCoroutine("Vibrate");
        }
        else
        {
            noAmmo_AudioSource.Play();
        }
    }

    IEnumerator Vibrate()
    {
        GamePad.SetVibration(0, 1f, 1f);

        yield return new WaitForSeconds(0.3f);

        GamePad.SetVibration(0, 0f, 0f);
    }

    public void Shoot_Continuous()
    {
        if (gunType == GunType.Auto)
        {
            shoot();
        }
    }

    private bool Can_Shoot()
    {
        bool can_Shoot = true;

        if (Time.time < next_posible_shoot_time)
        {
            can_Shoot = false;
        }


        if (currentAmmo == 0)
        {
            can_Shoot = false;
        }

        if (reloading)
        {
            can_Shoot = false;
        }

        return can_Shoot;
    }

    IEnumerator Render_Tracer(Vector3 hit_point)
    {
        Tracer.enabled = true;

        Tracer.SetPosition(0, spawn.position);
        Tracer.SetPosition(1, spawn.position + hit_point);

        //wait 1 second
        yield return null;

        Tracer.enabled = false;
    }

    public bool Reload()
    {
        if (totalAmmo != 0 && currentAmmo != ammoPerMag)
        {
            return true;
            reloading = true;
        }

        return false;

    }

    public void FinishReload()
    {
        currentAmmo = ammoPerMag;
        totalAmmo -= ammoPerMag;

        if (totalAmmo < 0 )
        {
            currentAmmo += totalAmmo;
            totalAmmo = 0;
        }

        GameUI.Show_Ammo(currentAmmo, totalAmmo);
        reloading_AudioSource.Play();

        StartCoroutine("reloadTime");
    }

    IEnumerator reloadTime()
    {
        yield return new WaitForSeconds(0.3f);

        reloading = false;
    }
}
