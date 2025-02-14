﻿using System.Collections.Generic;
using Markdown.Tags;
using Markdown.Tokens;

namespace Markdown.TagRenderer
{
    public interface ITagRenderer
    {
        string Render(IEnumerable<TagNode> tokens);
    }
}