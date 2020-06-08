/*
  Expression tree, final version: The Parametrized Visitor Pattern

  In this further refinement of the visitor patterns program, we
  use parametrized types to implement visitors, so that they don't 
  always have to return "object".  The advantages of this approach are the
  following:

	major: static (compile-time) type-checking
	minor: no need to type cast from "object" (compiler knows type).

  Note that the expr interface is not itself parametrized.  Only the
  accept function is parametrized.  That is, the accept function cannot
  make any assumptions about what T is.
*/

using System;


interface expr    // interface for abstract expression
{
  T accept<T>(exprvisitor<T> v); // polymorphic function
}

interface exprvisitor<out T>   // interface for abstract visitor
{                              // out means T is covariant
  T visit(intnode e);
  T visit(plusnode e);
  T visit(timesnode e);
  T visit(uminusnode e);
}

class intnode : expr  // integer expreesion
{
   internal int val;
   public intnode(int v) {val = v; }
   public T accept<T>(exprvisitor<T> v) { return v.visit(this); }
} // intnode


class plusnode : expr // + operator expression
{  internal expr left, right;
   public plusnode(expr l, expr r) { left=l; right=r; }
   public T accept<T>(exprvisitor<T> v) { return v.visit(this); }
} // plusnode

class timesnode : expr // * operator expression
{  internal expr left, right;
   public timesnode(expr l, expr r) { left=l; right=r; }
   public T accept<T>(exprvisitor<T> v) { return v.visit(this); }
} // timesnode

class uminusnode : expr
{  internal expr left;
   public uminusnode(expr l) {left=l;}
   public T accept<T>(exprvisitor<T> v) { return v.visit(this); }   
}

/////////////////////////////////
//////////// visitor classes: each visitor implements an operation on all
// subclasses (cases) of the abstract data type.

class evalvisitor : exprvisitor<int>
{
   public int visit(intnode e) 
   { return e.val; }
   public int visit(plusnode e) 
   {
       return e.left.accept(this) + e.right.accept(this);
   }
   public int visit(timesnode e)
   {
       return e.left.accept(this) * e.right.accept(this);
   }
   public int visit(uminusnode e)
   {
       return e.left.accept(this) * -1;
   }
} // evalvisitor

class printvisitor : exprvisitor<object>
{
   public object visit(intnode e) 
   { Console.Write( e.val ); return null; }

   public object visit(plusnode e) 
   { print("+",e.left,e.right);  return null; }

   public object visit(timesnode e)
   { print ("*",e.left,e.right); return null; }

   public object visit(uminusnode e)
   { Console.Write("(- "); e.left.accept(this); Console.Write(")"); 
     return null;
   }
    
   private void print(String op, expr l, expr r) //utility method
   {
       Console.Write("("+op+" ");
       l.accept(this); Console.Write(" "); 
       r.accept(this); Console.Write(")");
   }
} // printvisitor

//////////////////////////////////////
////////////              extension
interface exprvisitor2<out T> : exprvisitor<T>
{
    T visit(modnode e);
}
class modnode : expr
{
    internal expr left, right;
    public modnode(expr l, expr r){left = l; right =r;}
    public T accept<T>(exprvisitor<T> v) {return ((exprvisitor2<T>)v).visit(this);}
    // type casting between superclass and subclass
}
class evalvisitor2 : evalvisitor, exprvisitor2<int>
{
    public int visit(modnode e)
    {
        return e.left.accept(this) % e.right.accept(this);
    }
}
class printvisitor2 : printvisitor, exprvisitor2<object>
{
    public object visit(modnode e)
    {
        print("%",e.left,e.right); return null;
    }
    private void print(String op, expr l, expr r) 
   {
       Console.Write("("+op+" ");
       l.accept(this); Console.Write(" "); 
       r.accept(this); Console.Write(")");
   }
}
class sizer : exprvisitor2<int>
{
    public int visit(intnode e)
    {   return 1;}
    public int visit(plusnode e)
    {
        return 1 + e.left.accept(this) + e.right.accept(this);
    }
    public int visit(timesnode e)
    {
        return 1 + e.left.accept(this) + e.right.accept(this);
    }
    public int visit(modnode e)
    {
        return 1 + e.left.accept(this) + e.right.accept(this);
    }
    public int visit(uminusnode e)
    {
        return 1 + e.left.accept(this);
    }
} // sizer

///////////////////////////////
public class exptree5
{
  public static void Main()
  {
    expr A = new plusnode(new timesnode(new intnode(2),new intnode(3)),
	                  new plusnode(new intnode(1),new intnode(4)));
    printvisitor printer = new printvisitor();
    evalvisitor evaluator = new evalvisitor();
    sizer size = new sizer();
    A.accept(printer);
    int v = A.accept(evaluator);
    Console.WriteLine("\nvalue is "+ v );
    int sz = A.accept(size);
    Console.WriteLine("size is "+sz );

/////  new testing 
    printvisitor printer2 = new printvisitor2();
    evalvisitor evaluator2 = new evalvisitor2();
    sizer size2 = new sizer();
    expr B = new modnode(new intnode(3),new intnode(2));
    B.accept(printer2);
    int u = B.accept(evaluator2);
    Console.WriteLine("\nvalue is "+ u );
    Console.WriteLine("size is "+ B.accept(size2));
  } // main
}
