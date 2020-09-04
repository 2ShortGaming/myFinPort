using myFinPort.Enums;

namespace myFinPort.ViewModels
{
    public class BankAccountWizardVM
    {
        public decimal StartingBalance { get; set; }
        public decimal WarningBalance { get; set; }
        public string AccountName { get; set; }
        public AccountType AccountType { get; set; }

    }
}