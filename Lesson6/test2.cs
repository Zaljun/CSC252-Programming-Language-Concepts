/*
  Expression tree, "csc17" version.  We use interfaces and inheritance to
  implement three classes of expressions: for integers, for +, and for *
  expressions.  Each expression subclass will implement just what it needs
  to.  These "algebraic types" illustrate the potential for oop languages
  to achieve a new approach to program organization altogether.

  We also add one final refinement: use inheritance to allow code sharing 
  between + and * operator subclasses.  Class "operatornode" is the 
  superclass for both timesnode and plusnode.  Note that the superclass 
  doesn't implement the expr interface, since without knowing the type of 
  the operator, it won't know how to eval an expression.
*/

using System;

public interface expr    // abstract interface for all expression trees
{
   int eval();
   void print();
   int size();
}

class intnode : expr  // integer expreesion, or leaf nodes
{
   int val;
   public intnode(int v) {val = v; }
   public int eval() { return val; }
   public int size() {return 1;}
   public void print() { Console.Write(val); }
} // intnode


class operatornode  // interior node superclass for code sharing
{
   protected expr left, right; // type of subtrees is "expr", not "node"!
   public operatornode(expr l, expr r) { left=l; right=r; }
   protected void print(String op)  // prints in prefix notation
   {
	Console.Write("("+op+" ");
	left.print(); Console.Write(" ");
	right.print(); 
        Console.Write(")");
   }
} //operatornode superclass

class plusnode : operatornode, expr     // + operator expression
{
   public plusnode(expr l, expr r) : base(l,r) {}
   public int eval()   {  return left.eval() + right.eval(); }
   public int size() {return 1+left.size()+right.size();}
   public void print() { base.print("+"); }
} // plusnode

class timesnode : operatornode, expr    // * operator expression
{
   public timesnode(expr l, expr r) : base(l,r)  {}
   public int eval()   {  return left.eval() * right.eval(); }
   public int size() {return 1+left.size()+right.size();}
   public void print() { print("*"); }  //"base" is not needed (overloading)
} // timesnode


public class exptree3
{
  public static void Main()
  {
    expr A = new plusnode(new timesnode(new intnode(2),new intnode(3)),
	                  new plusnode(new intnode(1),new intnode(4)));
    A.print();
    Console.WriteLine("\nvalue is "+ A.eval());
    expr B = new modnode(new intnode(9), new intnode (2));
    B.print();
    Console.WriteLine("B value is "+ B.eval());
    Console.WriteLine("\nsize is " + B.size());
  } // main
}


//// And now for a whole new class of expressions, without changing anything 
//   above. This is the "unary minus" operation, such as in -4:

class uminusnode :  expr    // unary - expression
{
   expr left; // unary subextression  (no right)
   public uminusnode(expr s) { left = s; }
   public int eval()   {  return left.eval() * -1; }
   public int size() {return 1 + left.size();}
   public void print() 
   { Console.Write("-"); left.print(); }
} // uminusnode

class modnode : operatornode, expr
{
    public modnode(expr l, expr r): base(l,r) {left = l; right = r;}
    public int eval()
    {
        return left.eval() % right.eval();
    }
    public void print()
    {print("%");}
    public int size() {return 1+left.size()+right.size();}
}

// This object oriented organization allows us to modularly extend the
// design with new classes that implement the interface without having to
// edit the original code.  It's completely modular.  However, what if we
// want to have more than just the eval and print functions for these trees?