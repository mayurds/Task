
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class BoxSpawner : MonoBehaviour
	{
    public Tile[] tilesToSpawn;
    public Vector3[] spawnPos;
        float lasttime;
    int i = 0;
    public void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
  
    private void Update()
		{
		    if((Time.timeSinceLevelLoad - lasttime)>1)
			{
          
            if (i >= spawnPos.Length)
            {
                i = 0;
            }
				var spawnpos = spawnPos[i];
				var spawnTile = tilesToSpawn[Random.Range(0, tilesToSpawn.Length)];
                var tile = Instantiate(spawnTile) as Tile;
				tile.name = "Tile " ;
                tile.transform.position = spawnpos;
				tile.gameObject.SetActive(true);
           
            i++;
            lasttime = Time.timeSinceLevelLoad;
         
			}
		}
	}
