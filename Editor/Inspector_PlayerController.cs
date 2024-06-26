using marianateixeira.PlayerController;
using System.Diagnostics;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomEditor(typeof(PlayerController))]
public class Inspector_PlayerController : Editor
{
    PlayerController playerController;

    private void OnEnable()
    {
        playerController = (PlayerController)target;
    }
    private void OnDisable()
    {
        playerController = null;
    }

    public override VisualElement CreateInspectorGUI()
    {
        var root = new VisualElement();

        PropertyField move = new PropertyField();
        move.bindingPath = "Move";
        move.SetEnabled(false);

        PropertyField mask = new PropertyField() { label = "Collision Mask" };
        mask.bindingPath = "_platformMask";

        PropertyField data = new PropertyField();
        data.bindingPath = "Data";
        data.RegisterCallback<SerializedPropertyChangeEvent>(x => playerController.RecalculatePhysics(), TrickleDown.TrickleDown);

        Button saveData = new Button() { text = "Save Data" };
        saveData.clicked += playerController.SaveData;

        root.Add(move);
        root.Add(mask);
        root.Add(data);
        root.Add(saveData);
        return root;
    }
}