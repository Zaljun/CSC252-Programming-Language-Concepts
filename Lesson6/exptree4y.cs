/*
  Expression tree, "csc123" version: the Visitor Pattern.

  The program in exptree3.cs is fairly good, but notice that the
  code for an operation such as "eval" is scattered across many different
  classes.  This may not be the desired way to implement a program.  We may
  wish to use algebraic subtypes, but still "gather" the various cases of
  an operation into one class.  The visitor pattern allows this approach.
  It is the most aggressive use of oop that I know of.  Its central mechanism
  is referred to as "double dispatch".  

  The classes for the different types of expressions (intnode, timesnode, etc)
  are called "visitees".  Operations such as eval are encapsulated in classes
  called "visitors".  Each visitor class must know how to visit each kind
  of expression (intnode, timesnode, etc). That is, a "eval visitor", when
  visiting a node, will evaluate its integer value.  A "print visitor" will
  print each node that it visits.  Each of these visitors must implement 
  methods for visiting intnodes, plusnodes, timesnodes, etc ...

  One item some people might not like about this program is that elements
  that were "private" or "protected" now need to be visible to both the
  expr and the exprvisitor classes.  oop languages provides varying degrees
  of flexibility to implementing such a scenario, in addition to simply making
  everything "public".  The language Eiffel allows you to specify exactly 
  which classes something is to be visible to.  In C++, there is the "friend"
  declaration.  In Java, there's globally public versus public within a
  package.  In C#, instead of "public" we can use the modifier "internal",
  which means that it's public only to code that's compiled together, but
  not if the code is imported as a .dll by another program.  
*/

using System;

interface expr    // interface for abstract expression
{
  object accept(exprvisitor v); 
}
 interface expr2 : expr{ object accept(exprvisitor2 v); }

interface exprvisitor   // interface for abstract visitor
{
  object visit(intnode e);
  object visit(plusnode e);
  object visit(timesnode e);
  object visit(uminusnode e);
}
interface exprvisitor2 : exprvisitor { object visit(modnode e); }
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
    public modnode(expr l, expr r) { left = l; right = r; }
   // public object accept(exprvisitor2 v) { return null; }
    public object accept(exprvisitor v) { return ((exprvisitor2)v).visit(this); }
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
class evalvisitor2 : evalvisitor, exprvisitor2
{
    public object visit(modnode e)
    {
        return (int)e.left.accept(this) % (int)e.right.accept(this);
    }
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
    
   private void print(String op, expr l, expr r) //utility method
   {
       Console.Write("("+op+" ");
       l.accept(this); Console.Write(" "); 
       r.accept(this); Console.Write(")");
   }
} // printvisitor
class printvisitor2: printvisitor, exprvisitor2
{
    public object visit(modnode e)
    {
        print("%", e.left, e.right);return null;
    }
    private void print(String op, expr l, expr r){
        Console.Write("(" + op + " ");
        l.accept(this);Console.Write(" ");
        r.accept(this); Console.Write(")");
    }
}

class sizecounter : exprvisitor, exprvisitor2
{
    public object visit(intnode e) { return 1; }
    public object visit(plusnode e) { return (int)e.left.accept(this) + (int)e.right.accept(this) + 1; }
    public object visit(timesnode e) { return (int)e.left.accept(this) + (int)e.right.accept(this) + 1; }
    public object visit(uminusnode e) { return 1; }
    public object visit(modnode e) { return (int)e.left.accept(this) + (int)e.right.accept(this) + 1; }
}
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
    A.accept(printer);
    int v = (int)A.accept(evaluator); // cast from object to int needed
    Console.WriteLine("\nvalue is "+ v );
  
        expr B = new plusnode(new timesnode(new intnode(2), new intnode(4)),
        new modnode(new intnode(1), new intnode(5)));
    
        printvisitor2 printer2 = new printvisitor2();
    evalvisitor2 evalvisitor2 = new evalvisitor2();
        sizecounter sizer = new sizecounter();
        B.accept(printer2);
        int v2 = (int)B.accept(evaluator);
        Console.WriteLine("\nthe value is " + v2);
        int s2 = (int)B.accept(sizer);
        Console.WriteLine("\nthe size is " + s2);
    }

   
   

    
    // main
}

/*
   To understand how a line like 

      A.accept(printer); 

   works.  The first dispatch is made on the "visitee" object A.
   The accept method of the visitee invokes 
   
      v.visit(this);

   where v is an abstract exprvisitor.  Thus a second dispatch occurs, 
   this time on the "visitor" object v.  That is, both types of the visitee
   and visitor are made abstract and rely on dynamic dispatch.  In this
   example, the visitee is some specific expression node (plusnode), which
   needs to be processed differently.  The visitor is some specific visitor
   (printvisitor) that also behaves differently.  

   One final word: it may appear that method overloading is a central 
   mechanism here, but in fact it is not.  We can also define the visitor
   interface as:

    interface exprvisitor
    {
      object visiti(intnode e);
      object visitp(plusnode e);
      object visitt(timesnode e);
      object visitu(uminusnode e);
    }

   and the same mechanisms would still be needed.  Overloading is just
   a further convenience.  The central mechanism is that there are different
   kinds of visitors and different kinds of visitees, and the Visitor Pattern
   ties these classes together.

   The visitor pattern's disadvantage is that it's harder to add a new visitee
   class (for a new kind of expression).  It's advantage is that it's easier
   to add a new visitor class.
*/

