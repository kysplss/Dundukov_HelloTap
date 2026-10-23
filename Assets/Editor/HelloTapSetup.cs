#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Автоматически собирает сцену Hello Tap при первом открытии проекта.
/// Пересобрать вручную: Tools → Hello Tap → Build Scene.
/// </summary>
[InitializeOnLoad]
public static class HelloTapSetup
{
    private const string ScenePath = "Assets/Scenes/Main.unity";

    static HelloTapSetup()
    {
        // Ждём конца компиляции/импорта, затем собираем сцену, если её ещё нет
        EditorApplication.delayCall += () =>
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            if (!File.Exists(ScenePath))
            {
                BuildScene();
                ConfigureAndroid();
            }
        };
    }

    [MenuItem("Tools/Hello Tap/Build Scene")]
    public static void BuildScene()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
            AssetDatabase.CreateFolder("Assets", "Scenes");

        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Камера (нужна, чтобы не было предупреждения "No cameras rendering")
        var camGo = new GameObject("Main Camera") { tag = "MainCamera" };
        var cam = camGo.AddComponent<Camera>();
        cam.orthographic = true;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.10f, 0.12f, 0.18f);
        camGo.transform.position = new Vector3(0, 0, -10);
        camGo.AddComponent<AudioListener>();

        // Canvas + Canvas Scaler (Scale With Screen Size, 1080x1920, Match 0.5)
        var canvasGo = new GameObject("Canvas");
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 0.5f;
        canvasGo.AddComponent<GraphicRaycaster>();

        // EventSystem (нужен для работы кнопки)
        var esGo = new GameObject("EventSystem");
        esGo.AddComponent<EventSystem>();
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        esGo.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
#else
        esGo.AddComponent<StandaloneInputModule>();
#endif

        Font font = GetFont();

        // Заголовок (top-center)
        MakeText("TitleText", canvasGo.transform, font, "Hello Tap", 110,
            new Vector2(0.5f, 1f), new Vector2(0, -200), new Vector2(900, 200));

        // Счёт (center)
        var scoreText = MakeText("ScoreText", canvasGo.transform, font, "Счёт: 0", 130,
            new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(900, 250));

        // Кнопка (bottom-center)
        var btnGo = new GameObject("TapButton", typeof(RectTransform));
        btnGo.transform.SetParent(canvasGo.transform, false);
        var btnRt = btnGo.GetComponent<RectTransform>();
        btnRt.anchorMin = btnRt.anchorMax = btnRt.pivot = new Vector2(0.5f, 0f);
        btnRt.anchoredPosition = new Vector2(0, 250);
        btnRt.sizeDelta = new Vector2(700, 220);
        var btnImg = btnGo.AddComponent<Image>();
        btnImg.color = new Color(0.2f, 0.6f, 0.9f);
        var button = btnGo.AddComponent<Button>();
        button.targetGraphic = btnImg;
        MakeText("Text", btnGo.transform, font, "Тапни меня", 80,
            new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(700, 220));

        // Скрипт TapCounter на Canvas, поле Score Text, OnClick кнопки
        var counter = canvasGo.AddComponent<TapCounter>();
        var so = new SerializedObject(counter);
        so.FindProperty("scoreText").objectReferenceValue = scoreText;
        so.ApplyModifiedPropertiesWithoutUndo();
        UnityEventTools.AddPersistentListener(button.onClick, counter.Increment);

        // Сохраняем сцену и добавляем в Build Settings
        EditorSceneManager.SaveScene(scene, ScenePath);
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
        Debug.Log("Hello Tap: сцена собрана (" + ScenePath + "). Нажмите Play.");
    }

    [MenuItem("Tools/Hello Tap/Configure Android Settings")]
    public static void ConfigureAndroid()
    {
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
        PlayerSettings.productName = "Hello Tap";
        PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, "com.student.hellotap");
        Debug.Log("Android: портретная ориентация, package com.student.hellotap " +
                  "(замените student на свою фамилию латиницей).");
    }

    // ---------- вспомогательные методы ----------

    private static Font GetFont()
    {
        // Встроенный шрифт с поддержкой кириллицы
        Font f = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (f == null) f = Resources.GetBuiltinResource<Font>("Arial.ttf");
        return f;
    }

    private static Text MakeText(string name, Transform parent, Font font, string text, int size,
        Vector2 anchor, Vector2 pos, Vector2 dim)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = rt.pivot = anchor;
        rt.anchoredPosition = pos;
        rt.sizeDelta = dim;
        var t = go.AddComponent<Text>();
        t.font = font;
        t.text = text;
        t.fontSize = size;
        t.alignment = TextAnchor.MiddleCenter;
        t.color = Color.white;
        t.horizontalOverflow = HorizontalWrapMode.Overflow;
        t.verticalOverflow = VerticalWrapMode.Overflow;
        t.raycastTarget = false;
        return t;
    }
}
#endif
