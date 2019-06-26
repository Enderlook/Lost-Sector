using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCarousel : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Pages to slide.")]
    public GameObject[] pages;

    private int currentPage = 0;

    /// <summary>
    /// Change the page shifting from pages by the <paramref name="amount"/>. If overflow the page amount it restart from the other bound.
    /// </summary>
    /// <param name="amount">Amount of pages to shift.</param>
    public void Change(int amount)
    {
        pages[currentPage].SetActive(false);
        currentPage = (currentPage + amount) % pages.Length;
        pages[currentPage].SetActive(true);
    }
}
