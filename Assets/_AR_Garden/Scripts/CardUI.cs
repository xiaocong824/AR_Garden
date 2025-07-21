using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Oculus.Interaction;

public class CardUI : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("CardInfo")]
    [SerializeField] private Button xuanyuButton;
    [SerializeField] private Button jiajieButton;
    [SerializeField] private Sprite xuanyu_normal;
    [SerializeField] private Sprite xuanyu_select;
    [SerializeField] private Sprite jiajie_normal;
    [SerializeField] private Sprite jiajie_select;
    [SerializeField] private GameObject xuanyu_card;
    [SerializeField] private GameObject jiajie_card;
    
    [Header("CardImageInfo")]
    [SerializeField] private GameObject[] tomatoCards;
    [SerializeField] private GameObject[] tomatoCards_text;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;

    private int currentCardIndex = 0;

    void Start()
    {
        // 默认选中xuanyu
        SelectXuanyu();
        xuanyuButton.onClick.AddListener(SelectXuanyu);
        jiajieButton.onClick.AddListener(SelectJiajie);
        UpdateCardDisplay();
        leftButton.onClick.AddListener(OnLeftButton);
        rightButton.onClick.AddListener(OnRightButton);
    }

    private void SelectXuanyu()
    {
        xuanyuButton.image.sprite = xuanyu_select;
        jiajieButton.image.sprite = jiajie_normal;
        xuanyu_card.SetActive(true);
        jiajie_card.SetActive(false);
    }

    private void SelectJiajie()
    {
        xuanyuButton.image.sprite = xuanyu_normal;
        jiajieButton.image.sprite = jiajie_select;
        xuanyu_card.SetActive(false);
        jiajie_card.SetActive(true);
    }

    private void OnLeftButton()
    {
        if (currentCardIndex > 0)
        {
            currentCardIndex--;
            UpdateCardDisplay();
        }
    }

    private void OnRightButton()
    {
        if (currentCardIndex < tomatoCards.Length - 1)
        {
            currentCardIndex++;
            UpdateCardDisplay();
        }
    }

    private void UpdateCardDisplay()
    {
        for (int i = 0; i < tomatoCards.Length; i++)
        {
            tomatoCards[i].SetActive(i == currentCardIndex);
            tomatoCards_text[i].SetActive(i == currentCardIndex);
        }
        leftButton.interactable = currentCardIndex > 0;
        rightButton.interactable = currentCardIndex < tomatoCards.Length - 1;
    }
}
