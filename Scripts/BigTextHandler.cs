using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class BigTextHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public TextMeshProUGUI BigText;
    public List<DialogueEntry> dialogueList;
    public SuperLibrary SuperLibrary;
    float delaytime=0.05f;
    float waittime=0.5f;
    AudioClip bleep=null;
    Dictionary<string, DialogueEntry> basicLibrary = new Dictionary<string, DialogueEntry>();
    Dictionary<string, DialogueEntry> GoingForward = new Dictionary<string, DialogueEntry>();
    Dictionary<string, DialogueEntry> defaultlibrary = new Dictionary<string, DialogueEntry>();

    void Awake(){
        defaultlibrary["Standart_1"] = new DialogueEntry{
        mainText = "Battle Starts!"!,
        addTexts = new string[0]
        };
        defaultlibrary["Standart_2"] = new DialogueEntry{
        mainText = "The Rumble Begins!"!,
        addTexts = new string[0]
        };
    }
    public void ShowDialogue(string key)
    {
        if (!basicLibrary.ContainsKey(key))
        {
            return;
        }

        StartCoroutine(ShowDialogueSequence(basicLibrary[key]));
    }
    public Coroutine Check(DialogueEntry entry)
    {
        return StartCoroutine(ShowDialogueSequence(entry));
    }

    IEnumerator ShowDialogueSequence(DialogueEntry entry)
    {
        SuperLibrary.TW.ShowText(BigText, entry.mainText, delaytime, bleep);

        while (SuperLibrary.TW.IsTyping)
            yield return null;

        foreach (string addLine in entry.addTexts)
        {
            SuperLibrary.TW.AddText(BigText, addLine, delaytime, waittime, bleep);

            while (SuperLibrary.TW.IsTyping)
                yield return null;
        }
    }

    public void ChangeInformation(List<DialogueEntry> imported,List<DialogueEntry> forwardimported, float? dt, float? wt, AudioClip b)
    {
        if (imported.Count!=0 && imported != null)
        {
            int i=0;
            foreach (DialogueEntry en in imported){
            DialogueEntry entry = en;
            basicLibrary["Standart_"+i] = new DialogueEntry{
            mainText = entry.mainText,
            addTexts = entry.addTexts
            };
            i++;}
        }
        if (forwardimported != null && forwardimported.Count!=0)
        {
            int x=0;
            foreach (DialogueEntry en in forwardimported){
            DialogueEntry entry = en;
            basicLibrary["Standart_"+x] = new DialogueEntry{
            mainText = entry.mainText,
            addTexts = entry.addTexts
            };
            x++;}
        }

            delaytime=dt ?? 0.05f;
            waittime=wt ?? 1f;;
        if (b != null)
        {
            bleep=b;
        }
    }
    public void HideButton()
    {
        BigText.enabled=false;
    }


    public void ShowButton()
    {
        BigText.enabled=true;
    }

}
