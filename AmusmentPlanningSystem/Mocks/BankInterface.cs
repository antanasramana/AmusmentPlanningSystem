using AmusmentPlanningSystem.Models;

namespace AmusmentPlanningSystem.Mocks
{
    public interface BankInterface
    {
        public bool SendPaymentData(string password);
    }
    public class Bank : BankInterface
    {
        public bool SendPaymentData(string password)
        {
            return ValidatePaymentData(password);
        }
        private bool ValidatePaymentData(string password)
        {
            if (password == "password")
            {
                return true;
            }
            return false;
        }
    }
}
