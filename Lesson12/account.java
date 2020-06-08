/*       Welcome to BIG BOSS Savings and Loan ....
   
Your assignment is to implement each of the features demanded by the
"BOSS," each in a separate aspect.  You need to statisfy all the
requirements of the BOSS or you'll get fired.  
*/

/*
BOSS: Write me a program to keep track of bank accounts!
*/

public class account
{
    private double balance;
    private String owner; 
    public account(double x, String s) { balance=x; owner=s; }
    public String owner() { return owner; }
    public void withdraw(double a) { balance -= a; }
    public void deposit(double a) { balance += a; }
    public void printbalance() { System.out.println(balance); }

    // main for testing:
  public static void main(String[] argv)
  {
      account a1 = new account(2000,"you boss");
      account a2 = new account(1000,"me nerd");
      a1.deposit(400);
      a2.withdraw(300000);   // not enough money!
      a2.withdraw(-500000);  // trying to cheat!
      a1.printbalance();
      a2.printbalance();
  }//main
} // account

/*
YOU:  It's done sir!

BOSS: You call this a program?!  Is this all that your granola-eating, 
      recursion-loving professors taught you how to do at Hofstra?  
      You can withdraw a greater amount than your current balance!  It's 
      even possible to withdraw a negative amount, which will increase 
      the balance. Deposit also doesn't check if the amount is negative.
      No wonder we're losing so much money, you useless nerd!  (1)

YOU:  I've fixed the problems in the program sir!

BOSS: How the heck is the user going to interact with your program?  Most
      people need a graphical interface in order to use a computer:
      they're not geeks like you.  (2)

YOU:  That's easy, I'll do a javax.swing interface right away!

BOSS: Your bleeding heart professors probably think there are no bad
      people in the world (kum-ba-ya!)  There's no secret pin or
      password that needs to be entered before a customer can make a
      transaction.  We don't live on a commune like your hippie
      professors.  (3)

YOU:  OK, I'll do that too.  

BOSS: While you were doing that I got another nerd to implement 
      a feature to charge a fee when a withdraw lowers the balance 
      below a certain minimum.  He can't get it to work and he blames
      you. I fired him.  But he left you some choice comments and
      you better read them.  Here's the code he was working on.  
      You need to get it to work and integrate it into your program.  
      Rewrite it if you have to.  Get it to work or I'll fire you too.  (4)
*/

/*class gougingaccount extends account
{
    public gougingaccount(double x, String s) {
		super(x, s);
		// TODO Auto-generated constructor stub
	}
	protected double minimum = 500.0; // minimum to avoid charges
    protected double fee = 3.99; // fee to be charged.
    public void withdraw(double amt)  // override super.withdraw
    {  
	if (balance-amt < minimum) amt += fee;
	super.withdraw(amt);
    }	
// NOTE TO THE MOTHERHACKER WHO WROTE THE ACCOUNT CLASS:
// This WON'T COMPILE because you made the balance value PRIVATE.
// But the boss is going to fire me instead. Good luck to the next nerd
// dumb enough to take this job.
}
*/

/*
YOU:  It would've been nice if you told me all of this up front sir.   
      I could have done things differently then. It's so frustrating to 
      not know what else you're going to ask me to do.

BOSS: Well tough! I can't think of everything at once.  It's your job
      to do what I ask, and I don't care how you do it.  Didn't you
      learn all kinds of fancy programming languages from your fancy
      pants professors?  Pick one that allows you to add these
      features incrementally, without affecting the other parts of
      your program.  Pick one that allows you to easily merge programs
      written by multiple programmers into one.

YOU:  I think you're describing what my professors would call 
      "LOOSE COUPLING" and "SEPARATION OF CONCERN."

BOSS: Now you're insulting me!  I don't care how loose your couples
      are.  What a bunch of mumbo jumbo!  By the way, other banks may
      want to buy our software.  I want to be able to package
      different sets of these features together.  Depending on how
      much they want to pay for the software, we can sell them a
      "basic" version, which can be just the first program you wrote,
      a "deluxe version", with some more features, and an "ultra"
      version for those who pay the most.  It should also be possible
      for customers to pick which features to have in their package.

YOU:  This may be possible if all the different components of the program
      are loosely coupled.  But some of these features CROSSCUT others.
      It may be difficult to WEAVE them together.

BOSS: Enough with your academic mumbo jumbo.  The customers also want to
      see a transaction record.  Your programs need to keep track of all the
      transactions (withdraws and deposits) made on an account, and print
      out a report when asked for.  (5)

YOU:  Not another feature! You're killing me boss!

BOSS: Get back to work nerd!
*/