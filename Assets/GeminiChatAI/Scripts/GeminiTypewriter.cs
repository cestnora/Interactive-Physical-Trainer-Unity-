using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GeminiTypewriter : MonoBehaviour
{
    public float delay = 0.05f;
    public string fullText;
    private Coroutine typingCoroutine;

    public void StartTyping(string text)
    {
        fullText = AddLineBreaks(text, 38);

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(TypeText());
    }

    private IEnumerator TypeText()
    {
        this.GetComponent<TMP_Text>().text = ""; // Clear existing text

        foreach (char letter in fullText)
        {
            this.GetComponent<TMP_Text>().text += letter;
            yield return new WaitForSeconds(delay);
        }
    }

    private string AddLineBreaks(string text, int maxCharactersPerLine)
    {
        string processedText = "";
        int currentLineLength = 0;

        foreach (char c in text)
        {
            processedText += c;
            currentLineLength++;

            if (currentLineLength >= maxCharactersPerLine && c == ' ')
            {
                processedText += "\n";
                currentLineLength = 0;
            }
        }

        return processedText;
    }
}
