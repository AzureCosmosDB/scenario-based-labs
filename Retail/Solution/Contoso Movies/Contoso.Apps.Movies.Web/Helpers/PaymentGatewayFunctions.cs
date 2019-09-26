using Contoso.Apps.Common;
using Contoso.Apps.Movies.Web.Helpers;
using Contoso.Apps.Movies.Web.Models;
using System;
using System.Configuration;
using System.IO;
using System.Net;


/// <summary>
/// The endpoints, data transport (kvp/nvp) methods, and most of the data parameter
/// names closely approximate the PayPal gateway API, as a point of reference.
/// </summary>
public class NVPAPICaller
{
    // Get the current cart Id
    private string cartId = new CartHelpers().GetCartId();

    // Flag that determines the Gateway environment (live or sandbox)
    private const bool bSandbox = false;
    private const string CVV2 = "CVV2";

    // Gateway connection strings.
    //private string pEndPointURL = "https://microsoft-apiappbd2d9c33a31544819ca4da66ffeb9d6e.azurewebsites.net/api/nvp";
    private string pEndPointURL = ConfigurationManager.AppSettings["paymentsAPIUrl"];
    
    // Sandbox strings.
    private string pEndPointURL_SB = "https://localhost:44301/api/nvp";

    private const string SIGNATURE = "SIGNATURE";
    private const string PWD = "PWD";
    private const string ACCT = "ACCT";

    public string APIUsername = "ContosoUser";
    private string APIPassword = "ContosoPassword";
    private string APISignature = "ABCDEFGHIJKLMNOP1234567890";
    private string Subject = "";
    private string BNCode = "PP-ECWizard";


    // HttpWebRequest Timeout specified in milliseconds.
    private const int Timeout = 15000;
    private static readonly string[] SECURED_NVPS = new string[] { ACCT, CVV2, SIGNATURE, PWD };

    public void SetCredentials(string Userid, string Pwd, string Signature)
    {
        APIUsername = Userid;
        APIPassword = Pwd;
        APISignature = Signature;
    }

    /// <summary>
    /// This method is called first in order to perform the initial authorization for payment.
    /// </summary>
    /// <param name="amt"></param>
    /// <param name="token"></param>
    /// <param name="retMsg"></param>
    /// <returns></returns>
    public bool DoCheckoutAuth(OrderModel order, ref string token, ref NVPCodec decoder)
    {
        if (bSandbox)
        {
            pEndPointURL = pEndPointURL_SB;
        }

        NVPCodec encoder = new NVPCodec();
        encoder[NVPProperties.Properties.METHOD] = NVPProperties.Methods.AuthorizePayment;
        encoder[NVPProperties.Properties.BRANDNAME] = "Contoso Movies";
        encoder[NVPProperties.Properties.PAYMENTREQUEST_AMT] = order.Total.ToString();
        encoder[NVPProperties.Properties.FIRSTNAME] = order.FirstName;
        encoder[NVPProperties.Properties.LASTNAME] = order.LastName;
        encoder[NVPProperties.Properties.ADDRESS] = order.Address;
        encoder[NVPProperties.Properties.CITY] = order.City;
        encoder[NVPProperties.Properties.STATE] = order.State;
        encoder[NVPProperties.Properties.POSTALCODE] = order.PostalCode;
        encoder[NVPProperties.Properties.NAME_ON_CREDIT_CARD] = order.NameOnCard;
        encoder[NVPProperties.Properties.CREDIT_CARD_TYPE] = order.CreditCardType;
        encoder[NVPProperties.Properties.CREDIT_CARD_NUMBER] = order.CreditCardNumber;
        encoder[NVPProperties.Properties.CREDIT_CARD_EXPDATE] = order.ExpirationDate;
        encoder[NVPProperties.Properties.CCV2] = order.CCV;

        string pStrrequestforNvp = encoder.Encode();
        string pStresponsenvp = HttpCall(pStrrequestforNvp);

        decoder = new NVPCodec();
        decoder.Decode(pStresponsenvp);

        string strAck = decoder[NVPProperties.Properties.ACK].ToLower();
        if (strAck != null && (strAck == "success" || strAck == "successwithwarning"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Call this method to complete payment.
    /// </summary>
    /// <param name="finalPaymentAmount"></param>
    /// <param name="token"></param>
    /// <param name="PayerID"></param>
    /// <param name="decoder"></param>
    /// <param name="retMsg"></param>
    /// <returns></returns>
    public bool DoCheckoutPayment(string finalPaymentAmount, string token, ref NVPCodec decoder)
    {
        if (bSandbox)
        {
            pEndPointURL = pEndPointURL_SB;
        }

        NVPCodec encoder = new NVPCodec();
        encoder[NVPProperties.Properties.METHOD] = NVPProperties.Methods.ProcessPayment;
        encoder[NVPProperties.Properties.TOKEN] = token;
        encoder[NVPProperties.Properties.PAYMENTREQUEST_AMT] = finalPaymentAmount;

        string pStrrequestforNvp = encoder.Encode();
        string pStresponsenvp = HttpCall(pStrrequestforNvp);

        decoder = new NVPCodec();
        decoder.Decode(pStresponsenvp);

        string strAck = decoder[NVPProperties.Properties.ACK].ToLower();
        if (strAck != null && (strAck == "success" || strAck == "successwithwarning"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public string HttpCall(string NvpRequest)
    {
        string url = pEndPointURL;

        string strPost = NvpRequest + "&" + buildCredentialsNVPString();

        //next line added per recorded issue #23 related to TLS End-of-Life (EOL) issue
        //Soliance | sac | 01.16.19
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(url);
        objRequest.Timeout = Timeout;
        objRequest.Method = "POST";
        objRequest.ContentLength = strPost.Length;
        objRequest.ContentType = "text/plain";

        try
        {
            using (StreamWriter myWriter = new StreamWriter(objRequest.GetRequestStream()))
            {
                myWriter.Write(strPost);
            }
        }
        catch (Exception e)
        {
            // Log the exception.
            ExceptionUtility.LogException(e, "HttpCall in PaymentGatewayFunctions.cs");
        }

        //Retrieve the Response returned from the NVP API call to PayPal.
        HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
        string result;
        using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
        {
            result = sr.ReadToEnd();
        }

        return result;
    }

    private string buildCredentialsNVPString()
    {
        NVPCodec codec = new NVPCodec();

        if (!IsEmpty(APIUsername))
            codec[NVPProperties.Properties.USER] = APIUsername;

        if (!IsEmpty(APIPassword))
            codec[NVPProperties.Properties.PWD] = APIPassword;

        if (!IsEmpty(APISignature))
            codec[NVPProperties.Properties.SIGNATURE] = APISignature;

        if (!IsEmpty(Subject))
            codec["SUBJECT"] = Subject;

        codec["VERSION"] = "1.0";

        return codec.Encode();
    }

    public static bool IsEmpty(string s)
    {
        return s == null || s.Trim() == string.Empty;
    }

    /// <summary>
    /// Populates a new CheckoutErrorModel object from the passed in return data from the gateway.
    /// </summary>
    /// <param name="decoder"></param>
    /// <returns></returns>
    public CheckoutErrorModel PopulateGatewayErrorModel(NVPCodec decoder)
    {
        var errorVM = new CheckoutErrorModel
        {
            ErrorCode = decoder[NVPProperties.Properties.ERRORCODE],
            LongMessage = decoder[NVPProperties.Properties.LONGMESSAGE],
            ShortMessage = decoder[NVPProperties.Properties.SHORTMESSAGE]
        };
        return errorVM;
    }
}