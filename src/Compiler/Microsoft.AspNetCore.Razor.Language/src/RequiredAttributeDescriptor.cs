﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNetCore.Razor.Language;

public abstract class RequiredAttributeDescriptor : IEquatable<RequiredAttributeDescriptor>
{
    private enum Flags : byte
    {
        ContainsDiagnostics = 0x01,
        CaseSensitive = 0x02
    }

    private Flags _flags;

    private bool HasFlag(Flags flag) => (_flags & flag) != 0;
    private void SetFlag(Flags flags) => _flags |= flags;
    private void ClearFlag(Flags flags) => _flags &= ~flags;

    private void SetOrClearFlag(Flags flag, bool value)
    {
        if (value)
        {
            SetFlag(flag);
        }
        else
        {
            ClearFlag(flag);
        }
    }

    public string Name { get; protected set; }

    public NameComparisonMode NameComparison { get; protected set; }

    public bool CaseSensitive
    {
        get => HasFlag(Flags.CaseSensitive);
        protected set => SetOrClearFlag(Flags.CaseSensitive, value);
    }

    public string Value { get; protected set; }

    public ValueComparisonMode ValueComparison { get; protected set; }

    public string DisplayName { get; protected set; }

    public IReadOnlyList<RazorDiagnostic> Diagnostics
    {
        get => HasFlag(Flags.ContainsDiagnostics)
            ? TagHelperDiagnostics.GetDiagnostics(this)
            : Array.Empty<RazorDiagnostic>();

        protected set
        {
            if (value?.Count > 0)
            {
                TagHelperDiagnostics.AddDiagnostics(this, value);
                SetFlag(Flags.ContainsDiagnostics);
            }
            else
            {
                ClearFlag(Flags.ContainsDiagnostics);
            }
        }
    }

    public IReadOnlyDictionary<string, string> Metadata { get; protected set; }

    public bool HasErrors
    {
        get
        {
            if (!HasFlag(Flags.ContainsDiagnostics))
            {
                return false;
            }

            var errors = Diagnostics.Any(diagnostic => diagnostic.Severity == RazorDiagnosticSeverity.Error);

            return errors;
        }
    }

    public override string ToString()
    {
        return DisplayName ?? base.ToString();
    }

    public bool Equals(RequiredAttributeDescriptor other)
    {
        return RequiredAttributeDescriptorComparer.Default.Equals(this, other);
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as RequiredAttributeDescriptor);
    }

    public override int GetHashCode()
    {
        return RequiredAttributeDescriptorComparer.Default.GetHashCode(this);
    }

    /// <summary>
    /// Acceptable <see cref="RequiredAttributeDescriptor.Name"/> comparison modes.
    /// </summary>
    public enum NameComparisonMode
    {
        /// <summary>
        /// HTML attribute name case insensitively matches <see cref="RequiredAttributeDescriptor.Name"/>.
        /// </summary>
        FullMatch,

        /// <summary>
        /// HTML attribute name case insensitively starts with <see cref="RequiredAttributeDescriptor.Name"/>.
        /// </summary>
        PrefixMatch,
    }

    /// <summary>
    /// Acceptable <see cref="RequiredAttributeDescriptor.Value"/> comparison modes.
    /// </summary>
    public enum ValueComparisonMode
    {
        /// <summary>
        /// HTML attribute value always matches <see cref="RequiredAttributeDescriptor.Value"/>.
        /// </summary>
        None,

        /// <summary>
        /// HTML attribute value case sensitively matches <see cref="RequiredAttributeDescriptor.Value"/>.
        /// </summary>
        FullMatch,

        /// <summary>
        /// HTML attribute value case sensitively starts with <see cref="RequiredAttributeDescriptor.Value"/>.
        /// </summary>
        PrefixMatch,

        /// <summary>
        /// HTML attribute value case sensitively ends with <see cref="RequiredAttributeDescriptor.Value"/>.
        /// </summary>
        SuffixMatch,
    }
}
