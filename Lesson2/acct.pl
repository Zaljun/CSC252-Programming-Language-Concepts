sub maketoggle
{
    my $x = -1; # initial value of toggle
    sub
    {  $x = (-1) * $x; $x }  # returns pointer to this inlined subroutin
}
my $toggle = maketoggle();

print "1. Maketoggle\n";
print $toggle->(),"\n";
print $toggle->(),"\n";
print $toggle->(),"\n";



sub makeaccount
{  my $balance = $_[0];  # initialize by parameter
   my $password = $_[1];
   my $n = 0;            # counter of how many times the password is not right in a row

   my $deposit = sub { $balance += $_[0]; $balance; };

   # original withdraw method
   #my $withdraw = sub { if ($_[0]<=$balance) 
    #		     { $balance-=$_[0]; $balance; } 
     #                   else { print "insufficient funds\n"; }
      #                };

   # modified Withdraw 
   my $withdraw = sub { 
        if ($n >= 3)          # Check if it is disabled
        {print "account disabled\n";}
        else
        {
            if($password ne $_[1]) # Check the password
            {
                $n++;
                if ($n < 3)
                { print "wrong password\n";}
                else
                { print "call the cops!\n";}
            }   
            else
            { 
                $n = 0;
                if ($_[0]<=$balance) {$balance -=$_[0]; #print $balance,"\n";
                }
                else {print "insufficient funds\n";}
            }
        }
   };

   # Reset Password
   my $reset = sub {
       if ($n >= 3)      # Check if it is disabled
       {print "account disabled\n";}
       else              # Require correct password to update new password
       {
           if ($_[0] ne $password)
           {
               $n++;
               print "wrong password, reset agian\n";
           }
           else
           {
               print "enter new password\n";
               my $a = <STDIN>;
               $password = $a;
               print "new password is: ", $a, "\n"; 
           }
       }
   };

   my $inquiry = sub { $balance };

   my $compare = sub { my $bigger = 0;
                       if ($balance > $_[0]->{"inquiry"}->())
                       { $bigger = 1; $bigger;}
                       else { $bigger;}
   };

   my $dispatch;  # use hash table as an interface

   $dispatch->{"withdraw"} = $withdraw;
   $dispatch->{"deposit"} = $deposit;   
   $dispatch->{"inquiry"} = $inquiry;
   $dispatch->{"hasmoremoney"} = $compare; 
   $dispatch->{"passwordreset"} = $reset;
   $dispatch->{"set"} = sub {$balance = $_[0]}; 
   # in order to make a "link" to makesaving to update $balance

   $dispatch;  # makeaccount returns dispatching hash table
}

print "2. Compare\n";
my $ac1 = makeaccount(100);
my $ac2 = makeaccount(500);
$ac1->{"withdraw"}->(50);  
$ac2->{"deposit"}->(100);
#print $ac1->{"inquiry"}->();
print $ac2->{"hasmoremoney"}->($ac1),"\n";
my $acc1 = makeaccount(100);
my $acc2 = makeaccount(200);
print $acc1->{"hasmoremoney"}->($acc2),"\n"; 

print "3. Withdraw with password\n";
my $acc = makeaccount(100,"xyzzy");  # creates new account with password
$acc->{"withdraw"}->(50,"abcde");  # prints "wrong password"
$acc->{"withdraw"}->(50,"xzyzz");  # prints "wrong password"
$acc->{"withdraw"}->(50,"zzxxy");  # prints "call the cops!"
$acc->{"withdraw"}->(50,"xyzzy");  # prints "account disabled"

print "3. Password reset\n";
$acc->{"passwordreset"}->("xyzzy");
my $ac = makeaccount(100,"xyzzy");  
$ac->{"passwordreset"}->("xyzz");
$ac->{"passwordreset"}->("xyzzy");

sub makesavings
{
    my $dispatch = makeaccount($_[0]);
    my $interest = 0.05;
    my $balance = $dispatch->{"inquiry"}->();

    $dispatch->{"deposit"} = sub { 
        $balance += $_[0];  $balance *= (1+ $interest); 
        $dispatch->{"set"}->($balance);
        #print $balance,"\n";
        $balance;
        };
    $dispatch;
}
my $saving1 = makesavings(50);
print "4. Makesaving INHERITANCE\n";
$saving1->{"deposit"}->(150);
print $saving1->{"inquiry"}->(),"\n";
$saving1->{"withdraw"}->(150);
print $saving1->{"inquiry"}->(),"\n";


=Output Samples:
D:\360MoveData\Users\admin\Desktop\Lesson 2>perl acct.pl
1. Maketoggle
1
-1
1
2. Compare
1
0
3. Withdraw with password
wrong password
wrong password
call the cops!
account disabled
3. Password reset
account disabled
wrong password, reset agian
enter new password
a
new password is: a

4. Makesaving INHERITANCE
210
60
=cut





