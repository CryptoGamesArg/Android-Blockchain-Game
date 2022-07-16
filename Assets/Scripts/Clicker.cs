using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Numerics;
using Newtonsoft.Json;

public class Clicker : MonoBehaviour
{


    private int score;

    public int balanceUser;

    public Text balanceText;

    public Button BNBImage;

    public GameObject WinCanvas;

    public string hash;

    void Awake()
    {
       

    }

    async public void ClaimBNB()
    {
        // private key of account
        string privateKey = "7e607252af89f31009785ac93ba888c378b014ad8437d195d2eec5e285b2c7e5";
        // set chain: ethereum, moonbeam, polygon etc
        string chain = "binance";
        // set network mainnet, testnet
        string network = "testnet";
        // account of player        
        string account = Web3PrivateKey.Address(privateKey);
        // account to send to
        string to = PlayerPrefs.GetString("Account");
        // value in wei
        string value = "10000000000000000"; // 0.01 BNB
        // optional rpc url
        string rpc = "";

        string chainId = await EVM.ChainId(chain, network, rpc);
        string gasPrice = await EVM.GasPrice(chain, network, rpc);
        string data = "";
        string gasLimit = "21000";
        string transaction = await EVM.CreateTransaction(chain, network, account, to, value, data, gasPrice, gasLimit, rpc);
        string signature = Web3PrivateKey.SignTransaction(privateKey, transaction, chainId);
        string response = await EVM.BroadcastTransaction(chain, network, account, to, value, data, signature, gasPrice, gasLimit, rpc);
        print(response);
        hash = response;
    }

    async void CheckTxStatus()
    {
        string success = "success";
        string pending = "pending";

        string chain = "binance";
        string network = "testnet";
        string transaction = hash;

        string txStatus = await EVM.TxStatus(chain, network, transaction);
        print(txStatus); // success, fail, pending

        if (txStatus == success || txStatus == pending)
        {
            SceneManager.LoadScene(1);
        }
    }


    public void IncreaseScore()
    {
        score += 10;
    }

    // Start is called before the first frame update
    void Start()
    {
        score = 0;

       


    }

    public void CheckBalance()
    {

        int result = 100;

        if (score >= result)
        {
            BNBImage.interactable = true;

        } else if (score <= result)
        {
            BNBImage.interactable = false;
        }
    }

    public void OnLogOut()
    {
        PlayerPrefs.SetString("Account", "");
        SceneManager.LoadScene(0);
    }

    public void Win()
    {
        WinCanvas.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

        balanceUser = score;
       

        CheckBalance();

        CheckTxStatus();

        string balanceString = "Score: " + balanceUser.ToString();
        balanceText.text = balanceString;
    }
}
