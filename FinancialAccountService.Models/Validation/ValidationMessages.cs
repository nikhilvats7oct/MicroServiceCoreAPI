namespace FinancialAccountService.Models.Validation
{
    public static class ValidationMessages
    {
        public const string InvalidEmailAddress = "Please enter a valid email address";
        public const string MismatchEmailAddress = "Please ensure the emails match";

        public const string BadFormatPassword = "Password must be between 8 and 15 characters, and contain at least one lowercase, one uppercase and one numeric character";
        public const string MismatchPassword = "Please ensure the passwords match";

        public const string MissingLowellRef = "Lowell reference required";
        public const string InvalidLowellRef = "Invalid Lowell reference";

        //Registration
        public const string DataProtectionCheckFailed = "The account information you have entered does not match our records. Please check the information you are entering is correct. If the problem continues contact us on 0333 556 5551. Our opening times are Mon – Fri 8:00am–8:00pm and Sat 8.00am-2:00pm";
        public const string AccountAlreadyWebRegistered = "Account Already Web Registered";
        public const string SendRegistrationEmailError = "Error sending registration email.";
        public const string SendConfirmEmailEmailError = "Error sending confirmation email.";
        public const string CompleteRegistrationError = "Error completing registration.";
        public const string SendForgotPasswordEmailError = "Error sending forgot password email.";
    }
}