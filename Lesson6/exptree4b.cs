/*  In this extension of exptree4.cs, we reveal a possible crack in the
dynamic dispatch paradigm by attempting to extend the visitor pattern with a
new kind of visitee.  The result is that the compiler will loose the ability
to detect some kinds of inconsistencies, and that some if-else statements
(yikes) are once again needed to get things to work smoothly.

This file needs to be compiled with  csc exptree4.cs exptree4b.cs (together)
*/

using System;

// goal : add a new type of visitee b  (a div node for division)

interface evisitor : exprvisitor
{
   object visitd(divnode e);
}

// but do not extend expr - an evisitor is still an expression visitor
// this allows for maximal backwards compatibility.

class divnode : expr // + operator expression
{  internal expr left, right;
   public divnode(expr l, expr r) { left=l; right=r; }
   public object accept(exprvisitor v) 
     {   if (v is evisitor) return ((evisitor)v).visitd(this); 
         else throw new Exception("visitor doesn't know how to visit me");
     }
} // divnode


/// extended visitors
class evaluator : evalvisitor, evisitor
{   
   public object visitd(divnode e)
   { return (int)e.left.accept(this) / (int)e.right.accept(this); }
}


public class exptree4b
{
public static void main2()
 { 
     expr A = new plusnode(new divnode(new intnode(4),new intnode(2)),new intnode(3));
     int x = (int) A.accept(new evaluator()); // works ok

     A.accept(new printvisitor()); // throws runtime exception. // not a
     // compiler error.
 }
}
