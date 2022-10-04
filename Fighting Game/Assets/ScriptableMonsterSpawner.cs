using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Scripting language for ScriptableMonsterSpawner.
 * s: spawn command. value: prefabList index.
 * p: position command. value: spawnPoint list index.
 * t: delay time command. value: time to delay (in seconds)
 * 
 * ex: p2;t1;s3;t1.1;s3;p0;t1;s5;
 */

public class ScriptableMonsterSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] prefabList;
    [SerializeField] private Transform[] spawnPoints;

    [SerializeField] private string scriptString;
    [SerializeField] private bool runOnAwake;

    private int currentSpawnPointIndex = 0;

    private void Start()
    {
        if (runOnAwake)
        {
            PlayScript();
        }
    }

    /// <summary>
    /// Run given script now.
    /// </summary>
    public void PlayScript(string script)
    {
        SetScript(script);
        PlayScript();
    }


    /// <summary>
    /// Run previously set script.
    /// </summary>
    public void PlayScript()
    {
        StartCoroutine(ScriptCoroutine());
    }


    /// <summary>
    /// Coroutine for running the script.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ScriptCoroutine()
    {
        string subString;
        char command;
        float value;
        int index;

        subString = scriptString;
        index = 0;

        // Parse and run script
        while (index < scriptString.Length)
        {
            // Get command. ex: p, t, s
            command = subString[0];

            // Get value. ex: 1.1, 5, 0
            string valueString = subString.Substring(1, subString.IndexOf(';') - 1);
            value = float.Parse(valueString);

            int increment = valueString.Length + 2;

            // Remove parsed command.
            subString = subString.Substring(increment);
            index += increment;

            // Run command using specified value as argument.
            switch (command)
            {
                case 'p':
                    // Choose spawn position
                    currentSpawnPointIndex = (int)value;
                    break;
                case 't':
                    // Delay time
                    yield return new WaitForSeconds(value);
                    break;
                case 's':
                    // Spawn something
                    Instantiate(prefabList[(int)value], spawnPoints[currentSpawnPointIndex].position, spawnPoints[currentSpawnPointIndex].rotation);
                    break;
            }
        }

        yield return null;
    }


    /// <summary>
    /// Stop script from running.
    /// </summary>
    public void StopScript()
    {
        StopCoroutine(ScriptCoroutine());
    }


    /// <summary>
    /// Set script string.
    /// </summary>
    /// <param name="scriptString"></param>
    public void SetScript(string scriptString)
    {
        this.scriptString = scriptString;
    }
}
