using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformFall : MonoBehaviour
{
    Rigidbody2D rbody;
    Vector3 startPosition;
    [SerializeField] private float shakeTime = 0.5f;
    [SerializeField] private float shakeMagnitude = 5.0f;
    // Start is called before the first frame update
    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
        startPosition = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator ShakePlatform()
    {
        float elapsedTime = 0.0f;
        while (elapsedTime < shakeTime)
        {
            transform.position = startPosition + new Vector3(Random.Range(-shakeMagnitude, shakeMagnitude),
            Random.Range(-shakeMagnitude, shakeMagnitude), 0f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
       
        rbody.bodyType = RigidbodyType2D.Dynamic;
        rbody.AddForce(Vector2.up * 1.5f, ForceMode2D.Impulse);

    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == ("Player"))
        {
            if (col.gameObject.transform.position.y > startPosition.y)
            {
                StartCoroutine(ShakePlatform());
                Destroy(gameObject, 2.0f);
            }
        }
    }
}
