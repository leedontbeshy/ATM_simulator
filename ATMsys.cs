using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

public class ATMSystem
{
    private AccountList accountList = new AccountList();
    private Account currentAccount;
    private string dataFile = "accounts.xml";

    // Tạo delegate và event
    public delegate void TransactionHandler(string message);
    public event TransactionHandler TransactionCompleted;

    public ATMSystem()
    {
        LoadData();
        // Đăng ký phương thức nhận sự kiện
        TransactionCompleted += ShowMessage;
    }

    // Hiển thị tin nhắn thông báo (giả lập tin nhắn gửi về điện thoại)
    private void ShowMessage(string message)
    {
        Console.WriteLine($"[THÔNG BÁO]: {message}");
    }

    public void Run()
    {
        Console.Clear();
        Console.WriteLine("----- ATM SYSTEM -----");
        Console.Write("Nhập tên tài khoản: ");
        string username = Console.ReadLine();
        Console.Write("Nhập mật khẩu: ");
        string password = Console.ReadLine();

        if (Login(username, password))
        {
            Console.WriteLine($"Đăng nhập thành công! Xin chào {currentAccount.Username}");
            Menu();
        }
        else
        {
            Console.WriteLine("Sai tên tài khoản hoặc mật khẩu!");
        }
    }

    private void Menu()
    {
        int choice;
        do
        {
            Console.WriteLine("\n1. Xem số dư");
            Console.WriteLine("2. Rút tiền");
            Console.WriteLine("3. Chuyển tiền");
            Console.WriteLine("0. Thoát");
            Console.Write("Chọn chức năng: ");
            choice = int.Parse(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    Console.WriteLine($"Số dư hiện tại: {currentAccount.Balance} VND");
                    break;
                case 2:
                    Withdraw();
                    break;
                case 3:
                    Transfer();
                    break;
                case 0:
                    SaveData();
                    Console.WriteLine("Thoát chương trình!");
                    break;
                default:
                    Console.WriteLine("Chức năng không hợp lệ!");
                    break;
            }

        } while (choice != 0);
    }

    private bool Login(string username, string password)
    {
        foreach (var acc in accountList.Accounts)
        {
            if (acc.Username == username && acc.Password == password)
            {
                currentAccount = acc;
                return true;
            }
        }
        return false;
    }

    private void Withdraw()
    {
        Console.Write("Nhập số tiền cần rút: ");
        decimal amount = decimal.Parse(Console.ReadLine());

        if (amount <= 0)
        {
            Console.WriteLine("Số tiền không hợp lệ!");
            return;
        }

        if (amount > currentAccount.Balance)
        {
            Console.WriteLine("Không đủ số dư!");
            return;
        }

        currentAccount.Balance -= amount;
        SaveData();

        // Kích hoạt sự kiện thông báo rút tiền
        TransactionCompleted?.Invoke($"Bạn vừa rút {amount} VND. Số dư mới là {currentAccount.Balance} VND.");
    }

    private void Transfer()
    {
        Console.Write("Nhập tài khoản nhận: ");
        string receiverUsername = Console.ReadLine();
        Console.Write("Nhập số tiền cần chuyển: ");
        decimal amount = decimal.Parse(Console.ReadLine());

        if (receiverUsername == currentAccount.Username)
        {
            Console.WriteLine("Không thể chuyển cho chính mình!");
            return;
        }

        Account receiver = accountList.Accounts.Find(a => a.Username == receiverUsername);

        if (receiver == null)
        {
            Console.WriteLine("Tài khoản nhận không tồn tại!");
            return;
        }

        if (amount <= 0)
        {
            Console.WriteLine("Số tiền không hợp lệ!");
            return;
        }

        if (amount > currentAccount.Balance)
        {
            Console.WriteLine("Không đủ số dư để chuyển!");
            return;
        }

        currentAccount.Balance -= amount;
        receiver.Balance += amount;
        SaveData();

        // Kích hoạt sự kiện thông báo chuyển tiền
        TransactionCompleted?.Invoke($"Bạn vừa chuyển {amount} VND cho {receiver.Username}. Số dư mới là {currentAccount.Balance} VND.");
    }

    private void LoadData()
    {
        try
        {
            if (File.Exists(dataFile))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(AccountList));
                using (FileStream fs = new FileStream(dataFile, FileMode.Open))
                {
                    accountList = (AccountList)serializer.Deserialize(fs);
                }
                Console.WriteLine("Đọc file dữ liệu thành công.");
            }
            else
            {
                Console.WriteLine("Không tìm thấy file dữ liệu. Tạo file mẫu...");
                CreateSampleXML();
                LoadData();
            }
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Lỗi khi đọc file: {ex.Message}");
            CreateSampleXML();
            LoadData();
        }
    }

    private void SaveData()
    {
        try
        {
            XmlSerializer serializer = new XmlSerializer(typeof(AccountList));
            using (FileStream fs = new FileStream(dataFile, FileMode.Create))
            {
                serializer.Serialize(fs, accountList);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi khi lưu file: {ex.Message}");
        }
    }

    public void CreateSampleXML()
    {
        accountList = new AccountList();
        accountList.Accounts.Add(new Account("user1", "123456", 10000));
        accountList.Accounts.Add(new Account("user2", "abcdef", 15000));
        accountList.Accounts.Add(new Account("user3", "qwerty", 5000));

        SaveData();
        Console.WriteLine("Tạo file dữ liệu mẫu thành công.");
    }
}
