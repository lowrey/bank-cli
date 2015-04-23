using System;
using CommandLine;
using CommandLine.Text;

namespace Bank
{
    /*
    -a  --amount 
    -b --balance [id]
    -c --create
    -d --deposit [id]
    -n --date (mm/dd/yyyy)
    -w --withdraw [id]
    -t --top
    -l --bottom
    -e --euro
    -s --tests
    -h --help
     */

    internal class Options
    {
        [Option('a', "amount", DefaultValue = 0.0,
            HelpText = "The amount of the transaction")]
        public double Amount { get; set; }

        [Option('n', "date",
            HelpText = "Specify a date for displaying the balance of an account (mm/dd/yyyy format)")]
        public string Date { get; set; }

        [Option('d', "deposit",
            HelpText = "Deposit money into an account")]
        public string Deposit { get; set; }

        [Option('w', "withdraw",
            HelpText = "Withdraw money from an account")]
        public string Withdraw { get; set; }

        [Option('c', "create",
            HelpText = "Create a new account")]
        public bool NewAccount { get; set; }

        [Option('t', "top",
            HelpText = "Display the balances of the top 5 accounts with the highest balances")]
        public bool Top { get; set; }

        [Option('m', "bottom",
            HelpText = "Display the balances of the bottom 5 accounts with the smallest balances")]
        public bool Bottom { get; set; }

        [Option('l', "list",
            HelpText = "Display balances for all accounts")]
        public bool ListAll { get; set; }

        [Option('b', "balance", Required = false,
            HelpText = "Display the balance of an account")]
        public string BalanceId { get; set; }

        [Option('e', "euro", DefaultValue = false,
            HelpText = "Displays all balances in the Euro currency")]
        public bool Euro { get; set; }

        [Option('s', "tests", DefaultValue = false,
            HelpText = "Run tests")]
        public bool Test { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
                (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            var ledger = Ledger.Load();
            var options = new Options();
            if (Parser.Default.ParseArguments(args, options))
            {
                Console.WriteLine(new Action(options, ledger).Run());
            }
            ledger.Save();
        }
    }
}