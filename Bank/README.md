Usage
------
The program is best run and built using Visual Studio.  Input is processed through command line arguments passed through the
"bank.exe" executable.
### Arguments
  -a, --amount      (Default: 0) The amount of the transaction
  -n, --date        Specify a date for displaying the balance of an account (mm/dd/yyyy format)
  -d, --deposit     Deposit money into an account
  -w, --withdraw    Withdraw money from an account
  -c, --create      Create a new account
  -t, --top         Display the balances of the top 5 accounts with the highest balances
  -m, --bottom      Display the balances of the bottom 5 accounts with the smallest balances
  -l, --list        Display balances for all accounts
  -b, --balance     Display the balance of an account
  -e, --euro        (Default: False) Displays all balances in the Euro currency
  -s, --tests       (Default: False) Run tests
  --help            Display this help screen.

### Examples
* Create a new account
	bank --create
* Deposit $20 into account e2405331
	bank --deposit e2405331 --amount 20
* List the balance of account e2405331 on 1/4/2015 in Euros
	bank --balance e2405331 --euro --date 1/4/2015  
* List the top 5 accounts with the highest balances
	bank -t

Design
------
### Environment
This program has been successfully tested on Windows 8 running Visual Studio 2013 version "12.0.31101.00 Update 4"
 and with the .NET runtime of "Version 4.5.51641".  Additional packages managed by Nuget version "2.8.50926.663".  
 The solution file "Bank2010.sln" is set to be compatible with Visual Studio 2010 but has not been tested in that environment.

### Organization 
All user interface is dealt through the command line with the "bank.exe" executable.  Each deposit and withdrawal is handled 
as a separate transaction with the final result of the action being stored as well as the account number and the exact time 
handled.  Requests for a balance at a specific date are considered to be a request for the final balance of the last 
transaction that occurred on that day.


### Assumptions
There were a few assumptions I made that were not explicitly included in the prompt:
1. Accounts are identified by a unique identifier
2. The Euro conversion rate is set at 1.20 US dollars
3. Unrecognised actions at the command line interface are ignored
4. Transactions without a date specified are considered to be processed on the current day
5. If there are less than five customers in the system, all available accounts are shown when displaying the 
largest/smallest balances
6. Objects are serialized between transactions to the filesystem using the json format
7. Any transactions that would cause an invalid balance (less than zero), creates an error and is not processed
8. Only positive amounts are valid for any deposit or withdrawal transaction
9. Fractions of cents are allowed in all transactions
10. Requests for balances at a future date return the most current balance.

### Tests
I aimed for test coverage based on the different branches of the code.  Tests can be run from the executable by running it 
with the --tests option.  The tests are successful if the prompt returns "Ran all tests successfully".