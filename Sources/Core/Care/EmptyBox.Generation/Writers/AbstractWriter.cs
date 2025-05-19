using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace EmptyBox.Generation.Writers;

internal abstract class AbstractWriter<W>(string initial = "")
    where W : AbstractWriter<W>
{
    private class Scope : IDisposable
    {
        private readonly ScopeBracketFraming Frame;
        private readonly AbstractWriter<W> Source;

        private ScopeParameters Parameters;
        private bool IsDisposed = false;

        public Scope(AbstractWriter<W> source, ScopeBracketFraming frame, ScopeParameters parameters)
        {
            Source = source;
            Frame = frame;
            Parameters = parameters;

            if (parameters.HasFlag(ScopeParameters.Deferred))
            {
                Source.DeferredScopes.Add(this);
            }
            else
            {
                Do();
            }
        }

        public void Do()
        {
            Parameters &= ~ScopeParameters.Deferred;

            switch (Parameters & ScopeParameters.NoIndent)
            {
                case ScopeParameters.Default:
                    Source._IndentLevel += 4;

                    switch (Frame)
                    {
                        case ScopeBracketFraming.None: Source.Append(' ', 4); break;
                        case ScopeBracketFraming.Angle: Source.AppendLine('<'); break;
                        case ScopeBracketFraming.Curly: Source.AppendLine('{'); break;
                        case ScopeBracketFraming.Round: Source.AppendLine('('); break;
                        case ScopeBracketFraming.Square: Source.AppendLine('['); break;
                    }

                    break;
                case ScopeParameters.NoIndent:
                    switch (Frame)
                    {
                        case ScopeBracketFraming.Angle: Source.Append('<'); break;
                        case ScopeBracketFraming.Curly: Source.Append('{'); break;
                        case ScopeBracketFraming.Round: Source.Append('('); break;
                        case ScopeBracketFraming.Square: Source.Append('['); break;
                    }

                    break;
            }
        }

        public void Dispose()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(nameof(Scope));
            }
            else
            {
                IsDisposed = true;

                if (Parameters.HasFlag(ScopeParameters.Deferred))
                {
                    Source.DeferredScopes.Remove(this);
                    return;
                }

                switch (Parameters)
                {
                    case ScopeParameters.Default:
                        Source._IndentLevel -= 4;
                        Source.AppendLine();

                        break;
                }

                switch (Frame)
                {
                    case ScopeBracketFraming.Angle: Source.Append('>'); break;
                    case ScopeBracketFraming.Curly: Source.Append('}'); break;
                    case ScopeBracketFraming.Round: Source.Append(')'); break;
                    case ScopeBracketFraming.Square: Source.Append(']'); break;
                }
            }
        }
    }

    [SuppressMessage("MicrosoftCodeAnalysisCorrectness", "RS1035:Do not use APIs banned for analyzers", Justification = "Иначе никак")]
    public static string NewLine => Environment.NewLine;

    private readonly StringBuilder ActualWriter = new(initial);
    private readonly List<Scope> DeferredScopes = [];

    private int _IndentLevel = 0;

    public int IndentLevel
    {
        get => _IndentLevel;
        init => _IndentLevel = value;
    }
    public int Length => ActualWriter.Length;

    private void ProcessDeferred()
    {
        Scope[] scopes = DeferredScopes.ToArray();
        DeferredScopes.Clear();

        foreach (Scope scope in scopes)
        {
            scope.Do();
        }
    }

    public W Append<T>(T value)
    {
        ProcessDeferred();
        ActualWriter.Append(value);
        return (W)this;
    }

    public W Append(char value, int count = 1)
    {
        ProcessDeferred();
        ActualWriter.Append(value, count);
        return (W)this;
    }

    public W Append(string value)
    {
        ProcessDeferred();
        ActualWriter.Append(value);
        return (W)this;
    }

    public W AppendJoin<T>(IEnumerable<T> symbols, Func<W, T, W>? formatter = default, string separator = ", ")
    {
        formatter ??= static (writer, item) => writer.Append(item);
        IEnumerator<T> enumerator = symbols.GetEnumerator();

        if (enumerator.MoveNext())
        {
            formatter((W)this, enumerator.Current);
        }

        while (enumerator.MoveNext())
        {
            if (separator == NewLine)
            {
                AppendLine();
            }
            else
            {
                Append(separator);
            }

            formatter((W)this, enumerator.Current);
        }

        return (W)this;
    }

    public W AppendLine()
    {
        ProcessDeferred();
        ActualWriter.AppendLine();
        ActualWriter.Append(' ', _IndentLevel);
        return (W)this;
    }

    public W AppendLine(string text)
    {
        ProcessDeferred();
        ActualWriter.AppendLine(text);
        ActualWriter.Append(' ', _IndentLevel);
        return (W)this;
    }

    public W AppendLine(char value)
    {
        ProcessDeferred();
        ActualWriter.Append(value)
                    .AppendLine()
                    .Append(' ', _IndentLevel);
        return (W)this;
    }

    public IDisposable AppendScope(ScopeBracketFraming frame = ScopeBracketFraming.Curly, ScopeParameters indent = ScopeParameters.Default)
    {
        return new Scope(this, frame, indent);
    }

    public override string ToString()
    {
        return ActualWriter.ToString();
    }
}
