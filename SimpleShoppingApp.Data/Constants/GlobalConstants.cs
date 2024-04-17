namespace SimpleShoppingApp.Data.Constants
{
    public static class GlobalConstants
    {
        public const int UserMaxLength = 50;
        public const int CategoryNameMaxLength = 100;
        public const int ImageExtensionMaxLength = 6;
        public const int NotificationTextMaxLength = 100;
        public const int PhoneNumberMaxLength = 10;
        public const int ProductNameMinLength = 5;
        public const int ProductNameMaxLength = 500;
        public const int ProductDescriptionMinLength = 10;
        public const int AddressNameMaxLength = 100;
        public const int AddressNameMinLength = 10;
        public const int PasswordMinLength = 6;
        public const int EmailMinLength = 6;
        public const int EmailMaxLength = 100;
        public const int FirstLastNameMinLength = 2;
        public const int FirstLastNameMaxLength = 50;
        public const int FullNameMinLength = 5;
        public const int FullNameMaxLength = 100;
        public const string ProductPriceMinRange = "0.50";
        public const string ProductPriceMaxRange = "10000";
        public const string ProductQuantityMinRange = "1";
        public const string EditProductQuantityMinRange = "0";
        public const string ProductQuantityMaxRange = "1000000";

        public const string CurrentPasswordDisplay = "Current Password";
        public const string NewPasswordDisplay = "New Password";
        public const string ConfirmNewPasswordDisplay = "Confirm New Password";
        public const string ConfirmPasswordDisplay = "Confirm Password";
        public const string RememberMeDisplay = "Remember me";
        public const string FirstNameDisplay = "First Name";
        public const string LastNameDisplay = "Last Name";
        public const string PhoneNumberDisplay = "Phone Number";
        public const string AddressDisplay = "Address";
        public const string AddressIdDisplay = "Shipping Address";
        public const string PaymentMethodDisplay = "Payment Method";
        public const string ImagesUrlsDisplay = "Images Links";
        public const string HasDiscountDisplay = "Has Discount";
        public const string NewPriceDisplay = "New Price";
        public const string CategoryIdDisplay = "Category";
        public const string UsernameDisplay = "Username";

        public const string NoProductsErrorMessage = "You should add at least one product to proceed with the order";
        public const string NoImagesErrorMessage = "Please provide at least one url or at least one image";
        public const string ImagesUrlsRegex = "^((http|https)://)[-a-zA-Z0-9@:%._\\+~#?&//=]{2,256}\\.[a-z]{2,6}\\b([-a-zA-Z0-9@:%._\\+~#?&//=]*)$";
        public const string PhoneRegex = "^([+]?359)|0?(|-| )8[789]\\d{1}(|-| )\\d{3}(|-| )\\d{3}$";
        public const string FirstLastNameRegex = "^[a-zA-Z]+$";
        public const string FullNameRegex = "^[a-zA-Z]{2,50} [a-zA-Z]{2,50}$";
        public const string PhoneNumberErrorMessage = "Please provide valid Bulgarian phone number";
        public const string FullNameErrorMessage = "Invalid Full Name. Please make sure it contains space to separate first and last name and you use latin letters only and both first and last name should be between 2 to 50 characters long";
        public const string FirstNameErrorMessage = "Invalid First Name. Please make sure you use latin letters only";
        public const string LastNameErrorMessage = "Invalid Last Name. Please make sure you use latin letters only";
        public const string InvalidImageUrlErrorMessage = "Invalid image url";
        public const string HasDiscountErrorMessage = "Please check Has Discount or delete the specified new price";
        public const string NewPriceErrorMessage = "Please specify new price";


    }
}
