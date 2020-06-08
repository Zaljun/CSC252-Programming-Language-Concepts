import javax.swing.*;
import java.awt.event.*;

aspect testm
{
    //account t1 = new account(500,"T1");
    before() : execution(public * main(..))
    {
        account t2 = new account(100,"T2");
        t2.deposit(250);
        t2.withdraw(300);
        t2.deposit(250);
        t2.deposit(250);
        //t2.deposit(-10);
        t2.printbalance();

    }

    //// (Not done yet) I/O interface
    /*
    after() : execution(public * main (..)) && !within(main)
    {
        JFrame f=new JFrame("Account");    
        JButton create = new JButton("Create Account");            
        create.setBounds(20,150, 200,50);  
        JButton withdraw = new JButton("Withdraw");            
        withdraw.setBounds(80,150, 200,50);   
        JButton deposit = new JButton("Deposit");            
        deposit.setBounds(120,150, 200,50);    
        final JTextField name = new JTextField();  
        name.setBounds(200,400, 300,20);
        final JTextField balance = new JTextField();  
        balance.setBounds(400,400, 300,20);
        f.add(create); f.add(withdraw); f.add(deposit);f.add(name);f.add(balance);  
        f.setSize(800,800);    
        f.setLayout(null);    
        f.setVisible(true);     
                
        create.addActionListener(new ActionListener() {  
            public void actionPerformed(ActionEvent e) {       
                String n = name.getText();  
                double b = Double.parseDouble(balance.getText());   
                account a = new account(b,n);         
                    }  
        });
        /*
        withdraw.addActionListener(new ActionListener() {  
            public void actionPerformed(ActionEvent e) {       
                double b = Double.parseDouble(balance.getText());   
                account a = new account(b,n);         
                    }  
        });

        deposit.addActionListener(new ActionListener() {  
            public void actionPerformed(ActionEvent e) {       
                String n = name.getText();  
                double b = Double.parseDouble(balance.getText());   
                account a = new account(b,n);         
                    }  
        });
        */
        
    //}
    
}