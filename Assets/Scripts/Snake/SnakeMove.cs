using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;

public class SnakeMove : MonoBehaviour
{
    public Transform SnakeTailPa;

    public GameObject WinCanvas;

   

    public float speed = 3f;
    public float rotationSpeed = 200f;

    float velX = 0f;

    public GameObject botonEmpezar;

    public GameObject GameOverCanvas;

    public float circleDiameter;

    private List<Transform> snakeTail = new List<Transform>();
    private List<Vector2> positions = new List<Vector2>();

    public Text timer;

    private int score = 0;

    public float timeLeft;


    public void StartGame()
    {
        positions.Add(SnakeTailPa.position);
        Time.timeScale = 1;
        score = 0;

    }

    private void Start()
    {
        Time.timeScale = 0;
       
    }

    public void AddTail()
    {
        Transform tail = Instantiate(SnakeTailPa, positions[positions.Count - 1], Quaternion.identity, transform);
        snakeTail.Add(tail);
        positions.Add(tail.position);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
       
            if (other.gameObject.CompareTag("Food"))
            {
                Destroy(other.gameObject, 0.02f);
                AddTail();
                score += 100;
            }
            else if (other.gameObject.CompareTag("border"))
            {
                GameOver();
            }
        
    }

    private void FixedUpdate()
    {
        if (Time.timeScale == 1)
        {
            transform.Translate(Vector2.up * speed * Time.fixedDeltaTime, Space.Self);

            transform.Rotate(Vector3.forward * velX * rotationSpeed * Time.fixedDeltaTime);

            botonEmpezar.SetActive(false);
        }
    }

   

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 1)
        {
            velX = CrossPlatformInputManager.GetAxis("Horizontal");

            float distance = ((Vector2)SnakeTailPa.position - positions[0]).magnitude;

            if (distance > circleDiameter)
            {
                Vector2 direction = ((Vector2)SnakeTailPa.position - positions[0]).normalized;

                positions.Insert(0, positions[0] + direction * circleDiameter);
                positions.RemoveAt(positions.Count - 1);

                distance -= circleDiameter;
            }

            for (int i = 0; i < snakeTail.Count; i++)
            {
                snakeTail[i].position = Vector2.Lerp(positions[i + 1], positions[i], distance / circleDiameter);
            }

            timeLeft -= Time.deltaTime;
            timer.text = "Time Left:" + Mathf.Round(timeLeft);
            if (timeLeft < 0)
            {
                GameOver();
            }
            else if (score >= 1000)
            {
                Win();
            }
        }
    }

    public void GameOver()
    {
      Time.timeScale = 0;
      GameOverCanvas.SetActive(true);
    }

    public void Win()
    {
        Time.timeScale = 0;
        WinCanvas.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(3);
    }

    public void ClaimMenu()
    {
        SceneManager.LoadScene(4);
    }


}
