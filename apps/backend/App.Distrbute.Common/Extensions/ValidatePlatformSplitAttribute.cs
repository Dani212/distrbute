using System.ComponentModel.DataAnnotations;
using App.Distrbute.Common.Models;

namespace App.Distrbute.Common.Extensions;

public static class AttributeExtensions
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ValidatePlatformSplitAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is List<PlatformSplit> splits)
            {
                var fieldName = validationContext.DisplayName ?? validationContext.MemberName;

                switch (splits.Count)
                {
                    case 0:
                        return new ValidationResult($"At least one element required for field: {fieldName}.");
                        break;
                    // if it's one split, percentage must be 100
                    case 1 when Math.Abs(splits.First().Split - 100) > 0.00:
                        return new ValidationResult($"Platform split must be 100% for field: {fieldName}.");
                }

                var totalPercentage = splits.Sum(s => s.Split);

                if (Math.Abs(totalPercentage - 100) > 0.00)
                    return new ValidationResult($"Platform splits should equal 100% for field: {fieldName}.");
            }

            return ValidationResult.Success;
        }
    }
}