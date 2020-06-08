import javax.swing.*;

privileged aspect interf
{
    after(account A) : execution(account+.new(..)) && target(A)
    {
        JOptionPane.showMessageDialog(null,"You have created an account, your user name is: "+A.owner);
    }

    after(account A,double dep): execution(void account+.deposit(..)) && target(A) && args(dep)
    {
        JOptionPane.showMessageDialog(null,"Deposit "+dep,"User: "+A.owner,JOptionPane.WARNING_MESSAGE);
        JOptionPane.showMessageDialog(null,"Your Current Balance: "+A.balance,"User: "+A.owner,JOptionPane.WARNING_MESSAGE);
    }

    after(account A,double withdr): execution(void account+.withdraw(..)) && target(A) && args(withdr)
    {
        JOptionPane.showMessageDialog(null,"Withdraw "+withdr,"User: "+A.owner,JOptionPane.WARNING_MESSAGE);
        JOptionPane.showMessageDialog(null,"Your Current Balance: "+A.balance,"User: "+A.owner,JOptionPane.WARNING_MESSAGE);
    }
}