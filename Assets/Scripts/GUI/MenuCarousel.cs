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
        currentPage += amount;
        if (currentPage == pages.Length)
            currentPage = 0;
        else if (currentPage < 0)
            currentPage = pages.Length - 1;
        pages[currentPage].SetActive(true);
    }
}
