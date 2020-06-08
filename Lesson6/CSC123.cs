// *********************  original code *********************
using System;

interface expr    // interface for abstract expression
{
  object accept(exprvisitor v); 
}

interface exprvisitor   // interface for abstract visitor
{
  object visit(intnode e);
  object visit(plusnode e);
  object visit(timesnode e);
  object visit(uminusnode e);
}

class intnode : expr  // integer expreesion
{
   internal int val;
   public intnode(int v) {val = v; }
   public object accept(exprvisitor v) { return v.visit(this); }
} // intnode

class plusnode : expr // + operator expression
{  internal expr left, right;
   public plusnode(expr l, expr r) { left=l; right=r; }
   public object accept(exprvisitor v) { return v.visit(this); }
} // plusnode

class timesnode : expr // * operator expression
{  internal expr left, right;
   public timesnode(expr l, expr r) { left=l; right=r; }
   public object accept(exprvisitor v) { return v.visit(this); }
} // timesnode

class uminusnode : expr
{  internal expr left;
   public uminusnode(expr l) {left=l;}
   public object accept(exprvisitor v) { return v.visit(this); }   
}

class modnode : expr
{
    internal expr left, right;
    public modnode(expr l, expr r) {left = l; right = r;}
    public object accept(exprvisitor v)  {return ((exprvisitor2)v).visit(this);}
} 

/////////////////////////////////
//////////// visitor classes: each visitor implements an operation on all
// subclasses (cases) of the abstract data type.

class evalvisitor : exprvisitor
{
   public object visit(intnode e) 
   { return e.val; }
   public object visit(plusnode e) 
   {
       return (int)e.left.accept(this) + (int)e.right.accept(this);
   }
   public object visit(timesnode e)
   {
       return (int)e.left.accept(this) * (int)e.right.accept(this);
   }
   public object visit(uminusnode e)
   {
       return (int)e.left.accept(this) * -1;
   }
} // evalvisitor

class printvisitor : exprvisitor
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


// easy to add a new visitor
// ***************************************************************

// -------------------  codes that I modified  -------------------

interface exprvisitor2: exprvisitor  // inherate exprvisitor add modnode
{
    object visit(modnode e);
}

class evalvisitor2 : evalvisitor, exprvisitor2  // modnode eval
{
     public object visit(modnode e)
   {
       return (int)e.left.accept(this) % (int)e.right.accept(this);
   }
}// evalvisitor2

class sizer: exprvisitor2  // add size()
{
    public object visit(intnode e) // base: size of int = 1
    {return  1;}
    public object visit(plusnode e)
    {return  1 + (int)e.left.accept(this) + (int)e.right.accept(this);}
    public object visit(timesnode e)
    {return  1 + (int)e.left.accept(this) + (int)e.right.accept(this);}
    public object visit(uminusnode e)
    {return  1 + (int)e.left.accept(this);}
    public object visit(modnode e)
    {return  1 + (int)e.left.accept(this) + (int)e.right.accept(this);}
}// sizer

class printvisitor2: printvisitor, exprvisitor2 //modnode print
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
}// printvisitor2

public class exptree4
{
  public static void Main()
  {
    // original testing
    expr A = new plusnode(new timesnode(new intnode(2),new intnode(3)),
	                  new plusnode(new intnode(1),new intnode(4)));
    printvisitor printer = new printvisitor();
    evalvisitor evaluator = new evalvisitor();
    A.accept(printer);
    int v = (int)A.accept(evaluator); // cast from object to int needed
    Console.WriteLine("\nvalue is "+ v );
    // test of size based on expr A
    sizer size = new sizer();
    int sz = (int)A.accept(size);
    Console.WriteLine("size is : "+ sz);

    // new tesing of remain and size
    expr B = new modnode(new intnode(9), new intnode(2));
    printvisitor printer2 = new printvisitor2();
    B.accept(printer2);
    evalvisitor evaluator2 = new evalvisitor2();
    int u = (int)B.accept(evaluator2); 
    Console.WriteLine("\nvalue B is "+ u );
    sz = (int)B.accept(size);
    Console.WriteLine("size B is : "+ sz);
  } // main
}
/*Output Sample:
(+ (* 2 3) (+ 1 4))
value is 11
size is : 7
(% 9 2)
value B is 1
size B is : 3
 */

 /* What I learned
 In CSC16.cs, it didn't use dynamic dispatch. Therefore you have to change 
 a lot in code to inheritate and override to change or add new function.
 In CSC17.cs, it used dynamic dispatch so it could be easily modified modularly 
 when you tried to modify or add new "funtions" to it.
 However, if its interface changes then you have to modify all the related codes.
 In CSC123, it used double dynamic dispatch. Therefor when you tried to change
 one feture such as mod function or size(), the "behavior" between objects changed
 automaticly. It's efficient to add a new visitor.
  */