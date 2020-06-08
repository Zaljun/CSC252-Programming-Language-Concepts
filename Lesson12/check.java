import javax.swing.*;

privileged aspect check
{	
	before (account A, double withdr) : call(void account+.withdraw(..)) && args(withdr) && target(A)
	{
        if(withdr<0) 
        {
            JOptionPane.showMessageDialog(null,"Invalid Input "+withdr,"User: "+A.owner,JOptionPane.WARNING_MESSAGE);
            //System.out.println("Invalid withdraw input"+withdr); 
            System.exit(1);
        }
        if(withdr>A.balance) 
        {
            JOptionPane.showMessageDialog(null,"Not Enough Balance ","User: "+A.owner,JOptionPane.WARNING_MESSAGE);
            //System.out.println("Not enough balance"); 
            System.exit(1);
        }
    }
    
    
	before (account A, double dep) : call(void account+.deposit(..)) && target(A) && args(dep)
	{
        if(dep<0) 
        {
            JOptionPane.showMessageDialog(null,"Invalid Input "+dep,"User: "+A.owner,JOptionPane.WARNING_MESSAGE);
            //System.out.println("Invalid deposit input"+dep); 
            System.exit(1);
        }
	}
}