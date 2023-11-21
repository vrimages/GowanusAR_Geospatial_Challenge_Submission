using System.Collections.Generic;
using System.IO;
using System.Linq;
using TinyGiantStudio.Text.FontCreation;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TinyGiantStudio.Text.EditorFiles
{
    public class ModularTextEditorWindow : EditorWindow
    {
        #region Variable declaration

        [SerializeField] private VisualTreeAsset visualTreeAsset;

        #region Theme

        private bool _lightMode = false;

        private bool lightMode
        {
            get { return _lightMode; }
            set
            {
                _lightMode = value;
                UpdateTheme();
            }
        }

        private GroupBox rootHolder;
        private UnityEngine.UIElements.Button darkModeButton;
        private UnityEngine.UIElements.Button lightModeButton;
        private VisualElement themeModeBackgroundButton;

        #endregion Theme

        #region Font creation variables

        [SerializeField] private VisualTreeAsset previewTemplate;

        private GroupBox createFontContent;
        private UnityEngine.UIElements.Button createFontButton;
        private Foldout previewFoldout;
        private ScrollView previewScrollview;

        private CharacterGenerator _characterGenerator;

        private CharacterGenerator CharacterGenerator
        {
            get
            {
                if (_characterGenerator == null) _characterGenerator = new CharacterGenerator();

                return _characterGenerator;
            }
        }

        [SerializeField] private string _filePath;

        private string FilePath
        {
            set
            {
                _filePath = value;
                LoadFileContent();
                GetFontAsset();
                UpdateFontInformation();
                Kerning();

                if (value != null && value.Length > 0)
                {
                    createFontButton.SetEnabled(true);
                    previewFoldout.style.display = DisplayStyle.Flex;
                    CleanUpdatePreviews();
                }
                else
                {
                    NoFileSeletected();
                }
            }
            get { return _filePath; }
        }

        private void NoFileSeletected()
        {
            previewFoldout.style.display = DisplayStyle.None;
            createFontButton.SetEnabled(false);
        }

        private byte[] fileContent;
        public UnityEngine.Font font;

        private AssetSettings _settings;

        private int previewCount = 1;
        public List<Preview> previews = new List<Preview>();

        public class Preview
        {
            public Mesh mesh;
            public MeshPreview meshPreview;
            public IMGUIContainer previewContainer;
            public IMGUIContainer previewSettingsContainer;
            public TextField previewCharacterField;
        }

        [SerializeField]
        private AssetSettings Settings
        {
            get
            {
                _settings = StaticMethods.VerifySettings(_settings);
                return _settings;
            }
        }

        private SerializedObject soTarget;

        private readonly string kerningDocumentationDocURL = "https://ferdowsur.gitbook.io/modular-3d-text/text/fonts/creating-fonts/troubleshoot#no-kerning-found";
        private readonly string fontCreationTroubleShootDocURL = "https://ferdowsur.gitbook.io/modular-3d-text/fonts/creating-fonts/troubleshoot";

        #endregion Font creation variables

        #endregion Variable declaration

        #region Unity Things

        [UnityEditor.MenuItem("Tools/Tiny Giant Studio/Modular 3D Text (New)")]
        public static void ShowWindow()
        {
            ModularTextEditorWindow wnd = GetWindow<ModularTextEditorWindow>();

            Texture titleIcon = EditorGUIUtility.Load("Assets/Plugins/Tiny Giant Studio/Modular 3D Text/Utility/Editor Icons/M3D.png") as Texture;
            if (titleIcon)
                wnd.titleContent = new GUIContent("Modular 3D Text", titleIcon);
            else
                wnd.titleContent = new GUIContent("Modular 3D Text", titleIcon);

            wnd.minSize = new Vector2(400, 600);
        }

        private void OnDisable()
        {
            CleanupPreviews();
        }

        private void OnDestroy()
        {
            CleanupPreviews();
        }

        public void CreateGUI()
        {
            visualTreeAsset.CloneTree(rootVisualElement);

            SetupHeader();

            SetupFontCreationTab();

            SetupFooter();

            #region resume work session after unity session reset after something like a compile script complete

            UpdateFontInformation();

            if (fileContent != null && fileContent.Length > 0)
            {
                createFontButton.SetEnabled(true);
                previewFoldout.style.display = DisplayStyle.Flex;
                CleanUpdatePreviews();
            }
            else
            {
                NoFileSeletected();
            }

            #endregion resume work session after unity session reset after something like a compile script complete
        }

        #endregion Unity Things

        #region Header

        private void SetupHeader()
        {
            string assetLink = "https://bit.ly/Modular3DTextUnityAsset";
            string versionLink = "https://assetstore.unity.com/packages/3d/gui/modular-3d-text-in-game-3d-ui-system-247241?aid=1011ljxWe#releases";

            rootHolder = rootVisualElement.Q<GroupBox>("RootHolder");
            GroupBox header = rootHolder.Q<GroupBox>("Header");

            SetupFooterURLButton(header, "AssetIconButton", assetLink);
            SetupFooterURLButton(header, "AssetNameButton", assetLink);
            SetupFooterURLButton(header, "CheckUpdateButton", versionLink);

            darkModeButton = header.Q<UnityEngine.UIElements.Button>("DarkModeButton");
            darkModeButton.clicked += () => lightMode = true;
            lightModeButton = header.Q<UnityEngine.UIElements.Button>("LightModeButton");
            lightModeButton.clicked += () => lightMode = false;

            themeModeBackgroundButton = header.Q<VisualElement>("ThemeColor");

            UpdateTheme();
        }

        private void UpdateTheme()
        {
            if (lightMode)
            {
                darkModeButton.style.opacity = 0;
                lightModeButton.style.opacity = 1;
                themeModeBackgroundButton.style.backgroundColor = new Color(0.75f, 0.7f, 0.7f, 0.5f);
                rootHolder.style.backgroundColor = new Color(0.6f, 0.6f, 0.6f);

                ChangeAllFieldColor(new Color(0.4f, 0.375f, 0.3f, 0.5f));
                ChangeAllHardBackgroundColor(new Color(0.4f, 0.375f, 0.3f, 0.5f));
                ChangeAllTextButtonColor(new Color(0.05f, 0.01f, 0.0f, 1f));
                ChangeAllLabelColor(new Color(0.1f, 0.1f, 0.1f, 0.9f));
            }
            else
            {
                darkModeButton.style.opacity = 1;
                lightModeButton.style.opacity = 0;
                themeModeBackgroundButton.style.backgroundColor = new Color(0.2f, 0.2f, 0.4f, 0.5f);
                rootHolder.style.backgroundColor = new Color(0.2196f, 0.2196f, 0.2196f);

                ChangeAllFieldColor(new Color(1f, 0.06f, 0.07f, 0.75f));
                ChangeAllHardBackgroundColor(new Color(0.05f, 0.06f, 0.07f, 0.75f));
                ChangeAllTextButtonColor(new Color(0.7f, 0.8f, 1f, 1f));
                ChangeAllLabelColor(new Color(0.9f, 0.9f, 0.8f, 0.7f));
            }
        }

        private void ChangeAllFieldColor(Color color)
        {
            List<TextField> intFields = rootHolder.Query<TextField>(className: "unity-base-text-field__input").ToList();
            foreach (TextField background in intFields)
            {
                Debug.Log(background);
                background.style.backgroundColor = color;
            }
        }

        private void ChangeAllHardBackgroundColor(Color color)
        {
            List<GroupBox> backgrounds = rootHolder.Query<GroupBox>(className: "HardBackground").ToList();
            foreach (GroupBox background in backgrounds)
            {
                background.style.backgroundColor = color;
            }
        }

        private void ChangeAllLabelColor(Color color)
        {
            List<Label> labels = rootHolder.Query<Label>(className: "unity-label").ToList();
            foreach (Label label in labels)
            {
                label.style.color = color;
            }
        }

        private void ChangeAllTextButtonColor(Color color)
        {
            List<UnityEngine.UIElements.Button> textButtons = rootHolder.Query<UnityEngine.UIElements.Button>(className: "textButton").ToList();
            foreach (UnityEngine.UIElements.Button textButton in textButtons)
            {
                textButton.style.color = color;
            }
        }

        #endregion Header

        #region Font Creation

        private void SetupFontCreationTab()
        {
            createFontContent = rootVisualElement.Q<GroupBox>("CreateFontContent");

            previewFoldout = createFontContent.Q<Foldout>("PreviewFoldout");
            previewScrollview = createFontContent.Q<ScrollView>("PreviewScrollView");

            var selectFileButton = createFontContent.Q<UnityEngine.UIElements.Button>("SelectFileButton");
            selectFileButton.clicked += () => GetTTFFile();

            createFontButton = createFontContent.Q<UnityEngine.UIElements.Button>("CreateFontButton");
            createFontButton.SetEnabled(false);
            createFontButton.clicked += () => CreateFont();

            var resetMeshSettingsButton = createFontContent.Q<UnityEngine.UIElements.Button>("ResetMeshSettingsButton");
            resetMeshSettingsButton.clicked += () => Settings.ResetFontCreationMeshSettings();

            var resetPrebuiltCharactersButton = createFontContent.Q<UnityEngine.UIElements.Button>("ResetPrebuiltCharactersButton");
            resetPrebuiltCharactersButton.clicked += () => Settings.ResetFontCreationPrebuiltSettings();

            var LogPrebuiltCharactersButton = createFontContent.Q<UnityEngine.UIElements.Button>("LogPrebuiltCharactersButton");
            LogPrebuiltCharactersButton.clicked += () => TestCharacterList();

            var KerningDocumentation = createFontContent.Q<UnityEngine.UIElements.Button>("KerningDocumentation");
            KerningDocumentation.clicked += () => Application.OpenURL(kerningDocumentationDocURL);

            var CommonIssuesButton = createFontContent.Q<UnityEngine.UIElements.Button>("CommonIssuesButton");
            CommonIssuesButton.clicked += () => Application.OpenURL(fontCreationTroubleShootDocURL);

            NoFileSeletected();

            if (soTarget == null)
                soTarget = new SerializedObject(Settings);

            var styleEnumField = createFontContent.Q<EnumField>("StyleEnumField");
            styleEnumField.BindProperty(soTarget.FindProperty(nameof(AssetSettings.meshExportStyle)));

            var vartexDensityField = createFontContent.Q<IntegerField>("VertexDensityField");
            vartexDensityField.BindProperty(soTarget.FindProperty(nameof(AssetSettings.vertexDensity)));
            vartexDensityField.RegisterValueChangedCallback(evt =>
            {
                if (Settings.vertexDensity < 1) Settings.vertexDensity = 1;
                UpdateMeshSettingsNote();
                UpdatePreviews();
            });

            var smoothingAngleField = createFontContent.Q<IntegerField>("SmoothingAngleField");
            smoothingAngleField.BindProperty(soTarget.FindProperty(nameof(AssetSettings.smoothingAngle)));
            smoothingAngleField.RegisterValueChangedCallback(evt =>
            {
                if (Settings.smoothingAngle < 0) Settings.smoothingAngle = 0;
                UpdateMeshSettingsNote();
                UpdatePreviews();
            });

            var sizeXYField = createFontContent.Q<FloatField>("SizeXYField");
            sizeXYField.BindProperty(soTarget.FindProperty(nameof(AssetSettings.sizeXY)));
            sizeXYField.RegisterValueChangedCallback(evt =>
            {
                if (Settings.sizeXY <= 0) Settings.sizeXY = 1;
                UpdateMeshSettingsNote();
                UpdatePreviews();
            });

            var sizeZField = createFontContent.Q<FloatField>("SizeZField");
            sizeZField.BindProperty(soTarget.FindProperty(nameof(AssetSettings.sizeZ)));
            sizeZField.RegisterValueChangedCallback(evt =>
            {
                if (Settings.sizeZ <= 0) Settings.sizeZ = 1;
                UpdateMeshSettingsNote();
                UpdatePreviews();
            });

            var PreviewAmount = createFontContent.Q<IntegerField>("PreviewAmount");
            PreviewAmount.BindProperty(soTarget.FindProperty(nameof(AssetSettings.previewAmount)));
            PreviewAmount.RegisterValueChangedCallback(evt =>
            {
                previewCount = evt.newValue;
                UpdatePreviews();
            });
            previewCount = PreviewAmount.value;

            UpdateMeshSettingsNote();

            var charInputStyle = createFontContent.Q<EnumField>("CharInputStyle");
            charInputStyle.BindProperty(soTarget.FindProperty(nameof(AssetSettings.charInputStyle)));
            charInputStyle.RegisterValueChangedCallback(evt =>
            {
                UpdatePrebuiltCharacterFields();
            });
            var startPrebuiltCharacter = createFontContent.Q<TextField>("StartPrebuiltCharacter");
            startPrebuiltCharacter.BindProperty(soTarget.FindProperty(nameof(AssetSettings.startChar)));

            var endPrebuiltCharacter = createFontContent.Q<TextField>("EndPrebuiltCharacter");
            endPrebuiltCharacter.BindProperty(soTarget.FindProperty(nameof(AssetSettings.endChar)));

            var startUnicode = createFontContent.Q<TextField>("StartUnicode");
            startUnicode.BindProperty(soTarget.FindProperty(nameof(AssetSettings.startUnicode)));

            var endUnicode = createFontContent.Q<TextField>("EndUnicode");
            endUnicode.BindProperty(soTarget.FindProperty(nameof(AssetSettings.endUnicode)));

            var customCharacters = createFontContent.Q<TextField>("CustomCharacters");
            customCharacters.BindProperty(soTarget.FindProperty(nameof(AssetSettings.customCharacters)));

            var unicodeSequence = createFontContent.Q<TextField>("UnicodeSequence");
            unicodeSequence.BindProperty(soTarget.FindProperty(nameof(AssetSettings.unicodeSequence)));
            UpdatePrebuiltCharacterFields();
        }

        private void UpdateMeshSettingsNote()
        {
            TemplateContainer templateContainer = createFontContent.Q<TemplateContainer>("MeshSettingsNote");
            templateContainer.style.display = DisplayStyle.None;
            var headLine = templateContainer.Q<Label>("Headline");
            headLine.text = "";
            headLine.tooltip = "";
            var details = templateContainer.Q<Label>("Details");
            details.text = "";

            if (Settings.vertexDensity > 5)
            {
                templateContainer.style.display = DisplayStyle.Flex;
                string detailsText = "A high value can introduce a lot of problems including performance issue by creating too much vertices, intersecting faces etc." +
                    "\nThat's why, it is recommended to check the preview and use the lowest number that gives acceptable result." +
                    "\nThe default fonts are created with 1 density.";

                headLine.text += "Tip: Keep vertex density low.";
                headLine.tooltip += detailsText;
                details.text += detailsText;
            }
        }

        private void GetTTFFile()
        {
            FilePath = EditorUtility.OpenFilePanel("Select TTF font", "Assets", "ttf");

            //typeFace = new TypeFace(fileContent);
        }

        private void CreateFont()
        {
            EditorUtility.DisplayProgressBar("Creating font", "Preparing", 0 / 100);
            bool exportAsObj = ExportAs();
            List<char> listOfChar = GetCharacterList();

            GameObject fontHolderObject = new GameObject();

            EditorUtility.DisplayProgressBar("Creating font", "Creating pre-built meshes", 10 / 100);

            CharacterGenerator.GetFontObject(fontHolderObject, fileContent, listOfChar, Settings.sizeXY, Settings.sizeZ, Settings.vertexDensity, Settings.smoothingAngle, Settings.defaultTextMaterial);
            fontHolderObject.name = CharacterGenerator.typeFace.nameEntry;
            EditorUtility.DisplayProgressBar("Creating font", "Creating pre-built meshes", 10 / 100);

            if (fontHolderObject.transform.childCount > 0)
            {
                if (exportAsObj)
                {
                    MText_ObjExporter objExporter = new MText_ObjExporter();
                    string prefabPath = objExporter.DoExport(fontHolderObject, true);
                    if (string.IsNullOrEmpty(prefabPath))
                    {
                        Debug.Log("Object save failed");
                        EditorUtility.ClearProgressBar();
                        return;
                    }
                    MText_FontExporter fontExporter = new MText_FontExporter();
                    fontExporter.CreateFontFile(prefabPath, fontHolderObject.name, CharacterGenerator, fileContent);
                }
                else
                {
                    MText_MeshAssetExporter meshAssetExporter = new MText_MeshAssetExporter();
                    meshAssetExporter.DoExport(fontHolderObject);
                }
            }

            EditorUtility.ClearProgressBar();
            if (Application.isPlaying) Destroy(fontHolderObject);
            else DestroyImmediate(fontHolderObject);
        }

        #region TTF Data

        private void LoadFileContent()
        {
            if (!string.IsNullOrEmpty(FilePath))
                fileContent = File.ReadAllBytes(FilePath);
        }

        private void GetFontAsset()
        {
            if (FilePath == null) return;
            if (FilePath.Length == 0) return;

            if (FilePath.StartsWith(Application.dataPath))
            {
                string relativepath = "Assets" + FilePath.Substring(Application.dataPath.Length);
                try
                {
                    font = (UnityEngine.Font)AssetDatabase.LoadAssetAtPath(relativepath, typeof(UnityEngine.Font));
                }
                catch
                {
                }
            }
        }

        private void UpdateFontInformation()
        {
            if (FilePath == null)
            {
                HideFoldouts();
                return;
            }
            if (FilePath.Length == 0)
            {
                HideFoldouts();
                return;
            }

            CharacterGenerator.CreateTypeFace(fileContent);

            if (CharacterGenerator.typeFace == null)
            {
                HideFoldouts();
                return;
            }

            ShowFoldouts();

            createFontContent.Q<Label>("FontNameLabel").text = CharacterGenerator.typeFace.nameEntry;
            createFontContent.Q<Label>("FontLocationLabel").text = "File location : " + FilePath;
            if (CharacterGenerator.typeFace.kernTable != null)
                createFontContent.Q<Label>("FontKernCountLabel").text = "" + CharacterGenerator.typeFace.kernTable.Count;
            else
                createFontContent.Q<Label>("FontKernCountLabel").text = "0";
        }

        private void HideFoldouts()
        {
            rootHolder.Q<Foldout>("InformationFoldout").style.display = DisplayStyle.None;
            rootHolder.Q<Foldout>("MeshSettingsFoldout").style.display = DisplayStyle.None;
            rootHolder.Q<Foldout>("PrebuiltCharactersFoldout").style.display = DisplayStyle.None;
        }

        private void ShowFoldouts()
        {
            rootHolder.Q<Foldout>("InformationFoldout").style.display = DisplayStyle.Flex;
            rootHolder.Q<Foldout>("MeshSettingsFoldout").style.display = DisplayStyle.Flex;
            rootHolder.Q<Foldout>("PrebuiltCharactersFoldout").style.display = DisplayStyle.Flex;
        }

        #endregion TTF Data

        #region Export Settings

        private bool ExportAs()
        {
            return true;
        }

        #endregion Export Settings

        #region Preview

        private void CleanUpdatePreviews()
        {
            CleanupPreviews();

            UpdatePreviews();
        }

        private void UpdatePreviews()
        {
            if (previewCount < 1) previewCount = 1;
            else if (previewCount > 50) previewCount = 50;

            if (previews.Count > previewCount)
            {
                //for (int i = previews.Count - 1; i > previewCount - 1; i--)
                //{
                //    Debug.Log(i);
                //    //CleanUpPreview(previews[i]); //this crashes the editor
                //    //previews.Remove(previews[i]);
                //}
                CleanupPreviews();
                if (previewCount < 1) previewCount = 1;
            }

            if (previews.Count < previewCount)
            {
                for (int i = previews.Count; i < previewCount; i++)
                {
                    previews.Add(new Preview());
                }
            }

            for (int i = 0; i < previews.Count; i++)
            {
                UpdatePreview(previews[i]);
            }
        }

        private void UpdatePreview(Preview preview)
        {
            if (preview.previewContainer == null)
            {
                VisualElement previewHolder = new VisualElement();
                previewScrollview.Add(previewHolder);
                previewTemplate.CloneTree(previewHolder);

                preview.previewContainer = previewHolder.Q<IMGUIContainer>("PreviewContainer");
                preview.previewSettingsContainer = previewHolder.Q<IMGUIContainer>("PreviewSettingsContainer");
                preview.previewCharacterField = previewHolder.Q<TextField>("PreviewCharacterField");
                preview.previewCharacterField.RegisterValueChangedCallback(evt =>
                {
                    UpdatePreview(preview);
                });
            }

            char c;

            if (preview.previewCharacterField.text == null)
                c = 'O';
            else if (preview.previewCharacterField.text.Length <= 0)
                c = 'A';
            else
                c = preview.previewCharacterField.text.ToCharArray()[0];

            preview.mesh = CharacterGenerator.GetMesh(fileContent, Settings.sizeXY, Settings.sizeZ, Settings.smoothingAngle, 0, Settings.vertexDensity, c);
            if (preview.mesh == null) return;

            //preview.meshPreview?.Dispose();
            if (preview.meshPreview == null)
                preview.meshPreview = new MeshPreview(preview.mesh);
            else
                preview.meshPreview.mesh = (preview.mesh);

            preview.previewContainer.Q<Label>("ModelInformationLabel").text = MeshPreview.GetInfoString(preview.mesh);
            //preview.previewContainer.Q<Label>("ModelInformationLabel").text = "Vertex: " + preview.mesh.vertexCount + " Triangles: " + preview.mesh.triangles.Count();

            preview.previewContainer.onGUIHandler = null;
            preview.previewContainer.onGUIHandler += () =>
            {
                preview.meshPreview.OnPreviewGUI(preview.previewContainer.contentRect, null);
            };
            preview.previewSettingsContainer.onGUIHandler = null;
            preview.previewSettingsContainer.onGUIHandler += () =>
            {
                EditorGUILayout.BeginHorizontal();
                preview.meshPreview.OnPreviewSettings();
                EditorGUILayout.EndHorizontal();
            };
        }

        private void CleanupPreviews()
        {
            for (int i = 0; i < previews.Count; i++)
            {
                CleanUpPreview(previews[i]);
            }

            previews.Clear();
            if (previewScrollview != null)
                previewScrollview.Clear();
        }

        private void CleanUpPreview(Preview preview)
        {
            preview.meshPreview?.Dispose();
            preview.previewContainer?.Dispose();
            preview.previewSettingsContainer?.Dispose();

            try
            {
                if (preview.mesh)
                {
                    if (!Application.isPlaying)
                        DestroyImmediate(preview.mesh);
                    else
                        Destroy(preview.mesh);
                }
            }
            catch { }
        }

        #endregion Preview

        #region Prebuilt characters

        private void UpdatePrebuiltCharacterFields()
        {
            var characterRangeSelection = createFontContent.Q<GroupBox>("CharacterRangeSelection");
            characterRangeSelection.style.display = DisplayStyle.None;

            var unicodeRangeSelection = createFontContent.Q<GroupBox>("UnicodeRangeSelection");
            unicodeRangeSelection.style.display = DisplayStyle.None;

            var customCharacters = createFontContent.Q<TextField>("CustomCharacters");
            customCharacters.style.display = DisplayStyle.None;

            var unicodeSequence = createFontContent.Q<TextField>("UnicodeSequence");
            unicodeSequence.style.display = DisplayStyle.None;

            switch (Settings.charInputStyle)
            {
                case AssetSettings.CharInputStyle.CharacterRange:
                    characterRangeSelection.style.display = DisplayStyle.Flex;
                    break;

                case AssetSettings.CharInputStyle.UnicodeRange:
                    unicodeRangeSelection.style.display = DisplayStyle.Flex;
                    break;

                case AssetSettings.CharInputStyle.CustomCharacters:
                    customCharacters.style.display = DisplayStyle.Flex;
                    break;

                case AssetSettings.CharInputStyle.UnicodeSequence:
                    unicodeSequence.style.display = DisplayStyle.Flex;
                    break;

                default:
                    break;
            }
        }

        private void TestCharacterList()
        {
            List<char> myCharacters = GetCharacterList();
            Debug.Log("Character count: " + myCharacters.Count);
            for (int i = 0; i < myCharacters.Count; i++)
            {
                Debug.Log(myCharacters[i]);
            }
        }

        private List<char> GetCharacterList()
        {
            List<char> myChars = new List<char>();

            if (Settings.charInputStyle == AssetSettings.CharInputStyle.CharacterRange)
            {
                myChars = GetCharacterFromRange(Settings.startChar, Settings.endChar);
            }
            else if (Settings.charInputStyle == AssetSettings.CharInputStyle.UnicodeRange)
            {
                char start = ConvertCharFromUnicode(Settings.startUnicode);
                char end = ConvertCharFromUnicode(Settings.endUnicode);

                myChars = GetCharacterFromRange(start, end);
            }
            else if (Settings.charInputStyle == AssetSettings.CharInputStyle.CustomCharacters)
            {
                myChars = Settings.customCharacters.ToCharArray().ToList();
            }
            else if (Settings.charInputStyle == AssetSettings.CharInputStyle.UnicodeSequence)
            {
                NewFontCharacterRange characterRange = new NewFontCharacterRange();
                myChars = characterRange.RetrieveCharacterListFromUnicodeSequence(Settings.unicodeSequence);
            }
            myChars = myChars.Distinct().ToList();

            return myChars;
        }

        private List<char> GetCharacterFromRange(char start, char end)
        {
            NewFontCharacterRange characterRange = new NewFontCharacterRange();
            List<char> characterList = characterRange.RetrieveCharactersList(start, end);
            return characterList;
        }

        private char ConvertCharFromUnicode(string unicode)
        {
            string s = System.Text.RegularExpressions.Regex.Unescape("\\u" + unicode);
            s.ToCharArray();
            if (s.Length > 0)
                return s[0];
            else
                return ' ';
        }

        #endregion Prebuilt characters

        private void Kerning()
        {
        }

        #endregion Font Creation

        #region Footer

        private void SetupFooter()
        {
            #region Variables

            string publisherStoreLink = "https://assetstore.unity.com/publishers/45848?aid=1011ljxWe";
            string documentationLink = "https://ferdowsur.gitbook.io/modular-3d-text/";
            string forumLink = "https://forum.unity.com/threads/modular-3d-text-complete-ingame-ui-sytem.821931/";
            string facebookLink = "https://www.facebook.com/tinygiantstudio";
            string redditLink = "https://www.reddit.com/r/tinygiantstudio";

            #endregion Variables

            GroupBox footer = rootVisualElement.Q<GroupBox>("Footer");

            SetupFooterURLButton(footer, "GetMoreAssetsButton", publisherStoreLink);
            SetupFooterURLButton(footer, "DocumentationButton", documentationLink);
            SetupFooterURLButton(footer, "ForumButton", forumLink);
            SetupFooterURLButton(footer, "FaceBookButton", facebookLink);
            SetupFooterURLButton(footer, "RedditButton", redditLink);
        }

        private void SetupFooterURLButton(GroupBox footer, string buttonName, string targetURL)
        {
            footer.Q<UnityEngine.UIElements.Button>(buttonName).clicked += () => Application.OpenURL(targetURL);
        }

        #endregion Footer
    }
}