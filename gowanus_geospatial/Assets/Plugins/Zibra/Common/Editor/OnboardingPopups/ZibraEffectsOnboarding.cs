using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using com.zibra.common.Editor.SDFObjects;
using System;
using System.Text.RegularExpressions;

namespace com.zibra.common.Editor
{
    internal class ZibraEffectsOnboarding : EditorWindow
    {
#if !ZIBRA_EFFECTS_OTP_VERSION
        private const string ONBOARDING_GUID = "39f6c1ca9194bcb4d896f42e94b38b88";
        private const string AUTHORIZATION_GUID = "c70f412c74090c8488e3e2d7ec4a6101";
        private string CurrentUXMLGUID = ONBOARDING_GUID;
#else
        private const string REGISTRATION_GUID = "a4fb4aa3ec918b0438c0370d64ed10a9";
        private const string AUTHORIZATION_GUID = "067dcf765d95fb844898b6de5e81aeae";
        private string CurrentUXMLGUID = REGISTRATION_GUID;
#endif

        private const PluginManager.Effect LIQUID = PluginManager.Effect.Liquid;
        private const PluginManager.Effect SMOKE = PluginManager.Effect.Smoke;

        private Color LIGHT_RED = new Color(1f, 0.3f, 0.3f, 1f);
        private Color LIGHT_GREEEN = new Color(0.3f, 1f, 0.3f, 1f);

        private bool TriedToVerify = false;

        private TextField AuthKeyInputField;
        private VisualElement BackElement;
        private Label HeaderMessage;
        private Label BodyMessage;
        private Label StatusMessage;
#if !ZIBRA_EFFECTS_OTP_VERSION
        private Label AuthMessageLiquid;
        private Label AuthMessageSmoke;
        private VisualElement ValidationMessages;
#else
        private TextField OrderNumber;
        private TextField Email;
        private TextField Name;
        private Button Register;
        private VisualElement RegistrationFields;
        private Button HaveKey;
        private Button OrderHistory;
        private RegistrationManager.Status LatestRegistrationStatus;
        private bool TriedToRegister = false;
#endif
        private Button ActivateButton;
        private Button GetStartedButton;

        private ServerAuthManager.Status LatestStatusLiquid = ServerAuthManager.Status.NotInitialized;
        private ServerAuthManager.Status LatestStatusSmoke = ServerAuthManager.Status.NotInitialized;

        private ServerAuthManager ServerAuthInstance;

        public static GUIContent WindowTitle => new GUIContent("Zibra Effects Onboarding Screen");

        internal static void ShowWindowDelayed()
        {
            ShowWindow();
            EditorApplication.update -= ShowWindowDelayed;
        }

        [InitializeOnLoadMethod]
        internal static void InitializeOnLoad()
        {
            // Don't automatically open any windows in batch mode
            if (Application.isBatchMode)
            {
                return;
            }

            // If user already has saved license key, don't show him this popup
            if (ServerAuthManager.GetInstance().PluginLicenseKeys.Length > 0)
            {
                // If user removes key during editor session, don't show him popup
                SessionState.SetBool("ZibraEffectsProOnboardingShown", false);
                return;
            }

            if (SessionState.GetBool("ZibraEffectsProOnboardingShown", true))
            {
                SessionState.SetBool("ZibraEffectsProOnboardingShown", false);
                EditorApplication.update += ShowWindowDelayed;
            }
        }

        [MenuItem(Effects.BaseMenuBarPath + "Open Onboarding", false, 1)]
        private static void ShowWindow()
        {
            ZibraEffectsOnboarding window = (ZibraEffectsOnboarding)GetWindow(typeof(ZibraEffectsOnboarding));
            window.titleContent = WindowTitle;
            window.Show();
        }

        private static void CloseWindow()
        {
            ZibraEffectsOnboarding window = (ZibraEffectsOnboarding)GetWindow(typeof(ZibraEffectsOnboarding));
            window.Close();
        }

        private void OnEnable()
        {
            ServerAuthInstance = ServerAuthManager.GetInstance();

#if !ZIBRA_EFFECTS_OTP_VERSION
            string productName = "c70f412c74090c8488e3e2d7ec4a6101";
#else
            string productName;
            if (PluginManager.AvailableCount() != 1)
            {
                productName = "Zibra Solutions";
            }
            else if (PluginManager.IsAvailable(LIQUID))
            {
                productName = "Zibra Liquid";
            }
            else
            {
                productName = "Zibra Smoke & Fire";
            }
#endif

            var root = rootVisualElement;
            root.Clear();

            int width = 456;
            int height = 442;

            minSize = maxSize = new Vector2(width, height);

            var uxmlAssetPath = AssetDatabase.GUIDToAssetPath(CurrentUXMLGUID);
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlAssetPath);
            visualTree.CloneTree(root);

            root.Query<TextElement>().ToList().ForEach(e => { e.text = e.text.Replace("!ProductName", productName); });

#if !ZIBRA_EFFECTS_OTP_VERSION
            if (CurrentUXMLGUID == ONBOARDING_GUID)
            {
                root.Q<Button>("GetKey").clicked += GetKeyClick;
                root.Q<Button>("HaveKey").clicked += HaveKeyClick;
                root.Q<Button>("PrivacyPolicy").clicked += PrivacyPolicyClick;
                root.Q<Button>("TermsAndConditions").clicked += TermsAndConditionsClick;
            }
#endif
#if ZIBRA_EFFECTS_OTP_VERSION
            if (CurrentUXMLGUID == REGISTRATION_GUID)
            {
                if (ServerAuthInstance.IsGenerationAvailable())
                {
                    CurrentUXMLGUID = AUTHORIZATION_GUID;
                    OnEnable();
                    return;
                }

                StatusMessage = root.Q<Label>("StatusMessage");
                OrderNumber = root.Q<TextField>("OrderNumber");
                Email = root.Q<TextField>("Email");
                Name = root.Q<TextField>("Name");
                Register = root.Q<Button>("Register");
                Register.clicked += RegisterClick;
                RegistrationFields = root.Q<VisualElement>("RegistrationFields");
                HaveKey = root.Q<Button>("HaveKey");
                HaveKey.clicked += HaveKeyClick;
                OrderHistory = root.Q<Button>("OrderHistory");
                OrderHistory.clicked += OrderHistoryClick;

                EventCallback<KeyDownEvent> RegisterLambda = (evt =>
                {
                    if (evt.keyCode == KeyCode.Return)
                    {
                        RegisterClick();
                        evt.StopPropagation();
                    }
                });
                OrderNumber.RegisterCallback(RegisterLambda);
                Email.RegisterCallback(RegisterLambda);
                Name.RegisterCallback(RegisterLambda);
            }
#endif
            if (CurrentUXMLGUID == AUTHORIZATION_GUID)
            {
                root.Q<Button>("BackButton").clicked += BackClick;

                BackElement = root.Q<VisualElement>("BackElement");
                HeaderMessage = root.Q<Label>("HeaderMessage");
                BodyMessage = root.Q<Label>("BodyMessage");
                StatusMessage = root.Q<Label>("StatusMessage");
#if !ZIBRA_EFFECTS_OTP_VERSION
                AuthMessageLiquid = root.Q<Label>("AuthMessageLiquid");
                AuthMessageSmoke = root.Q<Label>("AuthMessageSmoke");
                ValidationMessages = root.Q<VisualElement>("ValidationMessages");
#endif
                ActivateButton = root.Q<Button>("Activate");
                GetStartedButton = root.Q<Button>("GetStart");
                AuthKeyInputField = root.Q<TextField>("ActivationField");

                AuthKeyInputField.RegisterCallback<KeyDownEvent>(evt =>
                                                                 {
                                                                     if (evt.keyCode == KeyCode.Return)
                                                                     {
                                                                         ActivateClick();
                                                                         evt.StopPropagation();
                                                                     }
                                                                 });
                ActivateButton.clicked += ActivateClick;
                GetStartedButton.clicked += GetStartedClick;

                UpdateActivationUI();
            }
        }

        private void GetKeyClick()
        {
#if !ZIBRA_EFFECTS_OTP_VERSION
            Application.OpenURL("https://license.zibra.ai/api/stripeTrial?source=plugin");
            CurrentUXMLGUID = AUTHORIZATION_GUID;
#else
            CurrentUXMLGUID = REGISTRATION_GUID;
#endif
            OnEnable();
        }

        private void HaveKeyClick()
        {
            CurrentUXMLGUID = AUTHORIZATION_GUID;
            OnEnable();
        }

        private void PrivacyPolicyClick()
        {
            Application.OpenURL("https://zibra.ai/privacy-policy/");
        }

        private void TermsAndConditionsClick()
        {
            Application.OpenURL("https://zibra.ai/terms-of-service/");
        }

#if ZIBRA_EFFECTS_OTP_VERSION
        private void OrderHistoryClick()
        {
            Application.OpenURL("https://assetstore.unity.com/orders");
        }
        private void RegisterClick()
        {
            if (!ValidateRegistrationInput())
            {
                return;
            }

            TriedToRegister = true;
            RegistrationFields.style.display = DisplayStyle.None;
            HaveKey.style.display = DisplayStyle.None;
            OrderHistory.style.display = DisplayStyle.None;
            StatusMessage.text = "Registering in progress, please wait.";
            RegistrationManager.Instance.Register(OrderNumber.text, Email.text, Name.text);
            UpdateRegistrationUI();
        }

        private void ReportError(string error)
        {
            LabelStyle(StatusMessage, LIGHT_RED);
            StatusMessage.text = error;
        }

        private bool ValidateRegistrationInput()
        {
            const string ORDER_NUMBER_REGEX = "^(\\d{13,14})|(IN\\d{12})$";
            const string EMAIL_REGEX = "^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]+$";
            const string NAME_REGEX = "^[a-zA-Z\\- ]+$";

            OrderNumber.value = OrderNumber.text.Trim();
            Email.value = Email.text.Trim();
            Name.value = Name.text.Trim();

            if (!Regex.IsMatch(OrderNumber.text, ORDER_NUMBER_REGEX))
            {
                ReportError(OrderNumber.text == "" ? "Please enter Order number or Invoice number." : "Invalid Order number or Invoice number.");
                return false;
            }
            if (!Regex.IsMatch(Email.text, EMAIL_REGEX))
            {
                ReportError(Email.text == "" ? "Please enter your Email." : "Invalid Email. Please ensure you enter a valid email address.");
                return false;
            }
            if (!Regex.IsMatch(Name.text, NAME_REGEX) || Name.text == "Name")
            {
                ReportError(Name.text == "" || Name.text == "Name" ? "Please enter your Name." : "Please ensure you have entered your name correctly.");
                return false;
            }

            return true;
        }

        private void UpdateRegistrationUI()
        {
            if (ServerAuthInstance.IsGenerationAvailable() ||
                RegistrationManager.Instance.CurrentStatus == RegistrationManager.Status.OK)
            {
                TriedToVerify = true;
                CurrentUXMLGUID = AUTHORIZATION_GUID;
                OnEnable();
                ReportActivationStart();
                return;
            }
            else if (!TriedToRegister || RegistrationManager.Instance.CurrentStatus == RegistrationManager.Status.InProgress)
            {
                // NOOP
            }
            else
            {
                ReportError(RegistrationManager.Instance.ErrorMessage);
                RegistrationFields.style.display = DisplayStyle.Flex;
                HaveKey.style.display = DisplayStyle.Flex;
                OrderHistory.style.display = DisplayStyle.Flex;
                TriedToRegister = false;
            }
        }
#endif

        private void ActivateClick()
        {
            string keys = AuthKeyInputField.text;

            if (keys == "")
            {
                LabelStyle(StatusMessage, LIGHT_RED);
                StatusMessage.text = ("Please enter your license key.");
                return;
            }

            if (!ServerAuthManager.CheckKeysFormat(keys))
            {
                LabelStyle(StatusMessage, LIGHT_RED);
                StatusMessage.text = ("Invalid license key.");
                return;
            }

            TriedToVerify = true;
            ServerAuthInstance.RegisterKey(keys);
#if !ZIBRA_EFFECTS_OTP_VERSION
            ValidationMessages.style.display = DisplayStyle.None;
#endif
            ReportActivationStart();
        }

        private void ReportActivationStart()
        {
            StatusMessage.style.display = DisplayStyle.Flex;
            BackElement.style.display = DisplayStyle.None;
            BodyMessage.style.display = DisplayStyle.None;
            AuthKeyInputField.style.display = DisplayStyle.None;
            ActivateButton.style.display = DisplayStyle.None;
            LabelStyle(StatusMessage, Color.white);
            StatusMessage.text = ServerAuthInstance.GetErrorMessage(LIQUID);
        }

        private void GetStartedClick()
        {
            EditorApplication.ExecuteMenuItem(Effects.BaseMenuBarPath + "Open User Guide");
            CloseWindow();
        }

        private void BackClick()
        {
#if !ZIBRA_EFFECTS_OTP_VERSION
            CurrentUXMLGUID = ONBOARDING_GUID;
#else
            CurrentUXMLGUID = REGISTRATION_GUID;
#endif
            OnEnable();
        }

        private void LabelStyle(Label authMessage, Color color)
        {
            authMessage.style.color = new StyleColor(color);
        }

        private void Update()
        {
            if (TriedToVerify && CurrentUXMLGUID == AUTHORIZATION_GUID)
            {
                if (ServerAuthInstance.GetStatus(LIQUID) == LatestStatusLiquid &&
                    ServerAuthInstance.GetStatus(SMOKE) == LatestStatusSmoke)
                {
                    return;
                }

                LatestStatusLiquid = ServerAuthManager.GetInstance().GetStatus(LIQUID);
                LatestStatusSmoke = ServerAuthManager.GetInstance().GetStatus(SMOKE);

                UpdateActivationUI();
            }

#if ZIBRA_EFFECTS_OTP_VERSION
            if (CurrentUXMLGUID == REGISTRATION_GUID)
            {
                if (RegistrationManager.Instance.CurrentStatus == LatestRegistrationStatus)
                {
                    return;
                }

                LatestRegistrationStatus = RegistrationManager.Instance.CurrentStatus;

                UpdateRegistrationUI();
            }
#endif
        }
        private void UpdateActivationUI()
        {
#if !ZIBRA_EFFECTS_OTP_VERSION
            if (ServerAuthInstance.IsLicenseVerified(LIQUID) && ServerAuthInstance.IsLicenseVerified(SMOKE))
            {
                ValidationMessages.style.display = DisplayStyle.None;
                LabelStyle(StatusMessage, LIGHT_GREEEN);
                StatusMessage.text = "License validated successfully";
                AuthKeyInputField.style.display = DisplayStyle.None;
                BodyMessage.style.display = DisplayStyle.None;
                ActivateButton.style.display = DisplayStyle.None;
                StatusMessage.style.display = DisplayStyle.Flex;
                GetStartedButton.style.display = DisplayStyle.Flex;
            }
            else if (ServerAuthInstance.IsLicenseVerified(LIQUID))
            {
                StatusMessage.style.display = DisplayStyle.None;
                AuthKeyInputField.style.display = DisplayStyle.None;
                BodyMessage.style.display = DisplayStyle.None;
                ActivateButton.style.display = DisplayStyle.None;
                LabelStyle(AuthMessageLiquid, LIGHT_GREEEN);
                AuthMessageLiquid.text = "Liquid license validated successfully";
                LabelStyle(AuthMessageSmoke, Color.white);
                AuthMessageSmoke.text = "Your license doesn't include Smoke & Fire";
                ValidationMessages.style.display = DisplayStyle.Flex;
                GetStartedButton.style.display = DisplayStyle.Flex;
            }
            else if (ServerAuthInstance.IsLicenseVerified(SMOKE))
            {
                StatusMessage.style.display = DisplayStyle.None;
                AuthKeyInputField.style.display = DisplayStyle.None;
                BodyMessage.style.display = DisplayStyle.None;
                ActivateButton.style.display = DisplayStyle.None;
                LabelStyle(AuthMessageLiquid, Color.white);
                AuthMessageLiquid.text = "Your license doesn't include Liquid";
                LabelStyle(AuthMessageSmoke, LIGHT_GREEEN);
                AuthMessageSmoke.text = "Smoke & Fire license validated successfully";
                ValidationMessages.style.display = DisplayStyle.Flex;
                GetStartedButton.style.display = DisplayStyle.Flex;
            }
            else if (!TriedToVerify ||
                ServerAuthInstance.GetStatus(LIQUID) == ServerAuthManager.Status.KeyValidationInProgress ||
                (ServerAuthInstance.GetStatus(LIQUID) == ServerAuthManager.Status.NotRegistered && 
                ServerAuthInstance.GetStatus(SMOKE) == ServerAuthManager.Status.NotRegistered))
            {
                // NOOP
            } else
            {
                LabelStyle(StatusMessage, LIGHT_RED);
                StatusMessage.text = ServerAuthInstance.GetErrorMessage(LIQUID);
                ValidationMessages.style.display = DisplayStyle.None;
                AuthKeyInputField.style.display = DisplayStyle.Flex;
                BodyMessage.style.display = DisplayStyle.Flex;
                ActivateButton.style.display = DisplayStyle.Flex;
                StatusMessage.style.display = DisplayStyle.Flex;
            }
#else
            if (ServerAuthInstance.IsGenerationAvailable())
            {
                LabelStyle(StatusMessage, LIGHT_GREEEN);
                HeaderMessage.text = "Zibra Solutions activated";
                StatusMessage.text = "";
                if (RegistrationManager.Instance.IsFirstRegistration)
                {
                    StatusMessage.text = $"Here's your unique license key: {ServerAuthInstance.GenerationLicenseKey}. It has also been sent to your email. You can use this key for activation on another device.";
                }
                BackElement.style.display = DisplayStyle.None;
                BodyMessage.style.display = DisplayStyle.None;
                AuthKeyInputField.style.display = DisplayStyle.None;
                ActivateButton.style.display = DisplayStyle.None;
                StatusMessage.style.display = DisplayStyle.Flex;
                GetStartedButton.style.display = DisplayStyle.Flex;
            }
            else if (!TriedToVerify ||
                ServerAuthInstance.GetStatus(LIQUID) == ServerAuthManager.Status.KeyValidationInProgress ||
                ServerAuthInstance.GetStatus(LIQUID) == ServerAuthManager.Status.NotRegistered)
            {
                // NOOP
            }
            else
            {
                LabelStyle(StatusMessage, LIGHT_RED);
                StatusMessage.text = ServerAuthInstance.GetGenerationErrorMessage();
                BackElement.style.display = DisplayStyle.Flex;
                BodyMessage.style.display = DisplayStyle.Flex;
                StatusMessage.style.display = DisplayStyle.Flex;
                AuthKeyInputField.style.display = DisplayStyle.Flex;
                ActivateButton.style.display = DisplayStyle.Flex;
                TriedToVerify = false;
            }
#endif
        }

    }
}
