using System;
using System.Collections.Generic;

[Serializable]
public class AccountList
{
    public List<Account> Accounts { get; set; } = new List<Account>();
}
