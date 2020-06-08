
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
  object visit(modnode e);
}

class intnode : expr  // integer expreesion
{
   internal int val;
   internal int siz= 1;
   public intnode(int v) {val = v; }
   public object accept(exprvisitor v) { return v.visit(this); }
} // intnode

class plusnode : expr // + operator expression
{  internal expr left, right;
   internal int siz = 1;
   public plusnode(expr l, expr r) { left=l; right=r; }
   public object accept(exprvisitor v) { return v.visit(this); }
} // plusnode

class timesnode : expr // * operator expression
{  internal expr left, right;
   internal int siz = 1;
   public timesnode(expr l, expr r) { left=l; right=r; }
   public object accept(exprvisitor v) { return v.visit(this); }
} // timesnode

class uminusnode : expr
{  internal expr left;
   internal int siz =1 ;
   public uminusnode(expr l) {left=l;}
   public object accept(exprvisitor v) { return v.visit(this); }   
}

class modnode : expr
{
    internal expr left, right;
    public modnode(expr l, expr r) {left = l; right = r;}
    public object accept(exprvisitor v) {return v.visit(this);}
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
   public object visit(modnode e)
   {
       return (int)e.left.accept(this) % (int)e.right.accept(this);
   }
} // evalvisitor

class sizer: exprvisitor
{
    public static int size = 0;
    public object visit(intnode e)
    {return size += e.siz;}
    public object visit(plusnode e)
    {return size += e.siz;}
    public object visit(timesnode e)
    {return size += e.siz;}
    public object visit(uminusnode e)
    {return size += e.siz;}
    public object visit(modnode e)
    {return size++;}
}
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
   public object visit(modnode e)
   {
       print("%",e.left,e.right); return null;
   }
    
   private void print(String op, expr l, expr r) //utility method
   {
       Console.Write("("+op+" ");
       l.accept(this); Console.Write(" "); 
       r.accept(this); Console.Write(")");
   }
} // printvisitor

// easy to add a new visitor

///////////////////////////////
public class exptree4
{
  public static void Main()
  {
    expr A = new plusnode(new timesnode(new intnode(2),new intnode(3)),
	                  new plusnode(new intnode(1),new intnode(4)));
    printvisitor printer = new printvisitor();
    evalvisitor evaluator = new evalvisitor();
    sizer size = new sizer();
    A.accept(printer);
    int v = (int)A.accept(evaluator); // cast from object to int needed
    Console.WriteLine("\nvalue is "+ v );
    int sz = (int)A.accept(size);
    Console.WriteLine("size is : "+ sz);
    expr B = new modnode(new intnode(10), new intnode(2));
    B.accept(printer);
    int u = (int)B.accept(evaluator); // cast from object to int needed
    Console.WriteLine("\nvalue is "+ u );
  } // main
}