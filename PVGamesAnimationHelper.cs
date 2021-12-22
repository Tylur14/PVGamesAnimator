using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
[Serializable] public class OnLayerChanged : UnityEvent<String> { }
public class PVGamesAnimationHelper : MonoBehaviour
{

    public enum DataRetrieveTypes
    {
        ANIM,
        RESOURCE
    }

    public enum FunctionalTypes
    {
        LOADER,
        CREATOR,
        LAYER_CONTROLLER
    }

    #region Variables-------------------------------------------------------------------------------------

    [Header("Helper Settings")] [SerializeField]
    private DataRetrieveTypes dataRetrieveType;
    [SerializeField] private FunctionalTypes functionalType;
    [SerializeField] private OnLayerChanged onValueChanged = null;

    [Header("Controls")] [SerializeField] private Button loadFileButton;
    [SerializeField] private Button createFileButton;
    [SerializeField] private TMP_InputField fileNameInput;
    [SerializeField] private Button[] layerControls;

    [Header("Displays")] [SerializeField] private Transform contentHolder;
    [SerializeField] private GameObject contentItemPrefab;

    [Header("New File Data")] [SerializeField]
    private string newAnimationName;
    [SerializeField] private string currentFileTarget;
    public string CurrentFileTarget
    {
        get { return currentFileTarget; }
        set
        {
            currentFileTarget = value;
            onValueChanged.Invoke(currentFileTarget);
        }
    }

    [Header("Paths")] [SerializeField] private string targetPath = "Assets/MythosAnimator/Animations/";
    private TextMeshProUGUI lastSelectedResource;
    public bool active;

    
    
    #endregion

    #region File Functions-------------------------------------------------------------------------------------

    void LoadAnimations()
    {
        for (int i = 0; i < contentHolder.childCount; i++)
        {
            Destroy(contentHolder.GetChild(i).gameObject);
        }

        var resources = GetAllInstances<PVGamesAnimationSheet>();
        foreach (var r in resources)
        {
            var obj = Instantiate(contentItemPrefab, contentHolder);
            obj.GetComponent<TextMeshProUGUI>().text = r.name;
            obj.GetComponent<Button>().onClick.AddListener(delegate { PickFile(obj); });
        }
    }

    void LoadResourceFiles()
    {
        for (int i = 0; i < contentHolder.childCount; i++)
        {
            Destroy(contentHolder.GetChild(i).gameObject);
        }

        var resources = Resources.LoadAll("Mythos", typeof(Texture2D));
        foreach (var r in resources)
        {
            var obj = Instantiate(contentItemPrefab, contentHolder);
            obj.GetComponent<TextMeshProUGUI>().text = r.name;
            obj.GetComponent<Button>().onClick.AddListener(delegate { PickFile(obj); });
        }
    }

    public void CreateNewAnimation()
    {
        var newAnimation = ScriptableObject.CreateInstance(typeof(PVGamesAnimationSheet));
        newAnimation.name = newAnimationName;

        var info = newAnimation.GetType().GetField("ID");
        info.SetValue(newAnimation, CurrentFileTarget);
        EditorUtility.SetDirty(newAnimation);
        string output = $"{targetPath}{newAnimation.name}.asset";
        AssetDatabase.CreateAsset(newAnimation, output);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        var anim = AssetDatabase.LoadAssetAtPath<PVGamesAnimationSheet>(output);
        anim.stopIndex = 1;
        anim.frameCount = 8;
        FindObjectOfType<PVGamesAnimationMaker>().incomingAnimation = anim;
    }

    public void PickFile(GameObject obj)
    {
        if (obj == null) return;
        var t = obj.GetComponent<TextMeshProUGUI>();
        CurrentFileTarget = t.text;
        if (lastSelectedResource != null)
            lastSelectedResource.color = Color.black;
        t.color = Color.blue;
        lastSelectedResource = t;
    }

    public void ConfirmSelection()
    {
        string output = $"{targetPath}{CurrentFileTarget}.asset";
        var anim = AssetDatabase.LoadAssetAtPath<PVGamesAnimationSheet>(output);
        FindObjectOfType<PVGamesAnimationMaker>().incomingAnimation = anim;
    }

    #endregion

    #region Update Functions-------------------------------------------------------------------------------------

    private void Update()
    {
        if (active)
            UpdateControlsDisplay();
    }

    void UpdateControlsDisplay()
    {
        if (functionalType == FunctionalTypes.LOADER)
            loadFileButton.interactable = CurrentFileTarget != string.Empty;

        else if (functionalType == FunctionalTypes.CREATOR)
        {
            createFileButton.interactable =
                (newAnimationName != string.Empty && CurrentFileTarget != string.Empty);
        }
        else if (functionalType == FunctionalTypes.LAYER_CONTROLLER)
        {
            foreach (var c in layerControls)
                c.interactable = CurrentFileTarget != string.Empty;
        }

    }

    public void ClearDisplay()
    {
        currentFileTarget = "";
        newAnimationName = "";
        for (int i = 0; i < contentHolder.childCount; i++)
        {
            Destroy(contentHolder.GetChild(i).gameObject);
        }
    }
    
    public void UpdateLayerDisplay(PVGamesAnimationSheet[] displayInfo,int selected)
    {
        
        ClearDisplay();
        for (var index = 0; index < displayInfo.Length; index++)
        {
            var r = displayInfo[index];
            var obj = Instantiate(contentItemPrefab, contentHolder);
            obj.GetComponent<TextMeshProUGUI>().text = r.name;
            obj.GetComponent<Button>().onClick.AddListener(delegate { PickFile(obj); });
            if (index == selected)
                obj.GetComponent<Button>().onClick.Invoke();
        }
    }

    public void UpdateNewFileName(string incomingName)
    {
        newAnimationName = incomingName;
    }

    #endregion

    #region Helpers-------------------------------------------------------------------------------------

    private void Toggle(bool state)
    {
        active = state;
        gameObject.SetActive(state);
        ClearSelection();
        if (fileNameInput != null) fileNameInput.text = newAnimationName = "";
        if (state && functionalType != FunctionalTypes.LAYER_CONTROLLER)
        {
            if (dataRetrieveType == DataRetrieveTypes.ANIM)
                LoadAnimations();
            else if (dataRetrieveType == DataRetrieveTypes.RESOURCE)
                LoadResourceFiles();
        }
    }

    public void ClearSelection()
    {
        CurrentFileTarget = "";
    }

    static T[] GetAllInstances<T>() where T : ScriptableObject
    {
        string[] guids = AssetDatabase.FindAssets("t:"+ typeof(T).Name);  //FindAssets uses tags check documentation for more info
        T[] a = new T[guids.Length];
        for(int i =0;i<guids.Length;i++)         //probably could get optimized 
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
        }
 
        return a;
    }
    
    private void OnEnable()
    {
        Toggle(true);
    }

    #endregion
    
    

    
}
