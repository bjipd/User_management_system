using FluentValidation.Results;
using System.Collections.Generic;
using System.Linq;

namespace Application.Extensions;

public static class ValidationResultExtensions
{
    public static IDictionary<string, string[]> ToDictionary(this ValidationResult result)
    {
        return result.Errors
            .GroupBy(e => string.IsNullOrWhiteSpace(e.PropertyName) ? "_" : e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
    }
}
