using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{

    private float life_time = 3f;
    public GameObject mat1;
    public GameObject mat2;
    private Color originalColor1;
    private Color originalColor2;
    private float fadePercent;
    private float deathTime;
    private bool fading;

    

    // Start is called before the first frame update
    void Start()
    {
        deathTime = Time.time + life_time;
        originalColor1 = mat1.GetComponent<Renderer>().material.color;
        originalColor2 = mat2.GetComponent<Renderer>().material.color;
        StartCoroutine("Fade");
    }

    IEnumerator Fade()
    {
        while (true)
        {
            yield return new WaitForSeconds(.2f);
            if (fading)
            {
                fadePercent += Time.deltaTime;
                mat1.GetComponent<Renderer>().material.color = Color.Lerp(originalColor1, Color.clear, fadePercent);
                mat2.GetComponent<Renderer>().material.color = Color.Lerp(originalColor2, Color.clear, fadePercent);

                if (fadePercent >= 1)
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                if (Time.time > deathTime)
                {
                    fading = true;
                }
            }
            
        }
    }

    private void OnTriggerEnter(Collider c)
    {
        if (c.tag == "Ground")
        {

            GetComponent<AudioSource>().Play();
            GetComponent<Rigidbody>().Sleep();
        }
    }
}
