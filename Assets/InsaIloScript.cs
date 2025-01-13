using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;

public class InsaIloScript : MonoBehaviour {

    public KMBombModule Module;
    public KMBombInfo Bomb;
    public KMAudio Audio;

    public KMSelectable[] Buttons;
    public GameObject[] Screens;
    public Material[] Mats;
    public TextMesh[] Texts;

    private int chosenIx, anIx, answer;
    private string[] tokipona = { "a", "akesi", "ala", "alasa", "ale", "anpa", "ante", "anu", "awen", "e", "en", "esun", "ijo", "ike", "ilo", "insa", "jaki", "jan", "jelo", "jo", "kala", "kalama", "kama", "kasi", "ken", "kepeken", "kili", "kiwen", "ko", "kon", "kule", "kulupu", "kute", "la", "lape", "laso", "lawa", "len", "lete", "li", "lili", "linja", "lipu", "loje", "lon", "luka", "lukin", "lupa", "ma", "mama", "mani", "meli", "mi", "mije", "moku", "moli", "monsi", "mu", "mun", "musi", "mute", "nanpa", "nasa", "nasin", "nena", "ni", "nimi", "noka", "o", "olin", "ona", "open", "pakala", "pali", "palisa", "pan", "pana", "pi", "pilin", "pimeja", "pini", "pipi", "poka", "poki", "pona", "pu", "sama", "seli", "selo", "seme", "sewi", "sijelo", "sike", "sin", "sina", "sinpin", "sitelen", "sona", "soweli", "suli", "suno", "supa", "suwi", "tan", "taso", "tawa", "telo", "tenpo", "toki", "tomo", "tu", "unpa", "uta", "utala", "walo", "wan", "waso", "wawa", "weka", "wile" };
    private string[] trans = { 
        "jan seme who",
        "ma seme where",
        "tenpo seme when",
        "tu wan three",
        "tu tu four",
        "luka wan six",
        "luka tu seven",
        "luka luka ten",
        "pipi linja worm",
        "ma kasi forest",
        "noka kasi root",
        "kasi kule flower",
        "kiwen sijelo bone",
        "kiwen waso egg",
        "kiwen lawa horn",
        "linja monsi tail",
        "lipu waso feather",
        "kiwen uta tooth",
        "nena uta tongue",
        "nena noka knee",
        "luka waso wing",
        "sinpin sijelo belly",
        "noka lawa neck",
        "telo sewi rain",
        "telo tawa river",
        "telo suli sea",
        "kiwen telo ice",
        "kon seli smoke",
        "ko seli ash",
        "tenpo pimeja night",
        "tenpo suno day",
        "nena ala smooth",
        "telo ala dry",
        "lon poka near",
        "insa ilo module",
        "moli ilo bomb",
        "loje walo pink",
        "loje pimeja maroon",
        "loje jelo orange",
        "jelo laso green",
        "loje laso purple",
        "telo nasa alcohol",
        "lukin ala blind",
        "kute ala deaf",
        "pilin jan love",
        "walo pimeja gray",
        "mani ale rich",
        "mani ala poor",
        "pipi ilo glitch",
        "telo uta saliva",
        "meli mama mother",
        "mije mama father",
        "telo pimeja soda",
        "tenpo lili second",
        "tenpo suli year",
        "tenpo ale eternity",
        "jan lili baby",
        "tomo noka basement",
        "jan tu twin",
        "akesi suli dinosaur",
        "kili jelo banana",
        "lawa ike dumb",
        "ilo nena button",
        "kama pona welcome",
        "mi pakala sorry",
        "mi wile please",
        "jan pona friend",
        "jan ike enemy",
        "jan utala soldier",
        "tomo tawa vehicle",
        "ma tomo city",
        "jan alasa hunter",
        "jan kalama speaker",
        "jan lawa leader",
        "nimi mute sentence",
        "nanpa ilo calculator",
        "sitelen tawa movie",
        "sina ala others",
        "ale ala nothing",
        "wan nanpa first",
        "luka nanpa fifth"
    };
    private string chosenWord;
    private List<string> onButtons = new List<string> {};

    //Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    void Awake () {
        moduleId = moduleIdCounter++;
    
        foreach (KMSelectable button in Buttons) {
            button.OnInteract += delegate () { buttonPress(button); return false; };
        }
    }

    // Use this for initialization
    void Start () {
        chosenIx = UnityEngine.Random.Range(0, trans.Count());
        chosenWord = trans[chosenIx].Split(' ')[2];
        
        Screens[0].GetComponent<MeshRenderer>().material.mainTextureOffset = new Vector2(0.0625f * (Array.IndexOf(tokipona, trans[chosenIx].Split(' ')[0]) % 16), 0.125f * (7 - (Array.IndexOf(tokipona, trans[chosenIx].Split(' ')[0]) / 16)));
        Screens[1].GetComponent<MeshRenderer>().material.mainTextureOffset = new Vector2(0.0625f * (Array.IndexOf(tokipona, trans[chosenIx].Split(' ')[1]) % 16), 0.125f * (7 - (Array.IndexOf(tokipona, trans[chosenIx].Split(' ')[1]) / 16)));
        
        onButtons.Add(chosenWord);

        bool flag = false;
        while (onButtons.Count() <= 5) {
            flag = false;
            anIx = UnityEngine.Random.Range(0, trans.Count());
            string word = trans[anIx].Split(' ')[2];
            for (int w = 0; w < onButtons.Count(); w++) {
                if (word == onButtons[w]) {
                    flag = true;
                }
            }
            if (!flag) { onButtons.Add(word); }
        }
        onButtons.Shuffle();
        for (int m = 0; m < 6; m++) { //note: this needs to be changed on a case-by-case basis currently, should change later
            Texts[m].text = onButtons[m];
            switch (onButtons[m]) {
                case "dinosaur": case "speaker":
                    Texts[m].transform.localScale = new Vector3(0.065f, 0.075f, 0.1f);
                break;
                case "basement": case "calculator": case "sentence": case "welcome":
                    Texts[m].transform.localScale = new Vector3(0.055f, 0.075f, 0.1f);
                break;
                default: break;
            }
            if (onButtons[m] == chosenWord) {
                answer = m;
            }
        }
        Debug.LogFormat("[Insa Ilo #{0}] Words: {1}, {2}, {3}, {4}, {5}, {6}", moduleId, onButtons[0], onButtons[1], onButtons[2], onButtons[3], onButtons[4], onButtons[5]);
        Debug.LogFormat("[Insa Ilo #{0}] On displays: {1} {2}", moduleId, trans[chosenIx].Split(' ')[0], trans[chosenIx].Split(' ')[1]);
        Debug.LogFormat("[Insa Ilo #{0}] Correct word: {1}", moduleId, chosenWord);
    }

    void buttonPress(KMSelectable button) {
        button.AddInteractionPunch();
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        for (int b = 0; b < 6; b++) {
            if (!moduleSolved && button == Buttons[b]) {
                if (onButtons[b] == chosenWord) {
                    Debug.LogFormat("[Insa Ilo #{0}] You pressed {1}, that is correct. Module solved.", moduleId, chosenWord);
                    Module.HandlePass();
                    Screens[0].GetComponent<MeshRenderer>().material.mainTextureOffset = new Vector2(0.75f, 0.125f);
                    Screens[1].GetComponent<MeshRenderer>().material.mainTextureOffset = new Vector2(0.25f, 0.25f);
                    moduleSolved = true;
                    Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.CorrectChime, transform);
                } else {
                    Debug.LogFormat("[Insa Ilo #{0}] You pressed {1}, that is incorrect, you should've pressed {2}. Strike!", moduleId, onButtons[b], chosenWord);
                    Module.HandleStrike();
                }
            }
        }
    }

    //twitch plays
    private bool inputIsValid(string cmd)
    {
        string[] validstuff = { "1", "2", "3", "4", "5", "6" };
        if (validstuff.Contains(cmd))
        {
            return true;
        }
        return false;
    }

    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} press <#> [Presses that button in reading order]";
    #pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {
        string[] parameters = command.Split(' ');
        if (Regex.IsMatch(parameters[0], @"^\s*press\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant) || Regex.IsMatch(parameters[0], @"^\s*button\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant) || Regex.IsMatch(parameters[0], @"^\s*pos\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            if (parameters.Length == 2)
            {
                if (inputIsValid(parameters[1]))
                {
                    yield return null;
                    if (parameters[1].Equals("1"))
                    {
                        Buttons[0].OnInteract();
                    }
                    else if (parameters[1].Equals("2"))
                    {
                        Buttons[1].OnInteract();
                    }
                    else if (parameters[1].Equals("3"))
                    {
                        Buttons[2].OnInteract();
                    }
                    else if (parameters[1].Equals("4"))
                    {
                        Buttons[3].OnInteract();
                    }
                    else if (parameters[1].Equals("5"))
                    {
                        Buttons[4].OnInteract();
                    }
                    else if (parameters[1].Equals("6"))
                    {
                        Buttons[5].OnInteract();
                    }
                }
            }
            yield break;
        }
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        Buttons[answer].OnInteract();
        yield return new WaitForSeconds(0.1f);
    }
    
}
