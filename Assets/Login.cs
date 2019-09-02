using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

#region For Sending Emails
using System;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Linq;
#endregion

public class Login : MonoBehaviour
{
    public InputField username;
    public InputField password;
    public InputField email;

    public Text loginSuccess;
    
    public string[] letters = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
    public string randomNums;
    public string randomLets;
    public string randomCode;
    public int[] numbers = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };
    public int randomNum;
    public int randomLet;
    

    private string user;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator CreateUser(string username, string email, string password)
    {
        string createUserURL = "http://localhost/nsi-rpg/insertuser.php";
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("email", email);
        form.AddField("password", password);
        UnityWebRequest webRequest = UnityWebRequest.Post(createUserURL, form);
        yield return webRequest.SendWebRequest();
        Debug.Log(webRequest.error);
    }

    public void CreateNewUser()
    {
        StartCoroutine(CreateUser(username.text, email.text, password.text));
    }

    IEnumerator LoginUser(string username, string password)
    {
        string loginURL = "http://localhost/nsi-rpg/login.php";
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);
        UnityWebRequest loginRequest = UnityWebRequest.Post(loginURL, form);
        yield return loginRequest.SendWebRequest();
        Debug.Log(loginRequest.downloadHandler.text);

        if (loginRequest.downloadHandler.text == "Login Successful!")
        {
            SceneManager.LoadScene(1);
            loginSuccess = GetComponent<Text>();
            loginSuccess.text = loginRequest.downloadHandler.text;
        }
        else
        {

        }
    }

    public void LoginExistingUser()
    {
        StartCoroutine(LoginUser(username.text, password.text));
    }

    public void SendEmail(InputField email)
    {
        RandomCodeGenerator();
        MailMessage mail = new MailMessage();
        mail.From = new MailAddress("sqlunityclasssydney@gmail.com");
        mail.To.Add(email.text);
        mail.Subject = "NSIRPG Password Reset";
        mail.Body = "Hello " + user + "\nYou can reset your password using this code: " + randomCode;

        SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
        smtpServer.Port = 25; // 80 25
        smtpServer.Credentials = new NetworkCredential("sqlunityclasssydney@gmail.com", "sqlpassword") as ICredentialsByHost;
        smtpServer.EnableSsl = true;

        ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate cert, X509Chain chain, SslPolicyErrors policyErrors)
        {
            return true;
        };

        // Sends message
        smtpServer.Send(mail);
        Debug.Log("Sending Email...");
    }

    public void RandomCodeGenerator()
    {
        randomCode = "";
        for (int c = 0; c < 3; c++)
        {
            for (int l = 0; l < 6; l++)
            {
                randomLet = UnityEngine.Random.Range(0, letters.Length);
                randomLets = letters[randomLet];
            }
            for (int n = 0; n < 6; n++)
            {
                randomNum = UnityEngine.Random.Range(0, numbers.Length);
                randomNums = numbers[randomNum].ToString();
            }
            randomCode += randomLets + randomNums;
        }
        Debug.Log(randomCode);
    }

    IEnumerator ForgotUser(InputField email)
    {
        string forgotURL = "http://localhost/nsi-rpg/checkemail.php";
        WWWForm form = new WWWForm();
        form.AddField("email_Post", email.text);
        UnityWebRequest forgotRequest = UnityWebRequest.Post(forgotURL, form);
        yield return forgotRequest.SendWebRequest();
        Debug.Log(forgotRequest.downloadHandler.text);

        if(forgotRequest.downloadHandler.text == "User not found")
        {
            Debug.Log(forgotRequest.downloadHandler.text);
        }
        else
        {
            user = forgotRequest.downloadHandler.text;
            SendEmail(email);
        }
    }

    public void CheckEmail(InputField email)
    {
        StartCoroutine(ForgotUser(email));
    }
}
