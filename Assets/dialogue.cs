using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class dialogue : MonoBehaviour {

    Text dialogueText;

    void Start()
    {
        dialogueText = GetComponent<Text>();
        StartCoroutine(DialogueCoroutine());
    }

    IEnumerator DialogueCoroutine()
    {
        int number = 10;
        dialogueText.text = "Foo";
        yield return new WaitForSeconds(2);
        dialogueText.text += " Bar";
        yield return new WaitForSeconds(2);
        dialogueText.text = "n = " + number;
        number *= 20;
        yield return new WaitForSeconds(2);
        dialogueText.text = "n = " + number;
    }
}
