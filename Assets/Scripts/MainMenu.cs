using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Numerics;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Newtonsoft.Json;

public class MainMenu : MonoBehaviour
{
    public Text walletAddress;

    public Text tokenBalance;

    public GameObject buyCanvas;

    public GameObject playCanvas;

    public string hashTransac;

    async void Awake()
    {
        string cero = "0";

        string chain = "binance";
        string network = "testnet"; // mainnet ropsten kovan rinkeby goerli
        string account = PlayerPrefs.GetString("Account");

        string balance = await EVM.BalanceOf(chain, network, account);
        print(balance);

        if (balance == cero)
        {
            ReceiveUnique();
        }

    }

    async void ShowSnakeBalance()
    {
        string chain = "binance";
        string network = "testnet";
        string contract = "0x549e80263f03334eB4Af74497F8B5D3B7Fda2BFB";
        string account = PlayerPrefs.GetString("Account");

        BigInteger balanceOf = await ERC20.BalanceOf(chain, network, contract, account);
        print(balanceOf);

        var decimals = BigInteger.Parse("1000000000000000000");

        BigInteger balanceMin = balanceOf / decimals;

        tokenBalance.text = balanceMin.ToString();
    }


    async public void CheckActive()
    {
        string argData = PlayerPrefs.GetString("Account");

        string falseMessage = "false";

        string trueMessage = "true";

        string chain = "binance";
        // set network mainnet, testnet
        string network = "testnet";
        // smart contract method to call
        string method = "active";
        // abi in json format
        string abi = "[ { \"constant\": true, \"inputs\": [ { \"name\": \"\", \"type\": \"address\" } ], \"name\": \"active\", \"outputs\": [ { \"name\": \"\", \"type\": \"bool\" } ], \"payable\": false, \"stateMutability\": \"view\", \"type\": \"function\" }, { \"constant\": false, \"inputs\": [ { \"name\": \"_amount\", \"type\": \"uint256\" } ], \"name\": \"receiveTokens\", \"outputs\": [], \"payable\": true, \"stateMutability\": \"payable\", \"type\": \"function\" }, { \"constant\": true, \"inputs\": [], \"name\": \"getBalanceBNB\", \"outputs\": [ { \"name\": \"\", \"type\": \"uint256\" } ], \"payable\": false, \"stateMutability\": \"view\", \"type\": \"function\" }, { \"payable\": true, \"stateMutability\": \"payable\", \"type\": \"fallback\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": false, \"name\": \"\", \"type\": \"address\" }, { \"indexed\": false, \"name\": \"\", \"type\": \"uint256\" } ], \"name\": \"Received\", \"type\": \"event\" } ]";
        // address of contract
        string contract = "0xaA2Cb6EA20808D51F84ec6aD350bf2BC635059C3";
        // array of arguments for contract
        string args = "[\"" + argData + "\"]";
        // connects to user's browser wallet to call a transaction
        string response = await EVM.Call(chain, network, contract, abi, method, args);
        // display response in game
        print(response);

        if (response == falseMessage)
        {
            buyCanvas.SetActive(true);
        }
        else if (response == trueMessage)
        {
            GoSnake();
        }
    }


    async public void PayBNBTicket()
    {
        //amount tokens = 1000 * 1000000000000000000

        string amount = "1000000000000000000000";

        // https://chainlist.org/
        string chainId = "97"; // rinkeby
        // account to send to
        string method = "receiveTokens";
        string contract = "0xaA2Cb6EA20808D51F84ec6aD350bf2BC635059C3";
        // value in wei
        string value = "12000000000000000";
        // data OPTIONAL
        string args = "[\"" + amount + "\"]";
        string abi = "[ { \"constant\": true, \"inputs\": [ { \"name\": \"\", \"type\": \"address\" } ], \"name\": \"active\", \"outputs\": [ { \"name\": \"\", \"type\": \"bool\" } ], \"payable\": false, \"stateMutability\": \"view\", \"type\": \"function\" }, { \"constant\": false, \"inputs\": [ { \"name\": \"_amount\", \"type\": \"uint256\" } ], \"name\": \"receiveTokens\", \"outputs\": [], \"payable\": true, \"stateMutability\": \"payable\", \"type\": \"function\" }, { \"constant\": true, \"inputs\": [], \"name\": \"getBalanceBNB\", \"outputs\": [ { \"name\": \"\", \"type\": \"uint256\" } ], \"payable\": false, \"stateMutability\": \"view\", \"type\": \"function\" }, { \"payable\": true, \"stateMutability\": \"payable\", \"type\": \"fallback\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": false, \"name\": \"\", \"type\": \"address\" }, { \"indexed\": false, \"name\": \"\", \"type\": \"uint256\" } ], \"name\": \"Received\", \"type\": \"event\" } ]";
        // gas limit OPTIONAL
        string gasLimit = "";
        // gas price OPTIONAL
        string data = await EVM.CreateContractData(abi, method, args);
        string gasPrice = "";
        // send transaction
        string response = await Web3Wallet.SendTransaction(chainId, contract, value, data, gasLimit, gasPrice);
        print(response);

        hashTransac = response;

    }

    async void CheckTxStatus()
    {
        string suc = "success";
        string pen = "pending";

        string chain = "binance";
        string network = "testnet";
        string transaction = hashTransac;

        string txStatus = await EVM.TxStatus(chain, network, transaction);
        print(txStatus); // success, fail, pending

        if (txStatus == suc || txStatus == pen)
        {
            buyCanvas.SetActive(false);
            playCanvas.SetActive(true);
        }
    }



    async public void ReceiveUnique()
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
        string value = "14000000000000000";
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

    }

    // Start is called before the first frame update
    void Start()
    {
        string wallet = PlayerPrefs.GetString("Account");
        walletAddress.text = wallet;
    }

   

    public void OnLogOut()
    {
        PlayerPrefs.SetString("Account", "");
        SceneManager.LoadScene(0);
    }

    public void GoClicker()
    {
        SceneManager.LoadScene(2);
    }

    public void GoSnake()
    {
        SceneManager.LoadScene(3);
    }

    public void OpenWebsite()
    {
        Application.OpenURL("https://android-blockgame.netlify.app/");
    }


    // Update is called once per frame
    void Update()
    {
        ShowSnakeBalance();
        CheckTxStatus();
    }
}
