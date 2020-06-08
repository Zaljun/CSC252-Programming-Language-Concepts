import javax.swing.*;

privileged aspect gouge 
{
	private double account.minimum = 500.0;
	private double account.fee = 3.99;
	after(account A) : call (void account+.withdraw(..)) && target(A)
	{
		if(A.balance < A.minimum && A.balance >= A.fee) 
		{
			A.balance -= A.fee;
			JOptionPane.showMessageDialog(null,"Account below minimum, charge fee 3.99","User: "+A.owner,JOptionPane.WARNING_MESSAGE);
			JOptionPane.showMessageDialog(null,"Your Current Balance: "+A.balance,"User: "+A.owner,JOptionPane.WARNING_MESSAGE);
		}
		if(A.balance < A.minimum && A.balance < A.fee)
		{
			A.balance = 0;
			JOptionPane.showMessageDialog(null,"Account below charge fee, mercy on you","User: "+A.owner,JOptionPane.WARNING_MESSAGE);
			JOptionPane.showMessageDialog(null,"Your Current Balance: "+A.balance,"User: "+A.owner,JOptionPane.WARNING_MESSAGE);
		}
		//System.out.println("account below minimum, charge fee 3.99");
	}
}