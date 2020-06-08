// *********************  original code *********************
using System;
interface expr    // abstract interface for expression trees
{
   int eval();
   void print();
}

class node : expr
{
   String op;       // the operator, "+" or "-", null if node is leaf with int
   int val;         // the integer value in case it's a leaf node
   node left, right;  // the left and right subtrees
   // no unions available in C#, so every node contains all of these fields

   //constructors: one for operators and one for atomic integers

   public node(int v)   // for integer, leaf nodes
   {  op = null;   val = v;  left = null;  right = null;}

   public node (String p, node l, node r)  // for operator nodes
   {  if (p==null) throw new Exception("null operator");
      op = p; left = l;  right = r;
   }
   
   ////// implementation of interface methods

   public int eval()
   { 
     switch (op)
     {  
        case null : return val;   // leaf node
	case "+" : return left.eval() + right.eval(); 
	case "*" : return left.eval() * right.eval(); 
	default : throw new Exception("invalid operator");
     }
   } // eval

   public void print() // prefix (preorder) notation
   {
      if (op==null) Console.Write(val); // leaf node
      else // operator node
      {
	Console.Write("(" + op + " ");
	left.print(); Console.Write(" ");
	right.print(); 
        Console.Write(")");
      }
   } // print
} // class node
// ***************************************************************

// -------------------  codes that I modified  -------------------
interface expr2: expr 
{
    int size();
}
class node2: node, expr2     // inheritate old node and implement int size() 
                             // chanege eval() to add remain operation
{
    string op;
    int val;
    node2 left, right;
    public node2(int v): base(v)   // for integer, leaf nodes
    {  op = null;   val = v;  left = null;  right = null;}

    public node2 (String p, node2 l, node2 r): base(p,l,r) // for operator nodes
    {  if (p==null) throw new Exception("null operator");
      op = p; left = l;  right = r;
    }
    public new int eval()            // override old eval function
                                     // almost redo
   { 
       switch (op)
       {  
           case null : return val;   // leaf node
	       case "+" : return left.eval() + right.eval(); 
	       case "*" : return left.eval() * right.eval(); 
           case "%" : return left.eval() % right.eval();  
	       default : throw new Exception("invalid operator");
       }
   } // new eval
   public int size()                  // add size function                         
   {
       switch(op)
       {
           case null: return 1;
           case "+" : return 1+left.size()+right.size();
           case "*" : return 1+left.size()+right.size();
           case "%" : return 1+left.size()+right.size();
           default  : throw new Exception("invalid operator");
       }
   }// new size
}
public class exptree1
{
  public static void Main()
  {
     // original testing
     expr A = new node("+",new node("*",new node(2),new node(3)),
	                   new node("+",new node(1),new node(4)));
	
     A.print();
     Console.WriteLine("\nvalue is " + A.eval());

    // added codes below to test remain and size
     expr2 B = new node2("%",new node2("+",new node2(4),new node2(5)),new node2(2));
     B.print();
     Console.WriteLine("\nvalue of B is " + B.eval());
     Console.WriteLine("size of B is "  + B.size());
  } // main
}
/*Output Sample:
(+ (* 2 3) (+ 1 4))
value is 11
(% (+ 4 5) 2)
value of B is 1
size of B is 5
 */