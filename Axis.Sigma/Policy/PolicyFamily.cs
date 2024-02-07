using Axis.Luna.Common;
using Axis.Luna.Common.Results;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Axis.Sigma.Policy
{
    public readonly struct PolicyFamily:
        IEquatable<PolicyFamily>,
        IResultParsable<PolicyFamily>,
        IDefaultValueProvider<PolicyFamily>
    {
        internal readonly static Regex NamePattern = new(
            "^[a-zA-Z_][0-9a-zA-Z_-]*$",
            RegexOptions.Compiled);

        internal readonly static Regex NamespacePattern = new(
            "^spfn(:[a-zA-Z_][0-9a-zA-Z_-]*)+$",
            RegexOptions.Compiled);

        private readonly string _namespace;
        private readonly string _name;

        public string Name => _name;

        public string Namespace => _namespace;

        public bool IsDefault
            => _name is null
            && _namespace is null;

        public static PolicyFamily Default => default;

        #region Construction
        private PolicyFamily(
            string @namespace,
            string name)
        {
            _name = name;
            _namespace = @namespace;
        }

        public static PolicyFamily Of(
            string urn)
            => Parse(urn).Resolve();

        public static implicit operator PolicyFamily(
            string urn)
            => Parse(urn).Resolve();

        public static implicit operator string(
            PolicyFamily policyFamily)
            => policyFamily.ToString();

        #endregion

        #region Parse
        public static bool TryParse(string urn, out IResult<PolicyFamily> result)
        {
            if (string.IsNullOrWhiteSpace(urn))
                throw new ArgumentException($"Invalid {nameof(urn)}: null/empty/whitespace");

            var nameDelimiter = urn.LastIndexOf(':');

            if (nameDelimiter == -1)
            {
                result = Result.Of<PolicyFamily>(new FormatException(
                    $"Invalid format: missing namespace delimiter ':'"));
                return false;
            }

            if (nameDelimiter == urn.Length - 1)
            {
                result = Result.Of<PolicyFamily>(new FormatException(
                    $"Invalid format: missing policy family name"));
                return false;
            }

            var @namespace = urn[..nameDelimiter];
            if (!NamespacePattern.IsMatch(@namespace))
            {
                result = Result.Of<PolicyFamily>(new FormatException(
                    $"Invalid format: invalid namespace '{@namespace}'"));
                return false;
            }

            var name = urn[nameDelimiter..];
            if (!NamePattern.IsMatch(name))
            {
                result = Result.Of<PolicyFamily>(new FormatException(
                    $"Invalid format: invalid name '{name}'"));
                return false;
            }

            result = Result.Of(new PolicyFamily(@namespace, name));
            return true;
        }

        public static IResult<PolicyFamily> Parse(string text)
        {
            _ = TryParse(text, out var result);
            return result;
        }
        #endregion

        #region Equality

        public bool Equals(
            PolicyFamily other)
            => EqualityComparer<string>.Default.Equals(_name, other._name)
            && EqualityComparer<string>.Default.Equals(_namespace, other._namespace);

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is PolicyFamily other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_name, _namespace);
        }

        #endregion

        public override string ToString()
        {
            return !IsDefault
                ? $"{_namespace}:{_name}"
                : "*";
        }
    }
}
