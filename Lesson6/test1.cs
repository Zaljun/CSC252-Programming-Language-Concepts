using System;

interface expr    // abstract interface for expression trees
{
   int eval();
   void print();
   //int size();
}

class node : expr
{
   String op;       // the operator, "+" or "-", null if node is leaf with int
   int val;         // the integer value in case it's a leaf node
   //int siz;
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
    case "%" : return left.eval() % right.eval();  // ******************add
	default : throw new Exception("invalid operator");
     }
   } // eval

   public int size()                               // ******************add
   {
       switch(op)
       {
           case null: return 1;
           case "+" : return 1+left.size()+right.size();
           case "*" : return 1+left.size()+right.size();
           case "%" : return 1+left.size()+right.size();
           default  : throw new Exception("invalid operator");
       }
   }

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

public class exptree1
{
  public static void Main()
  {
     expr A = new node("+",new node("*",new node(2),new node(3)),
	                   new node("+",new node(1),new node(4)));
	
     A.print();
     Console.WriteLine("\nvalue is " + A.eval());
     expr B = new node("%",new node("+",new node(4),new node(5)),new node(2));
     B.print();
     Console.WriteLine("\nvalue is " + B.eval());
     Console.WriteLine("\nsize is " + ((node)B).size());
  } // main
}
