using System;
using System.Text.RegularExpressions;

namespace backend.Models {
    public class RegisterDto {
        public string Username { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;


        public List<string> checkValidation() {
            var errors = new List<string>();
            if (!IsValid()) {
                errors.Add("All fields are required.");
                return errors;
            }
            if (!IsValidPassword()) {
                errors.Add("Password must be at least 8 characters long, contain at least one uppercase letter, one lowercase letter, one digit, and one special character.");
            }
            if (!isValidEmail()) {
                errors.Add("Invalid email format.");
            }
            return errors;
        }

        private bool IsValid() {
            return !string.IsNullOrEmpty(Username) 
                && !string.IsNullOrEmpty(FirstName) 
                && !string.IsNullOrEmpty(LastName) 
                && !string.IsNullOrEmpty(Email) 
                && !string.IsNullOrEmpty(Password);
        }
        private bool IsValidPassword() {
            HashSet<char> specialChars = new HashSet<char> { '!', '@', '#', '$', '%', '^', '&', '*'};
            return !string.IsNullOrEmpty(Password)
                && Password.Length >= 8
                && Password.Any(char.IsUpper)
                && Password.Any(char.IsLower)
                && Password.Any(char.IsDigit)
                && Password.Any(specialChars.Contains);
        }
        private bool isValidEmail() {
            if (string.IsNullOrEmpty(Email)) {
                return false;
            }

            // Define a regex pattern for email validation
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return emailRegex.IsMatch(Email);
        }
    }
    public class  LoginDto {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
    public class UserDTO {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
       
    }

    public class ChangePasswordDto {
        public string OldPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;

        public List<string> checkValidation()
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(OldPassword))
            {
                errors.Add("Old password is required.");
            }

            if (string.IsNullOrEmpty(NewPassword))
            {
                errors.Add("New password is required.");
            }

            if (NewPassword.Length < 8)
            {
                errors.Add("New password must be at least 8 characters long.");
            }

            if (OldPassword == NewPassword)
            {
                errors.Add("New password must be different from the old password.");
            }

            return errors;
        }
    }
}
