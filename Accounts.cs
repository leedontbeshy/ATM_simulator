using System;
using System.Xml.Serialization;

[Serializable]
public class Account
{
    public string Username { get; set; }
    public string Password { get; set; }
    public decimal Balance { get; set; }

    public Account() { }

    public Account(string username, string password, decimal balance)
    {
        Username = username;
        Password = password;
        Balance = balance;
    }
}
