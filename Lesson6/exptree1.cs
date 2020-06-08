/*
   This program implements an expression tree such as:

          +
        /   \
       /     \
      *       +
     / \     / \
    2   3   1   4

    This "parse tree" represents the expression (2*3)+(1+4), and is how program
expressions are represented inside a compiler.  Our purpose here is to write
a method "eval" which evaluates the expression and returns the result (11 for
this example).  We also want to define a procedure that traverses the tree
with a preorder traversal and print the expressions as we visit them.  

Note that we have to distinguish between tree nodes that contain values, and
those that contain an operator such as * or +, and for these we need to 
distinguish which operator it is in order to write the eval function.

Our first approach, which I call the "csc16 approach", does not use advanced
oop features despite fancy keywords like "interface" and "class".

The file exptree3.cs will contain the "csc17 approach", which uses oop
much more aggressively, and implements "algebraic types".  The file
"exptree4.cs" will implement the "csc123" approach using the visitor pattern.
*/

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

public class exptree1
{
  public static void Main()
  {
     expr A = new node("+",new node("*",new node(2),new node(3)),
	                   new node("+",new node(1),new node(4)));
	
     A.print();
     Console.WriteLine("\nvalue is " + A.eval());
  } // main
}

// How would I add a minus node, a unary minus (-3+4)???
// Can I add them in a modular way that does not touch the existing code?
// Can I add them in a type safe way so that missing cases are recognized
// by the compiler as errors?

