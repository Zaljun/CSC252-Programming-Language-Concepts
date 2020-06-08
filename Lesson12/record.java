import javax.swing.*;
import java.util.*;

aspect record
{
    private LinkedList account.depositl = new LinkedList<>();
    private LinkedList account.withdrawl = new LinkedList<>();

    public void account+.showDep()
    {
        for(int i=0; i<depositl.size(); i++)
        {
            System.out.println(depositl.get(i));
        }
    }
    public void account+.showWith()
    {
        for(int i=0; i<withdrawl.size(); i++)
        {
            System.out.println(withdrawl.get(i));
        }
    }

    after(account A, double dep) : execution(void account+.deposit(..)) && args(dep) && target(A)
    {
        A.depositl.add(Double.toString(dep));
        //System.out.println(A.depositl);
        String r = JOptionPane.showInputDialog(null,
           "Show Deposit Record? Press y for Yes, n for No.");
           if(r.equals("y")||r.equals("Y")) A.showDep();
    }

    after(account A, double withdr): execution(void account+.withdraw(..)) && args(withdr) && target(A)
    {
        A.withdrawl.add(Double.toString(withdr));
        String r = JOptionPane.showInputDialog(null,
           "Show Withdraw Record? Press y for Yes, n for No.");
           if(r.equals("y")||r.equals("Y")) A.showWith();
    }

}