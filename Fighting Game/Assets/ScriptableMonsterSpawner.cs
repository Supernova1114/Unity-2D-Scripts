using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Scripting language for ScriptableMonsterSpawner.
 * s: spawn command.
 * p: position command.
 * t: delay time command.
 * 
 * ex: p2;t1;s3;t1;s3;p0;t1;s5;
 */

public class ScriptableMonsterSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] prefabList;
    [SerializeField] private Transform[] spawnPoints;

    private int currentSpawnPointIndex = 0;

    [SerializeField] private string scriptString;

    private void Start()
    {
        PlayScript();
    }


    public void PlayScript()
    {
        IEnumerator ScriptCoroutine()
        {
            string scriptSubString;
            char command;
            float value;
            int index;

           
            index = 0;

            // p2;t1;s3;t1;s3;p0;t1;s5;
            // index 
            //
            //
            //
            //
            //

            while (index < scriptString.Length)
            {
                // Get command. ex: p, t, s
                command = scriptString[index];
                index++;

                print("Command: " + command);

                // Get value. ex: 1.1, 5, 0
                string valueString = scriptString.Substring(index, Mathf.Abs(scriptString.IndexOf(';') - index));
                value = float.Parse(valueString);

                index += valueString.Length + 1;

                print("ValueStr: " + valueString);

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

        StartCoroutine(ScriptCoroutine());
    }

    
    public void SetScript(string scriptString)
    {
        this.scriptString = scriptString;
    }
}
