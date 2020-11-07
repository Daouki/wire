using System.Collections.Generic;
using System.Diagnostics;

using WireC.AST;
using WireC.AST.Statements;
using WireC.AST.Types;

namespace WireC.MiddleEnd
{
    public class Scope
    {
        /// <summary>
        /// Symbols defined in this scope.
        /// </summary>
        private readonly List<Symbol> _symbols = new List<Symbol>();

        public Scope(Scope outer = null)
        {
            Outer = outer;
        }

        /// <summary>
        /// Scope that encloses this scope. May be null - if it is, it means that this scope
        /// is a global scope.
        /// </summary>
        public Scope Outer { get; }

        /// <summary>
        /// Define a new symbol created from a function definition.
        /// </summary>
        /// <param name="function">Function to be defined as a symbol.</param>
        /// <param name="functionType">Type of that function.</param>
        /// <returns>True if function was defined successfully; false if it wasn't.</returns>
        public bool DefineSymbol(FunctionDefinition function, IType functionType)
        {
            Debug.Assert(functionType is FunctionType);

            if (IsSymbolDefinedLocally(function.Name.Lexeme)) return false;
            _symbols.Add(
                new Symbol
                {
                    Name = function.Name,
                    Type = functionType,
                }
            );
            return true;
        }

        public bool IsSymbolDefined(string name)
        {
            if (IsSymbolDefinedLocally(name)) return true;
            return Outer != null && Outer.IsSymbolDefined(name);
        }

        public bool IsSymbolDefinedLocally(string name)
        {
            return _symbols.Exists(symbol => symbol.Name.Lexeme == name);
        }

        public Symbol GetSymbol(string name)
        {
            return _symbols.Exists(symbol => symbol.Name.Lexeme == name)
                ? _symbols.Find(symbol => symbol.Name.Lexeme == name)
                : Outer?.GetSymbol(name);
        }
    }
}
