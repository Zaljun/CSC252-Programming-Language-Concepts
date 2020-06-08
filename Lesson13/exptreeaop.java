/*
    Expression Tree Program, Aspect-Oriented Version in AspectJ

    This program makes aggressive use of "inter-type" declarations in
    AspectJ to modify the oop structure of a program according to various
    aspects.
*/


//////// base oop program  (can also call this the base aspect)

interface expr  // empty interface - specifies only that type exits
{}

// The specific elements of the interface will be defined by individual
// aspects.

class opnode   // an operator node superclass
{
  public  expr left, right;   // left and right subtrees
  public  opnode(expr l, expr r)
    { left=l;  right=r; }

} // opnode

class intexp implements expr  // inter leaf node
{
    public int val;
    public intexp(int v) {val=v;}
}

class sumexp extends opnode implements expr  // + expression
{
    public sumexp(expr l, expr r)
    {  super(l,r); }
}

class multexp extends opnode implements expr  // * expression
{
    public multexp(expr l, expr r)
    {  super(l,r); }
}

public class exptreeaop
{
    public static void main(String[] args)
    {
	expr E = new sumexp(new multexp(new intexp(2),new intexp(3)),
			    new sumexp(new intexp(1),new intexp(4)));
	E.print();  // the print method is declared in aspect printing below
	            // as an "intertype declaration".
	int v = E.eval();
	System.out.println("value equals "+v);
	System.out.println("largest integer in tree is "+E.largest()+"\n");


	expr B = new sumexp(new intexp(2),null);
	expr C = new sumexp(new intexp(2),new intexp(4));
	System.out.println("B is a tree: "+B.wellformed());
	System.out.println("C is a tree: "+C.wellformed());
	((sumexp)C).right = C;
	System.out.println("C is still a tree: "+C.wellformed());	
    }  // main
}



////////// aspects  (these can also be in seperate files)

/* I am now concerned with the aspect of printing the tree */
aspect printing
{
    public abstract void expr.print(); // appends interface

    public void intexp.print() 
	{ System.out.print(val); }
    public void sumexp.print()
	{ preorder("+",left, right); }
    public void multexp.print()
	{ preorder("*",left, right); }
        
    private static void preorder(String op, expr l, expr r) // utility
	{
	    System.out.print("("+op+" ");
	    l.print(); System.out.print(" ");
	    r.print(); System.out.print(")");
	}

    // advice to print a newline after calling print() from outside
    after() : call(void expr+.print()) && !within(printing)
	{
	    System.out.print("\n");
	}
}  // printing aspect


/* Now I'm concerned with the evaluation of the methods */
aspect evaluation
{
    public abstract int expr.eval();
    public int intexp.eval()  { return val; }
    public int sumexp.eval()  { return left.eval()+right.eval(); }
    public int multexp.eval() { return left.eval()*right.eval(); }
} // evaluation


/*
  You should be asking at this point, so what's the big deal, why is this
better than the two oop-style approaches that we've already seen.

But remember, the visitor pattern makes it easy to add a new visitor,
but when adding a new visitee, we need techniques such as object
adapters.  One or two object adapters in a program are fine, but
suppose there are twenty.  In contrast, the program seen in
exptree3.cs ("csc17 approach") avoids this problem.  However, the
tradeoff is that in order to add a new method such as print or eval,
we will have to edit several classes.  In that program, a method such
as print "crosscut" multiple classes.  With the visitor pattern, a
new visitee class "crosscut" multiple visitors.  What a dilemma.
OOP can create its own nightmares.  In other words, 
the technology of oop appears to only SCALE so much!

Here's where the fun starts. Run and hide if you get scared, earthlings!
*/


/* First, my "concern" is to add a new kind of expression (unary minus),
   and the associated operations on it.  
*/

class uminus
{
  public expr subexp; // subexpression
  public uminus(expr s) {subexp=s;}
}

aspect newexpression
{
    // declare inheritance relationship here.  Whether this declaration
    // is globally visible depends on whether the aspect is compiled with
    // the program (it is here because it's in the same file).
    declare parents: uminus implements expr;

    public void uminus.print()
	{ System.out.print("- "); subexp.print(); }
    public int uminus.eval() { return -1 * subexp.eval(); }

    // add a new test case, without changing "main". 
    after() : execution(public static void *.main(String[]))
	{
	    System.out.println("testing our new class...");
	    expr A = new uminus(new multexp(new intexp(2),new intexp(4)));
	    A.print();
	    System.out.println("value is "+A.eval());
	}

    // small but potentially nasty problem: did you notice that an extra
    // line was printed?  AOP is promising but that doesn't mean we've found
    // the right mixture of capabilities yet.

} // newexpression aspect



/* Now I'm concerned with adding a new method, in a modular way similar to
   visitor pattern, but one that can visit uminus expressions as well, 
   without the need to use object adaptors.  The method returns the
   largest number found in the tree.
*/

aspect newmethod
{
    public abstract int expr.largest();

    public int intexp.largest()
	{ return val; }
    public int sumexp.largest()
 	{   
	    int l = left.largest();
	    int r = right.largest();
	    if (l>r) return l; else return r;
	}
    public int multexp.largest()
 	{   
	    int l = left.largest();
	    int r = right.largest();
	    if (l>r) return l; else return r;
	}
    public int uminus.largest()
	{
	    return subexp.largest();
	}
} // newmethod

/*
   The next new method checks that the tree is well formed.  That is, there
   are no looping pointers and that no pointer points to null.  To do
   that we need to add a marker to each node to indicate that it's
   been visited. Note that this marker is local to the aspect, since it
   is only relevant to the local algorithm.

   Note: the code may look like that AspectJ is adding multiple-inheritance
   to Java.  Technically it is not: wrather, the code is automatically
   appending each subclass with a new instance variable.
*/

aspect wellformedness 
    percflow(call(boolean expr+.wellformed()) && !within(wellformedness))
{
    public abstract boolean expr.wellformed();

    // says that each object of type expr will have a visited variable:
    private boolean expr.visited = false;


    /* The code that each of the above methods have in
       common can be captured inside an advice: */
    
    boolean around(expr m) : execution(boolean expr+.wellformed()) 
	                       && target(m)
	{
	    if (m.visited) return false;
	    m.visited = true;
	    return proceed(m);  // proceed with the rest:
	}

    public boolean intexp.wellformed() 
	{
	   return true;
        }

    public boolean uminus.wellformed()
	{
	    return (subexp!=null && subexp.wellformed());
	}

    public boolean opnode.wellformed() 
	{
	    return (left!=null && right!=null 
		    && left.wellformed() && right.wellformed());
	}

    // the following 2 lines are needed because of an AspectJ bug:
    public boolean sumexp.wellformed() {return super.wellformed();}
    public boolean multexp.wellformed() { return super.wellformed();}

    // the following procedure and advice resets the flags to false
    // so they can be used again.  Note that loops won't a problem
    // because the flag would set to false the first time its seen.
    private abstract void expr.reset();
    private void intexp.reset() { visited=false; }
    private void sumexp.reset()
	{ visited = false;
	  if (left!=null && left.visited) left.reset();
	  if (right!=null && right.visited) right.reset();
	}
    private void multexp.reset()
	{ visited = false;
	  if (left!=null && left.visited) left.reset();
	  if (right!=null && right.visited) right.reset();
	}
    private void uminus.reset()
	{ visited = false;
	  if (subexp!=null && subexp.visited) subexp.reset();
	}
    after(expr m) : call(boolean expr+.wellformed()) 
	            && !within(wellformedness) && target(m)
	{
	    m.reset();
	}
} // wellformedness

/*
    We can identify three principle advantages of AOP:

    1. Aspects can be developed separately (i.e "separation of
       concern").  Moreover, they can be attached to or decoupled from
       the rest of the program as the situation demands.  That is, if
       all the aspects are in different files, we can include/exclude
       them from a program by simply choosing to compile them together
       or not.  This is a decisive advantage over the traditional
       approach to modular programming.  If you have 10 different
       aspects, there are 2 to the 10th power = 1024 different subsets
       of them.  That is, 1024 different ways to combine them.  Which of
       these are you going to need?  With regular oop, you may choose
       one or maybe two of these combinations when composing your
       program, but how can you possibly anticipate situations where
       other combinations are needed?

    2. The organizational structure of a (very large) program can be
       re-oriented to suite a particular concern, without necessarily
       affecting other aspects of the program.  This is examplified by
       the ability to modify within an aspect the inheritance
       hierarchy of a program.  It is even possible to have class A
       extend B in one aspect and class B extend A in another aspect.
       This may drive OOP purists up the wall, but consider this: if
       you have points (x,y) and circles (x,y,radius). In some
       contexts you might want to treat both as points, and in others
       you might want both as circles.  Sure, if you plan out
       everything perfectly, you may find the perfect inheritance
       hierarchy that satisfy both needs (both point and circle should
       extend a common superclass).  However, how can you be sure that
       you can always foresee everything that a large program, with
       thousands of lines and hundreds of components will need.  One
       problem with oop is that it sometimes requries hindsight to
       write a well-organized program (oop purists are sending out
       assasins as we speak!).

    3. Program functionality can be extended without changing the original
       code.  We can also achieve this to a degree by writing new functions
       and extending exiting classes.  However, we would have to at least
       edit all our code to use the new function/classes instead.  We may
       also need to create a number of adapters to force the exiting code
       to integrate with the new code, something that is always tricky to do.

       Note, however, that this is not always possible.  There are
       no pointcuts, for example, to identify the entering and exitinng
       of while loops.  Some degree of cooperation between program fragments
       may still be needed.

    These points are the goals of aspect oriented programming.  Whether the
    mechanisms of the AspectJ language is the best way to achieve them is
    another question.
*/

