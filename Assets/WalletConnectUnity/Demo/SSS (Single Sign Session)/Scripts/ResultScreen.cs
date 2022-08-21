using UnityEngine;
using UnityEngine.UI;
using WalletConnectSharp.Unity;
using WalletConnectSharp.Unity.Models;
using GraphQlClient.Core;
using UnityEngine.Networking;
using Defective.JSON;

public class ResultScreen : MonoBehaviour
{
    public Text resultText;
    public GraphApi lensReference; 
    public void OnMessageSigned(WCMessageSigned messageSigned)
    {
        resultText.text = "Signed Message: " + messageSigned.SignedMessage + "\nRecovered Signer Address: Not Implemented";
    }

    public async void GetAccessToken()
    {
        GraphApi.Query query = lensReference.GetQueryByName("authenticate", GraphApi.Query.Type.Query);
        var signerAddress = WalletConnect.ActiveSession.Accounts[0];
        var signature = "";
        query.SetArgs(new { request = new { address = signerAddress, signature = signature } });

        UnityWebRequest request = await lensReference.Post(query);
        string data = request.downloadHandler.text;
        JSONObject json = new JSONObject(data);
        string accessToken = json.GetField("data").GetField("authenticate").GetField("accessToken").stringValue;
        Debug.Log(accessToken);
    }
}
