/* An example showing the need to seperate interface from superclass 

Some Java/C# programmers don't see the need for an interface.  They prefer
defining "abstract classes".  That is, classes that contain only stub function
definitions (they're called "pure virtual functions" in C++):

abstract class bankaccount
{
  double balance;
  public void withdraw(double x) { balance -= x; }
  public abstract void deposit(doube x);  
}

The meaning is that "deposit" is a function whose definition is to be 
supplied by subclasses.  Any class that extends (inherits) bankaccount
would need to supply a definition of deposit.  However, an abstract
class can also contain regular code - such as the balance variable and
the withdraw function.  Such an abstract class serves the purpose of both
an "interface" and a superclass with code that can be inherited.  In many
circumstances it would appear that such a combination is OK.  But in general,
we have to distinguish between "extending" a class and "implementing" an
interface.  An interface is basically a type.  Any class that implements
the interface is assumed to provide definitions for the methods in the
interface.  Extending a class is more of a mechanism for importing code.
In implementing a function to statisfy an interface requirement, we may
wish to barrow it from a superclass instead of rewriting it ourselfs.

The first reason why you should seperate superclass from interface is that
Java/C# only allow for "single inheritance" - you can only extend one
class, but you can implement multiple interfaces.  Once you commit to
extending a class you cannot extend another. 

But there's also a more subtle and even more important reason: types 
constrain the use of objects.  You may want to selectively inherit some
methods from a class and ignore others that are not supposed to be in
the interface.

The following simple example shows why you cannot always combine the use
of interface and superclass: 
*/
using System;
interface basketballplayer
{
  void dribble();
  void score();
}

interface footballplayer
{
  void tackle();
  void score();
}

class athlete 
{
  public void dribble() { Console.WriteLine("boing boing boing"); }
  public void tackle() { Console.WriteLine("eat dirt!"); }
}

class pointguard : athlete, basketballplayer
{
   public void score() { Console.WriteLine("layup"); }
}

class linebacker : athlete, footballplayer
{
   public void score() { Console.WriteLine("safety"); }
}

/*
   Both "concrete" classes pointguard and linebacker can benefit
   from code already found in the superclass "athlete".  The superclass
   in this case can be thought of as a repository of code that anyone
   can inherit.
   
   But suppose we don't want a line backer to dribble or a point guard 
   to tackle.  This is how that's enforced:

   basketballplayer b1 = new pointguard();
   footballplayer f1 = new linebacker();

   We can call b1.dribble() and f1.tackle() but we cannot call 
   b1.tackle() or f1.dribble(). "tackle" is not in the type specification
   of basketballplayer.  The interface is the correct TYPE of the objects, 
   not the superclass.

   We cannot use abstract classes for basketballplayer and footballplayer
   because of the single-inheritance constraint. Suppose, then, that we 
   tried to use only one abstract class to serve the purposes of both 
   interface and code repository:
*/

abstract class absathlete
{
  public void jump() { Console.WriteLine("toing"); }
  public void run()  { Console.WriteLine("whoosh!"); }
  public void dribble() { Console.WriteLine("boing boing boing"); }
  public void tackle() { Console.WriteLine("eat dirt!"); }
  public abstract void score();
}

class quarterback : absathlete
{
   public override void score() { Console.WriteLine("touchdown!"); }  
}

/*
  Now we would have to instantiate objects as follows:
  
  absathlete f2 = new quarterback();

  and f2.dribble() would become acceptable. 
*/

