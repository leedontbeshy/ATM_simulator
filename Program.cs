using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace ATMApp
{

    
    [Serializable]
    public class Account
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public decimal Balance { get; set; }
    }

    [Serializable]
    [XmlRoot("Accounts")] // Tên root trong XML
    public class AccountList
    {
        [XmlElement("Account")]
        public List<Account> Accounts { get; set; } = new List<Account>();
    }

    public class ATMSystem
    {
        private AccountList accountList = new AccountList();
        private Account currentAccount;
        private readonly string dataFile = "accounts.xml";

        public ATMSystem()
        {
            LoadData();
        }

        public void Run()
        {
            Console.WriteLine("===== ATM SYSTEM =====");

            if (!Login())
            {
                Console.WriteLine("Đăng nhập thất bại! Thoát chương trình.");
                return;
            }

            int choice = -1;
            do
            {
                Console.WriteLine("\n----- Menu -----");
                Console.WriteLine("1. Rút tiền");
                Console.WriteLine("2. Xem số dư");
                Console.WriteLine("3. Thoát");
                Console.Write("Chọn: ");

                bool isNumber = int.TryParse(Console.ReadLine(), out choice);
                if (!isNumber)
                {
                    Console.WriteLine("Vui lòng nhập số!");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        Withdraw();
                        break;
                    case 2:
                        ShowBalance();
                        break;
                    case 3:
                        Console.WriteLine("Cảm ơn bạn đã sử dụng ATM!");
                        break;
                    default:
                        Console.WriteLine("Lựa chọn không hợp lệ.");
                        break;
                }

            } while (choice != 3);
        }

        private bool Login()
        {
            Console.Write("Nhập tên tài khoản: ");
            string username = Console.ReadLine();

            Console.Write("Nhập mật khẩu: ");
            string password = Console.ReadLine();

            currentAccount = accountList.Accounts.Find(acc => acc.Username == username && acc.Password == password);

            if (currentAccount != null)
            {
                Console.WriteLine("Đăng nhập thành công!");
                return true;
            }
            else
            {
                Console.WriteLine("Sai tài khoản hoặc mật khẩu!");
                return false;
            }
        }

        private void Withdraw()
        {
            try
            {
                Console.Write("Nhập số tiền muốn rút: ");
                bool isDecimal = decimal.TryParse(Console.ReadLine(), out decimal amount);

                if (!isDecimal || amount <= 0)
                {
                    Console.WriteLine("Số tiền không hợp lệ!");
                    return;
                }

                if (currentAccount.Balance >= amount)
                {
                    currentAccount.Balance -= amount;
                    SaveData();

                    Console.WriteLine($"Đã rút {amount} VND thành công.");
                    Console.WriteLine($"Số dư còn lại: {currentAccount.Balance} VND");

                    // Thông báo tin nhắn (giả lập)
                    Console.WriteLine($"[SMS] Bạn vừa rút {amount} VND. Số dư: {currentAccount.Balance} VND");
                }
                else
                {
                    Console.WriteLine("Số dư không đủ!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Đã xảy ra lỗi khi rút tiền: {ex.Message}");
            }
        }

        private void ShowBalance()
        {
            Console.WriteLine($"Số dư hiện tại: {currentAccount.Balance} VND");
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
                }
                else
                {
                    Console.WriteLine("Không tìm thấy file dữ liệu, tạo file mới...");
                    CreateSampleXML();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi đọc file dữ liệu: {ex.Message}");
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
                Console.WriteLine($"Lỗi khi lưu file dữ liệu: {ex.Message}");
            }
        }

        public void CreateSampleXML()
        {
            try
            {
                var sampleAccounts = new AccountList();
                sampleAccounts.Accounts.Add(new Account { Username = "user1", Password = "123456", Balance = 5000 });
                sampleAccounts.Accounts.Add(new Account { Username = "user2", Password = "abcdef", Balance = 10000 });

                XmlSerializer serializer = new XmlSerializer(typeof(AccountList));
                using (FileStream fs = new FileStream(dataFile, FileMode.Create))
                {
                    serializer.Serialize(fs, sampleAccounts);
                }

                Console.WriteLine("File dữ liệu mẫu đã được tạo!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi tạo file mẫu: {ex.Message}");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            ATMSystem atm = new ATMSystem();



            atm.Run();

            Console.WriteLine("Nhấn phím bất kỳ để thoát...");
            Console.ReadKey();
        }
    }
}
