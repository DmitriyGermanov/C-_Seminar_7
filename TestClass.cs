namespace Seminar_7
{
/*    
 *    Дан класс(ниже), создать методы создающий этот класс вызывая один из его конструкторов(по одному конструктору на метод). Задача не очень сложна и служит больше для разогрева перед следующей задачей.
 */
class TestClass
    {
        [CustomName("IntAtr")]
        public int I { get; set; }
        [CustomName("StringAtr")]
        public string? S { get; set; }
        [CustomName("DecimalAtr")]
        public decimal D { get; set; }
        [CustomName("Char[]Atr")]
        public char[]? C { get; set; }

        public TestClass()
        { }
        private TestClass(int i)
        {
            this.I = i;
        }
        public TestClass(int i, string s, decimal d, char[] c) : this(i)
        {
            this.S = s;
            this.D = d;
            this.C = c;
        }
    }
}
