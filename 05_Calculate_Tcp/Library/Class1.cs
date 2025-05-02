namespace Library
{
    public class Reg
    {
        public int One { get; set; }
        public int Two { get; set; }
        public string Operation { get; set; }
        public int Calculate()
        {
            switch (Operation)
            {
                case "+":
                    return One + Two;

                case "-":
                    return One - Two;

                case "*":
                    return One * Two;

                case "/":
                    return One / Two;
            }
            return 0;
        }
    }
}
