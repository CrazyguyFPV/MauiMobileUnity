using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OntologyCSharpSDK;
using OntologyCSharpSDK.Interface;
using TMPro;
using System.Globalization;
using Vuplex.WebView;
using Vuplex.WebView.Demos;
using Ryu_Maui;

public static class Screens
{
    public static int CreateOrLogin = 0;
    public static int EnterPhoneNumber = 1;
    public static int Enter2FACode = 2;
    public static int VerifyYourID = 3;
    public static int ProcessingID = 4;
    public static int Matchmaking = 5;
    public static int Withdrawal = 6;
    public static int DepositSuccess = 7;
    public static int WithdrawalSuccess = 8;
    public static int BackupPhoneNumber = 9;
    public static int BackupCode = 10;
    public static int DepositOptions = 11;
}

public class UIManager : MonoBehaviour
{
    public OntologyCSharpSDK.Wallet.Account myAccount;

    private static readonly string _node = "http://polaris1.ont.io:20334"; //Ontology TestNet
    static OntologySdk OntSDK = new OntologySdk(_node, ConnectionMethodFactory.ConnectionMethod.REST);

    string uniqueID = MauiUtils.RandomString();

    // Create or Login screen
    public GameObject CreateOrLoginErrorText;
    public TMP_InputField NewUsernameInputField;

    // Login with phone number
    public TMP_InputField PhoneNumberInputField;
    public TMP_InputField CodeInputField;
    public GameObject CodeErrorText;
    public TMP_InputField CodeInputFieldBackup;
    public GameObject BackupCodeError;
    public TMP_InputField PhoneNumberInputFieldBackup;

    // Withdrawal
    public TextMeshProUGUI WithdrawableBalanceText;
    public TextMeshProUGUI AccountWithNumberText;
    public GameObject SendToPaypal;
    public TMP_InputField AmountWithdrawInput;

    // All Menus
    public GameObject Menus;

    // Ryu Home Screen
    public TextMeshProUGUI UsernameText;
    public TextMeshProUGUI CashBalanceText;
    public TextMeshProUGUI CrystalBalanceText;

    // Misc Saved vars
    public int myCrystals = 0;
    public double myCash = 0;


    // Web View
    public CanvasWebViewPrefab webViewPrefab;
    public GameObject webPanel;

    // Deposit
    public TextMeshProUGUI NewDepositAmountText;
    public TextMeshProUGUI DepositSuccessBalanceText;
    public TextMeshProUGUI DepositOptionBalanceText;

    // Results
    public TextMeshProUGUI ResultsBalanceText;

    [SerializeField] List<GameObject> uiPanels;
    private int curScreen = 0;
    private MauiInterface Maui;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        curScreen = Screens.CreateOrLogin;
        SetPanel(Screens.CreateOrLogin);            // start on first ui panel
        Maui = FindObjectOfType<MauiInterface>();

        if (StorageManager.Instance.isLoggedIn()) // User is logged in
            ShowRyuHome();
        else
        {
            UsernameText.text = "";
            CashBalanceText.text = "...";
            CrystalBalanceText.text = "...";
            DepositSuccessBalanceText.text = "...";
            ShowMenus();
        }

        yield return null;
    }

    private void ShowRyuHome()
    {
        HideAllMenus();
        UsernameText.text = StorageManager.Instance.GetUsername();
        CashBalanceText.text = "..."; // Hide balances while it loads
        CrystalBalanceText.text = "..."; // Hide balances while it loads
        DepositOptionBalanceText.text = "...";//
        ResultsBalanceText.text = "...";
        LoadBalances();
    }

    public void LoadBalances()
    {
        RyuInterface.GetBalances(balances =>
        {
            myCash = balances.cash;
            CashBalanceText.text = balances.cash.ToString("C", new CultureInfo("en-US"));
            myCrystals = balances.pearls;
            CrystalBalanceText.text = balances.pearls.ToString();

            DepositSuccessBalanceText.text = balances.cash.ToString("C", new CultureInfo("en-US"));
            DepositOptionBalanceText.text = balances.cash.ToString("C", new CultureInfo("en-US"));
            ResultsBalanceText.text = balances.cash.ToString("C", new CultureInfo("en-US"));
        });
    }

    // CreateOrLogin screen
    public void AttemptToCreateNewAccount()
    {
        NewUsernameInputField.DeactivateInputField(); // Prevent user from spamming
        string attemptedUsername = NewUsernameInputField.text;

        RyuInterface.IsUsernameTaken(attemptedUsername, isTaken =>
        {
            //
            // TODO: check usernamen length and bad words here, trim spaces
            //
            if (isTaken)
            {
                NewUsernameInputField.ActivateInputField(); // Allow user to type new username
                CreateOrLoginErrorText.SetActive(true);
            }
            else
            {
                RyuInterface.NewAccountWithoutPhone(attemptedUsername, myAccount, MauiConstants.BrazilAPIKey, MauiConstants.BrazilGameID, didSucceed =>
                {
                    Debug.Log("NewAccountWithoutPhone didSucceed " + didSucceed + " for: " + attemptedUsername);
                    StorageManager.Instance.SetUsername(attemptedUsername);
                    ShowRyuHome();
                });
            }
        });
    }

    public void HideErrorText()
    {
        CreateOrLoginErrorText.SetActive(false);
        CodeErrorText.SetActive(false);
        BackupCodeError.SetActive(false);
    }

    public void HideAllMenus()
    {
        Menus.SetActive(false);
    }

    public void ShowMenus()
    {
        Menus.SetActive(true);
    }

    private string userPhoneNumber;

    // Connect Ryu Account
    public void Get2FaCode()
    {
        string phoneNumber = "+1";

        phoneNumber += PhoneNumberInputField.text;
        phoneNumber += PhoneNumberInputFieldBackup.text;

        userPhoneNumber = phoneNumber;

        RyuInterface.SendNewCode(phoneNumber, response =>
        {
            Debug.Log("Sent code to: " + phoneNumber + " Address: " + response.address + " Exists: " + response.exists + " MessageId" + response.messageId);
        });
    }

    public void loginUserWithCode()
    {
        string code = CodeInputField.text;
        CodeInputField.DeactivateInputField();

        RyuInterface.LoginUser(code, userPhoneNumber, loginResult =>
        {
            if (loginResult != null)
            {
                StorageManager.Instance.SetUsername(loginResult.username);
                StorageManager.Instance.StorePhoneNumber(userPhoneNumber);

            //
            string encryptedKey = loginResult.encryptedKey;
                string passphrase = MauiUtils.ComputeSha256Hash(userPhoneNumber + "-STRING");
                string privateKey = MauiUtils.NEP2Decrypt(encryptedKey, passphrase);
            //

            StorageManager.Instance.StorePrivateKey(privateKey);

                ShowRyuHome();
            }
            else
            {
                CodeErrorText.SetActive(true);
                CodeInputField.ActivateInputField();
            }
        });
    }

    public void BackupWithCode()
    {
        string code = CodeInputFieldBackup.text;
        CodeInputFieldBackup.DeactivateInputField();

        string phoneNumber = "+1";

        phoneNumber += CodeInputFieldBackup.text;

        RyuInterface.Backup(code, phoneNumber, didBackup =>
        {
            if (didBackup)
            {
                ShowRyuHome();
            }
            else
            {
                BackupCodeError.SetActive(true);
                CodeInputFieldBackup.ActivateInputField();
            }
        });
    }

    public void getWithdrawable()
    {
        AccountWithNumberText.text = "Account with number " + StorageManager.Instance.GetPhoneNumber();
        WithdrawableBalanceText.text = "...";
        RyuInterface.GetWithdrawableBalance(withdrawableBalance =>
        {
            Debug.Log("withdrawableBalance: " + withdrawableBalance);
            WithdrawableBalanceText.text = withdrawableBalance.ToString("C", new CultureInfo("en-US"));
        });
    }

    public void SendWithdrawal()
    {
        string amountText = AmountWithdrawInput.text;
        double amount = System.Convert.ToDouble(amountText);
        RyuInterface.Withdraw(amount, didSucceed =>
        {
            if (didSucceed)
            {
                ShowWithdrawalSuccess();
            }
            else
            {
            //TODO show withdrawls error
        }

        });
    }

    private double amountDeposited; // For passing to the deposit success

    public void showPayPal1()
    {
        amountDeposited = 3;
        string link = generatePayPalLink("3.00", "DO1");
        ShowWebviewWithLink(link);
    }

    public void showPayPal2()
    {
        amountDeposited = 10;
        string link = generatePayPalLink("10.00", "DO2");
        ShowWebviewWithLink(link);
    }

    public void showPayPal3()
    {
        amountDeposited = 25;
        string link = generatePayPalLink("25.00", "DO3");
        ShowWebviewWithLink(link);
    }

    public void showPayPal4()
    {
        amountDeposited = 100;
        string link = generatePayPalLink("100.00", "DO4");
        ShowWebviewWithLink(link);
    }

    private void ShowWebviewWithLink(string url)
    {
        Debug.Log("Going to Checkout website: " + url);
        webPanel.SetActive(true);

        // Watches for the change in website title
        webViewPrefab.Initialized += (sender, e) =>
        {
            // Allows for typing with keyboard
            var hardwareKeyboardListener = HardwareKeyboardListener.Instantiate();
            hardwareKeyboardListener.KeyDownReceived += (sender, eventArgs) =>
            {
                webViewPrefab.WebView.HandleKeyboardInput(eventArgs.Value);
            };

            // Watches for the change in website title
            webViewPrefab.WebView.TitleChanged += (sender, eventArgs) =>
            {
                Debug.Log("Page title changed: " + eventArgs.Value);

                if (eventArgs.Value == "pc-paypal-success")
                {
                    webPanel.SetActive(false);
                    LoadBalances();
                    ShowDepositSuccess();
                    NewDepositAmountText.text = amountDeposited.ToString("C", new CultureInfo("en-US")); //amountDeposited
                }
                else if (eventArgs.Value == "pc-paypal-fail")
                {
                    webPanel.SetActive(false);
                    HideAllMenus();
                    // TODO: show error screen
                }
            };

            // Allows popups ? doens't work with Pappal but prevents error
            var webViewWithPopups = webViewPrefab.WebView as IWithPopups;
            if (webViewWithPopups != null)
            {
                webViewWithPopups.SetPopupMode(PopupMode.LoadInNewWebView);

                webViewWithPopups.PopupRequested += async (sender, eventArgs) =>
                {
                    Debug.Log("Popup opened with URL: " + eventArgs.Url);
                    // Create and display a new WebViewPrefab for the popup.
                    var popupPrefab = WebViewPrefab.Instantiate(eventArgs.WebView);
                    popupPrefab.transform.parent = transform;
                    popupPrefab.transform.localPosition = Vector3.zero;
                    popupPrefab.transform.localEulerAngles = new Vector3(0, 180, 0);
                    await popupPrefab.WaitUntilInitialized();
                    popupPrefab.WebView.CloseRequested += (popupWebView, closeEventArgs) =>
                    {
                        Debug.Log("Closing the popup");
                        popupPrefab.Destroy();
                    };
                };
            }

            webViewPrefab.WebView.LoadUrl(url);
        };

    }

    private string generatePayPalLink(string amount, string depositId)
    {
        string[] info = Maui.getSignature();
        string address = info[0];
        string publicKeyString = info[1];
        string message = info[2];
        string signature = info[3];

        string baseString = "https://ryu.games/checkout"; // TODO
        string envString = "?env=dev";
        string addrString = "&addr=" + address;
        string rsigString = "&rsig=" + signature;
        string randomKeyString = "&randomKey=" + message;
        string depositOptionIdString = "&depositOptionId=" + depositId;
        string amountString = "&amount=" + amount;
        string gameIdString = "&gameId=" + MauiConstants.BrazilGameID;
        string schemeString = "&scheme=PCDemoScheme";
        string versionString = "&version=4";
        string sdkVersionString = "&sdkVersion=1";
        string urlString = baseString + envString + addrString + rsigString + randomKeyString + depositOptionIdString + amountString + gameIdString + schemeString + versionString + sdkVersionString;
        return urlString;
    }

    // UI Control
    void SetPanel(int index)
    {
        if (index < uiPanels.Count)
        {
            // disable all first
            foreach (var panel in uiPanels)
                panel.SetActive(false);

            uiPanels[index].SetActive(true);
            curScreen = index;
        }
    }

    public void NextPressed()
    {
        curScreen++;
        if (curScreen == uiPanels.Count)
            curScreen = Screens.CreateOrLogin;      // wrap
        SetPanel(curScreen);
    }

    public void BackPressed()
    {
        curScreen--;
        if (curScreen < 0)
            curScreen = uiPanels.Count - 1;      // wrap
        SetPanel(curScreen);
    }

    public void ShowMatchMaking()
    {
        SetPanel(Screens.Matchmaking);
    }

    public void ShowFirstPanel()
    {
        SetPanel(Screens.CreateOrLogin);
    }

    public void ShowWithdrawl()
    {
        //
        // Uncomment to require backup to withdrawl
        //
        //if (StorageManager.Instance.HasBackedupPhone())
        SetPanel(Screens.Withdrawal);
        //else
        //SetPanel(10); // Show Backup if needed
    }

    public void ShowEnterCodeForBackup()
    {
        SetPanel(Screens.BackupCode);
    }

    public void ShowWithdrawalSuccess()
    {
        SetPanel(Screens.WithdrawalSuccess);
    }

    public void ShowDepositSuccess()
    {
        SetPanel(Screens.DepositSuccess);
    }

    public void ShowDeposit()
    {
        SetPanel(Screens.DepositOptions);
    }
}
