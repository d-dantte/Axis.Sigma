using System;

namespace Axis.Sigma.Policy
{
    public enum AttributeCategory
    {
        Subject,
        Resource,
        Intent,
        Environment
    }

    public static class AttributeCategoryExtensions
    {
        public static string CategoryCode(this
            AttributeCategory category)
            => category switch
            {
                AttributeCategory.Subject => "sub",
                AttributeCategory.Resource => "res",
                AttributeCategory.Intent => "int",
                AttributeCategory.Environment => "env",
                _ => throw new InvalidOperationException(
                    $"Invalid category: '{category}'")
            };

        public static AttributeCategory CategoryValue(this
            string categoryCode)
            => categoryCode?.ToLower() switch
            {
                "sub" => AttributeCategory.Subject,
                "res" => AttributeCategory.Resource,
                "int" => AttributeCategory.Intent,
                "env" => AttributeCategory.Environment,
                _ => throw new InvalidOperationException(
                    $"Invalid category code: '{categoryCode}'")
            };
    }
}
