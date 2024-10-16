﻿using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace ChangeTrackerOwnedEntitiesReproducible.Domain
{
    public static class Ensure
    {
        public static void NotNullOrEmpty(
            [NotNull] string? value,
            [CallerArgumentExpression("value")] string? paramName = default)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(paramName);
            }
        }
    }
}
