using Lisp;
using Xunit;

namespace Tests
{
    public class PrinterTests
    {
        [Fact]
        public void SymbolReturnsText() =>
            Assert.Equal("rawr", Printer.Print(new AstSymbol("rawr")));

        [Fact]
        public void ListReturnsParens() =>
            Assert.Equal("()", Printer.Print(new AstList()));

        [Fact]
        public void FilledListReturnsSExpression() =>
            Assert.Equal("(ra wr)", Printer.Print(new AstList(new AstSymbol("ra"), new AstSymbol("wr"))));
        
        [Fact]
        public void RecursiveList() =>
            Assert.Equal("(ra ())", Printer.Print(new AstList(new AstSymbol("ra"), new AstList())));
        
    }
}