import javax.swing.*;
public aspect password 
{
	private String account.password;
    private int account.count;
    //count = 0;
    // new procedure to change password

    private void account.changepw()
	{
	    String p = JOptionPane.showInputDialog(null,
			   "Time to change password. Enter old password:");
	    if (this.password.equals(p))
	      password = JOptionPane.showInputDialog(null, "Enter new password:");
	    else 
		{ JOptionPane.showMessageDialog(null,"you're an impostor!!!");
		  throw new Error("call the FBI!");
		}
	    //count = 0;  // reset counter
	}
    
    // advice to set password when object is first created

    after(account A) : execution(account.new (..)) && target(A)
	{
	    A.password = JOptionPane.showInputDialog(null,
			     "Enter a password and keep it secret: ");
	}    

    before(account A) : (call(public void account.deposit(..))||call(public void account.withdraw(..))) && target(A)
	{
	    A.count = 0;
        String p = JOptionPane.showInputDialog(null,
		   "Enter password:");
       while (!A.password.equals(p))
	   {
         A.count++;
         if(A.count<4)
         {
            JOptionPane.showMessageDialog(null,
            "Wrong password. Try again.");
            p = JOptionPane.showInputDialog(null,
		   "Enter password:");
         }
         else 
         {
            JOptionPane.showMessageDialog(null,
            "you're an impostor!!!");
            throw new Error("call the FBI!");
         }
       }
       if (A.count>2 && A.password.equals(p))
       {
           String r = JOptionPane.showInputDialog(null,
           "Do you want to change your password? Press y for Yes, n for No.");
           if(r.equals("y")||r.equals("Y")) A.changepw();
       }
	} 
}
