using System.ComponentModel.DataAnnotations;

namespace Common.Extentions;

public class LanguageCodeValidation : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        var allowedCodes = new string[] { "uz", "ru", "en" };
        if (allowedCodes.Contains(value))
            return true;

        return false;
    }
}