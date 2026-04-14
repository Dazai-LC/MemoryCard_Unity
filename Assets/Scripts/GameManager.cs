using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject cardPrefab;
    public Sprite[] cardSprites; // 5 hình
    public Sprite backSprite;

    public TextMeshProUGUI scoreText;
    public GameObject scoreUI;

    public GameObject endPanel;
    public TextMeshProUGUI finalScoreText;

    private int matchedPairs = 0;

    private List<Card> cards = new List<Card>();
    private Card firstCard;
    private Card secondCard;

    private bool canClick = true;
    private int score = 0;

    void Start()
    {
        SpawnCards();
    }

    void SpawnCards()
    {
        // 🔥 check tránh crash ngu
        if (cardPrefab == null)
        {
            Debug.LogError("CHƯA GÁN CARD PREFAB!");
            return;
        }

        List<(Sprite sprite, int id)> spawnList = new List<(Sprite, int)>();

        int idCounter = 0;

        // 🔥 tạo cặp
        foreach (Sprite s in cardSprites)
        {
            if (s == null) continue;

            spawnList.Add((s, idCounter));
            spawnList.Add((s, idCounter));

            idCounter++;
        }

        // 🔥 shuffle
        for (int i = 0; i < spawnList.Count; i++)
        {
            var temp = spawnList[i];
            int rand = Random.Range(i, spawnList.Count);
            spawnList[i] = spawnList[rand];
            spawnList[rand] = temp;
        }

        // 🔥 spawn + grid
        float spacing = 1.5f;
        int columns = 5;

        // 🔥 tính trước (chỉ 1 lần)
        int rows = Mathf.CeilToInt((float)spawnList.Count / columns);
        float width = (columns - 1) * spacing;
        float height = (rows - 1) * spacing;

        for (int i = 0; i < spawnList.Count; i++)
        {
            GameObject obj = Instantiate(cardPrefab);

            var data = spawnList[i];
            Card card = obj.GetComponent<Card>();
            card.Init(this, data.sprite, backSprite, data.id);

            int col = i % columns;
            int row = i / columns;

            float x = col * spacing - width / 2f;
            float y = height / 2f - row * spacing;

            obj.transform.position = new Vector3(x, y, 0);

            cards.Add(card);
        }
    }

    public bool CanClick()
    {
        return canClick;
    }

    public void OnCardClicked(Card card)
    {
        if (!canClick) return;

        if (firstCard == null)
        {
            firstCard = card;
        }
        else if (secondCard == null && card != firstCard)
        {
            secondCard = card;
            StartCoroutine(CheckMatch());
        }
    }

    IEnumerator CheckMatch()
    {
        canClick = false;

        yield return new WaitForSeconds(0.5f);

        if (firstCard.id == secondCard.id)
        {
            score += 10;

            firstCard.Hide();
            secondCard.Hide();

            matchedPairs++;

            // 🔥 check win
            if (matchedPairs >= cardSprites.Length)
            {
                EndGame();
            }
        }
        else
        {
            score -= 2;

            firstCard.Flip();
            secondCard.Flip();

            Debug.Log("Sai! Score: " + score);
        }

        UpdateScoreUI();

        firstCard = null;
        secondCard = null;

        canClick = true;
    }
    void UpdateScoreUI()
    {
        scoreText.text = "Score: " + score;
    }

    void EndGame()
    {
        // 🔥 ẩn UI điểm khi đang chơi
        if (scoreUI != null)
            scoreUI.SetActive(false);

        // 🔥 hiện panel kết thúc
        endPanel.SetActive(true);

        if (finalScoreText != null)
        {
            finalScoreText.text = "Score: " + score;
        }
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}