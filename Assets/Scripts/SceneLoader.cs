using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    List<Image> NeutralTransitionImages;

    bool loadDone;

    bool loadLock = false;
    
    private void Awake()
    {
        SetUpSingleton();
        NeutralTransitionImages = new List<Image>(GetComponentsInChildren<Image>());
    }

    private void SetUpSingleton()
    {
        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    public void LoadScene(int index)
    {
        if (!loadLock) StartCoroutine(SceneLoadingNeutral(index));
    }

    public void ReloadScene()
    {
        if (!loadLock) StartCoroutine(SceneLoadingNeutral(SceneManager.GetActiveScene().buildIndex));
    }

    public void ReturnToMenu()
    {
        if (!loadLock) LoadScene(0);
    }

    private IEnumerator SceneLoadingNeutral(int index)
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        loadLock = true;
        //Debug.Log("Transition Start");
        do
        {
            foreach (Image i in NeutralTransitionImages)
            {
                i.fillAmount += Time.unscaledDeltaTime * 2;
            }
            yield return new WaitForEndOfFrame();
        } while (NeutralTransitionImages[0].fillAmount < 1);
        //Debug.Log("Transition Middle");
        SceneManager.LoadScene(index);
        yield return new WaitUntil(LoadDone);
        do
        {
            foreach (Image i in NeutralTransitionImages)
            {
                i.fillAmount -= Time.unscaledDeltaTime * 2;
            }
            yield return new WaitForEndOfFrame();
        } while (NeutralTransitionImages[0].fillAmount > 0);
        //Debug.Log("Transition End");
        loadLock = false;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        loadDone = true;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    bool LoadDone() { return loadDone; }

}
