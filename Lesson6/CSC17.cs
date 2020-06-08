// *********************  original code *********************
using System;

public interface expr    // abstract interface for all expression trees
{
   int eval();
   void print();
}

class intnode : expr  // integer expreesion, or leaf nodes
{
   int val;
   public intnode(int v) {val = v; }
   public int eval() { return val; }
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
   public void print() { base.print("+"); }
} // plusnode

class timesnode : operatornode, expr    // * operator expression
{
   public timesnode(expr l, expr r) : base(l,r)  {}
   public int eval()   {  return left.eval() * right.eval(); }
   public void print() { print("*"); }  //"base" is not needed (overloading)
} // timesnode

class uminusnode :  expr    // unary - expression
{
   expr left; // unary subextression  (no right)
   public uminusnode(expr s) { left = s; }
   public int eval()   {  return left.eval() * -1; }
   public void print() 
   { Console.Write("-"); left.print(); }
} // uminusnode
// ***************************************************************

// -------------------  codes that I modified  -------------------
public interface expr2: expr  // inherate expr add size()
{
    int size();
}
                    ////// add size for evry node
class intnode2 : intnode, expr2  
{
    public intnode2(int v): base(v) {}
    public int size() {return 1;}
} // intnode2

class plusnode2: plusnode, expr2
{
    new protected expr2 left,right;
    public plusnode2(expr2 l, expr2 r): base(l,r) {left =l; right =r;}
    public int size() {return 1+left.size()+right.size();}
}// plusnode2

class timesnode2: timesnode, expr2
{
    new protected expr2 left,right;
    public timesnode2(expr2 l, expr2 r): base(l,r) {left =l; right =r;}
    public int size() {return 1+left.size()+right.size();}
}// timesnode2

class uminusnode2 : uminusnode, expr2
{
    expr2 left;
    public uminusnode2(expr2 s): base(s){left =s;}
    public int size() {return 1+left.size();}
}// umiusnode2

class modnode2 : operatornode, expr2  // modularly create a new class
                                      // to do remaining
{
    new protected expr2 left,right;
    public modnode2(expr2 l, expr2 r): base(l,r) {left = l; right = r;}
    public int eval()
    {
        return left.eval() % right.eval();
    }
    public void print()
    {print("%");}
    public int size() {return 1+left.size()+right.size();}
}// modnode2

public class exptree3
{
  public static void Main()
  {
    // original testing
    expr A = new plusnode(new timesnode(new intnode(2),new intnode(3)),
	                  new plusnode(new intnode(1),new intnode(4)));
    A.print();
    Console.WriteLine("\nvalue is "+ A.eval());

    // new testing of remain and size
    expr2 B = new modnode2(new plusnode2(new intnode2(4),new intnode2(5)),
                    new intnode2(2));
    B.print();
    Console.WriteLine("\nvalue of B is "+ B.eval());
    Console.WriteLine("size of B is " + B.size());
  } // main
}

/*Output Sample:
(+ (* 2 3) (+ 1 4))
value is 11
(% (+ 4 5) 2)
value of B is 1
size of B is 5
 */