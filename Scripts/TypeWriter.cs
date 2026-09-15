using System.Collections;
using TMPro;
using UnityEngine;


public class TypeWriter : MonoBehaviour
{
    public AudioSource audiosource;
    public SuperLibrary SuperLibrary;
    Coroutine typingCoroutine;
    TextMeshProUGUI currentTextBox;
    string currentText;
    bool currentTextIsAppend;
    public bool IsTyping => typingCoroutine != null;

    void Update()
    {
        if (IsTyping && Input.anyKeyDown)
            CompleteCurrentText();
    }

    public void ShowText(TextMeshProUGUI textBox, string fullText, float delayPerCharacter, AudioClip bleep)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        currentTextBox = textBox;
        currentText = fullText.Replace("\\n", "\n");
        currentTextIsAppend = false;
        typingCoroutine = StartCoroutine(TypeText(textBox, fullText, delayPerCharacter, bleep));
    }

    public void AddText(TextMeshProUGUI textBox, string addText, float delayPerCharacter, float delay, AudioClip bleep)
    {
        currentTextBox = textBox;
        currentText = addText.Replace("\\n", "\n");
        currentTextIsAppend = true;
        typingCoroutine = StartCoroutine(AddTextIE(textBox, addText, delayPerCharacter, delay, bleep));
    }

    void CompleteCurrentText()
    {
        if (currentTextBox == null)
            return;

        StopCoroutine(typingCoroutine);
        currentTextBox.text = currentTextIsAppend
            ? currentTextBox.text + currentText
            : currentText;
        typingCoroutine = null;
    }

    IEnumerator TypeText(TextMeshProUGUI textBox, string fullText, float delayPerCharacter, AudioClip bleep)
{
    fullText = fullText.Replace("\\n", "\n"); // convert literal \n into a real newline

    textBox.text = "";

    foreach (char letter in fullText)
    {
        textBox.text += letter;
        if (bleep != null && letter != ' ')
            audiosource.PlayOneShot(bleep);
        yield return new WaitForSeconds(delayPerCharacter);
    }

    typingCoroutine = null;
}

    IEnumerator AddTextIE(TextMeshProUGUI textBox, string addText, float delayPerCharacter, float delay, AudioClip bleep)
{
    addText = addText.Replace("\\n", "\n"); // same fix here

    yield return new WaitForSeconds(delay);

    foreach (char letter in addText)
    {
        textBox.text += letter;
        if (bleep != null && letter != ' ')
            audiosource.PlayOneShot(bleep);
        yield return new WaitForSeconds(delayPerCharacter);
    }

    typingCoroutine = null;
}
}
