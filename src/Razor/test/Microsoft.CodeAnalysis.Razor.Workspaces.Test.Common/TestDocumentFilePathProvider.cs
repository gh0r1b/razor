﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT license. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.Razor.Workspaces;

namespace Microsoft.AspNetCore.Razor.LanguageServer;

internal class TestDocumentFilePathProvider : DocumentFilePathProvider
{
    public static readonly TestDocumentFilePathProvider Instance = new();

    public TestDocumentFilePathProvider(LanguageServerFeatureOptions? options = null)
        : base(options ?? TestLanguageServerFeatureOptions.Instance)
    {
    }
}
