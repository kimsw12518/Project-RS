using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    AudioSource AS;
    [SerializeField]
    float time = 0f;
    [SerializeField]
    float interval;
    int count = 0;

    IEnumerator Coroutine_musicChop()
    {
        while(true)
        {
            yield return new WaitForSecondsRealtime(interval);
            AS.Pause();
            AS.time = time;
            AS.Play();
            print(++count);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        AS = GetComponent<AudioSource>();
        StartCoroutine(Coroutine_musicChop());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        time += Time.fixedDeltaTime;
        //AS.Pause();
        //AS.time = time;
        //AS.Play();
    }
}
