using System;

namespace ATMApp
{
    class Program
    {
        static void Main(string[] args)
        {
            ATMSystem atm = new ATMSystem();
            atm.Run();

            Console.WriteLine("Chương trình kết thúc.");
            Console.ReadKey();
        }
    }
}
