using Axis.Luna.Common;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;
using System;
using System.Collections.Generic;

namespace Axis.Sigma.Policy
{
    public readonly struct AttributeTarget:
        IEquatable<AttributeTarget>,
        IDefaultValueProvider<AttributeCategory>,
        IResultParsable<AttributeTarget>
    {
        private readonly AttributeCategory _category;
        private readonly string _name;

        public string Name => _name;

        public AttributeCategory Category => _category;

        public static AttributeCategory Default => default;

        public bool IsDefault => _name is null && _category == default;

        public AttributeTarget(
            AttributeCategory category,
            string name)
        {
            _category = category;
            _name = name.ThrowIf(
                string.IsNullOrWhiteSpace,
                _ => new InvalidOperationException(
                    $"Invalid attribute name: {name}"));
        }

        public bool Equals(
            AttributeTarget other)
            => _category.Equals(other._category)
            && EqualityComparer<string>.Default.Equals(_name, other._name);

        public override bool Equals(
            object? obj)
            => obj is AttributeTarget att
            && Equals(att);

        public static bool operator ==(
            AttributeTarget left,
            AttributeTarget right)
            => left.Equals(right);

        public static bool operator !=(
            AttributeTarget left,
            AttributeTarget right)
            => !(left == right);

        public override int GetHashCode()
            => HashCode.Combine(_name, _category);

        public override string ToString()
        {
            return !IsDefault
                ? $"{_category.CategoryCode()}:{NormalizeName()}"
                : ".";
        }

        private string? NormalizeName()
        {
            if (_name is null)
                return null;

            // what this really needs to test is if the name is not a conventional "identifier"
            if (_name.Contains(' '))
                return $"'{_name}'";

            return _name;
        }

        #region Result Parsable
        public static bool TryParse(string text, out IResult<AttributeTarget> result)
        {
            result = Parse(text);
            return result.IsDataResult();
        }

        public static IResult<AttributeTarget> Parse(string text)
        {
            ArgumentException.ThrowIfNullOrEmpty(text);

            var parts = text.Split(':');

            if (parts.Length != 2)
                return Result.Of<AttributeTarget>(new FormatException(
                    $"Invalid format: '{text}'"));

            return Result
                .Of(() => parts[0].CategoryValue())
                .Map(cat => new AttributeTarget(cat, parts[1]));
        }
        #endregion
    }
}
