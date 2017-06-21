namespace Domain
{
    using System;
    using Zebble.Framework;

    /// <summary>
    /// Provides the business logic for Settings class.
    /// </summary>
    partial class Settings
    {
        /// <summary>
        /// Validates this instance to ensure it can be saved in a data repository.
        /// If this finds an issue, it throws a ValidationException for that.        
        /// This calls ValidateProperties(). Override this method to provide custom validation logic in a type.
        /// </summary>
        public override void Validate(ValidationResult result)
        {
            base.Validate(result);

            if (IsNew && Database.Any<Settings>())
                throw new Exception("Settings is Singleton!");
        }
    }
}