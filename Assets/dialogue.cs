using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class dialogue : MonoBehaviour {

    Text dialogueText;

    void Start()
    {
        dialogueText = GetComponent<Text>();
        dialogueText.text = "David + Brad + Roberto /n+ Alonso + Théo";
        StartCoroutine(DialogueCoroutine());
    }

    IEnumerator DialogueCoroutine()
    {
        int number = 10;
        dialogueText.text = "Foo";
        dialogueText.text += " Bar";
        dialogueText.text = "n = " + number;
        yield return new WaitForSeconds(2);
        dialogueText.text = "n = " + number;
        number *= 20;
    }
}
