using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public List<string> AsyncScenesName;
    public List<UnityEngine.AsyncOperation> AsyncScenes = new List<UnityEngine.AsyncOperation>();
    public bool Updated;

    private void Update()
    {
        if (!Updated)
        {
            for (int i = 0; i < AsyncScenesName.Count; i++)
            {
                AsyncScenes.Add(SceneManager.LoadSceneAsync(AsyncScenesName[i]));
                AsyncScenes[i].allowSceneActivation = false;
            }

            Updated = true;
        }
    }

    public async void LoadAsyncScene(string SceneName)
    {
        var scene = SceneManager.LoadSceneAsync(SceneName);
        scene.allowSceneActivation = false;
    }

    public async void LoadScene(int TargetSceneID)
    {
        if (TargetSceneID >= 0 && TargetSceneID < AsyncScenes.Count)
        {
            do
            {
                await Task.Delay(100);
            } while (AsyncScenes[TargetSceneID].progress < 0.9f);

            AsyncScenes[TargetSceneID].allowSceneActivation = true;
            Updated = false;
        }
    }
}
