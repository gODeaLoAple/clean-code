﻿using System;
using System.Collections.Generic;
using Markdown.Tokens;

namespace Markdown.Lexer
{
    public class LexIterator
    {
        private static readonly HashSet<char> SpecialCharacters = new()
        {
            Characters.Underscore,
            Characters.Escape,
            Characters.NewLine,
            Characters.OpenCircleBracket,
            Characters.CloseCircleBracket,
            Characters.OpenSquareBracket,
            Characters.CloseSquareBracket
        };

        private readonly string text;
        private int currentIndex;
        private bool isNewLine = true;

        public LexIterator(string text)
        {
            this.text = text ?? throw new ArgumentNullException(nameof(text));
        }

        public IEnumerable<Token> Lex()
        {
            if (text == string.Empty) yield return Token.Text(string.Empty);
            while (currentIndex < text.Length)
            {
                var symbol = text[currentIndex];
                yield return symbol switch
                {
                    Characters.Underscore => LexNonNewLineContainsToken(LexUnderscore),
                    Characters.Escape => LexNonNewLineContainsToken(LexEscape),
                    Characters.Sharp => LexHeader(),
                    Characters.NewLine => LexNewLine(),
                    Characters.OpenCircleBracket => LexNonNewLineContainsToken(LexOpenCircleBracket),
                    Characters.CloseCircleBracket => LexNonNewLineContainsToken(LexCloseCircleBracket),
                    Characters.OpenSquareBracket => LexNonNewLineContainsToken(LexOpenSquareBracket),
                    Characters.CloseSquareBracket => LexNonNewLineContainsToken(LexCloseSquareBracket),
                    _ => LexText()
                };
                currentIndex++;
            }
        }

        private Token LexNonNewLineContainsToken(Func<Token> getToken)
        {
            isNewLine = false;
            return getToken();
        }

        private static Token LexOpenCircleBracket() => Token.OpenCircleBracket;

        private static Token LexCloseCircleBracket() => Token.CloseCircleBracket;

        private static Token LexOpenSquareBracket() => Token.OpenSquareBracket;

        private static Token LexCloseSquareBracket() => Token.CloseSquareBracket;

        private Token LexUnderscore()
        {
            if (!IsNextCharacter(Characters.Underscore))
                return Token.Cursive;
            currentIndex++;
            return Token.Bold;
        }

        private static Token LexEscape() => Token.Escape;

        private Token LexHeader()
        {
            if (!isNewLine || !IsNextCharacter(Characters.WhiteSpace)) return LexText();
            currentIndex++;
            return Token.Header1;
        }

        private Token LexNewLine()
        {
            isNewLine = true;
            return Token.NewLine;
        }

        private Token LexText()
        {
            var start = currentIndex;
            var end = start + 1;
            for (; end < text.Length; end++)
            {
                if (isNewLine && text[end] == Characters.Sharp || SpecialCharacters.Contains(text[end])) break;

                isNewLine = isNewLine && char.IsWhiteSpace(text[end]);
            }

            currentIndex = end - 1;
            return Token.Text(text.Substring(start, end - start));
        }

        private bool IsNextCharacter(char ch) => currentIndex + 1 < text.Length && text[currentIndex + 1] == ch;
    }
}