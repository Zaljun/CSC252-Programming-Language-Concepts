/*    Programming Assignment: Emulating Multiple Inheritance, 2020 Version

    ********* NOTE CAREFULLY: *********

    This assignment differs from the one described at the end of the video
    lecture in the following ways.

    1. BOTH PARTS OF THE ASSIGNMENT ARE REQUIRED (in the video the second
    part was optional).

    2. As an additional requirement, you should only emulate one of the
    two inheritances.  You has a language that have built-in support for
    a single inheritance, so you only need to emulate the other one.  Solutions
    that emulate both inheritances will be considered incorrect.
    In other words, you must take advantage of whatever existing support for
    inheritance that the language offers.
*/

using System;

// C# (and Java, Kotlin) does not allow for inheritance of multiple
// classes (abstract or not), only interfaces.  In class A : B,C,D
// only B can be a class, the others must be interfaces.

//// But we want to have multiple inheritance anyway.  Are we going to let
// a silly little thing like a language restriction stop us?  Think back
// to how we managed to achieve inheritance and even dynamic dispatch in Perl
// using just closures.

// Given:

public interface Ia
{
   void f();
   void g();
   void h1();
}

public interface Ib
{
   void f();
   void g();
   void h2();
}


public class A : Ia
{
  public virtual void f() { Console.WriteLine("f from class A"); }
  public virtual void g() { Console.WriteLine("g from class A"); }
  public virtual void h1() { Console.WriteLine("h1 from class A"); }
}

public class B : Ib
{
  public virtual void f() { Console.WriteLine("f from class B"); }
  public virtual void g() { Console.WriteLine("g from class B"); }
  public virtual void h2() { Console.WriteLine("h2 from class B"); }
}

/// Show how to construct a class

//public class C : Ia, Ib

// That implements both intferaces.  (class C can implement other
// interfaces, and extend a class as well).

//**********************
/// FURTHERMORE, C MUST inherit f() from class A and g() from class B.
//**********************

/// Write all your code in a separate file.
// Your class C must work without any changes to this program (no edits).
// You should compile this program into a .dll: csc /t:library
// (mcs -t library) and use it in your program.  To load a .dll when compiling,
// do csc yourprogram.cs /r:multinhasn.dll (mcs yourprog.cs -r blah.dll).

// Also write a Main to demonstrate your solution:
// public void Main()
// { C n = new C();
//   n.f(); n.g(); n.h1(); n.h2();
// }

///// DO NOT CHANGE THE CODE HERE
///** Anyone who just copy and paste the code from A and B into C or
///// who tries to decompile the .dll will loose 100000 points.

// Now, this doesn't mean you can't define f, g and h1/h2 in your subclasses
// It means that you cannot copy the "...", because it represents arbitrarily
// complex code.  Your solution must work in general, regardless of what "..."
// is.  



/********************  PART II (ALSO REQUIRED) *******************/

// Part 2 is a slightly harder version of the above problem.  This time there
// are no interfaces Ia, Ib to give you a clue.  Instead you just have

public class A2
{
  protected virtual void f() {Console.WriteLine("A2.f");}
  protected virtual void g() {Console.WriteLine("A2.g");}
  protected virtual void h1() {Console.WriteLine("A2.h1");}
}
public class B2
{
  protected virtual void f() {Console.WriteLine("B2.f");}
  protected virtual void g() {Console.WriteLine("B2.g");}
  protected virtual void h2() {Console.WriteLine("B2.h2");}
}

// Write in a separate file a class C2 that inherits all the functionality
// of A2 and B2, with the same stipulation that it should take f() from A2
// and g() from B2.

// Hint: you may define other interfaces, classes to help you, but YOU MAY 
// NOT CHANGE A2, B2 (do not change the .dll)

// You may not make ANY changes to this file, but you must use all the
// code in it.  You can compile your program with this one, or create a
// .dll from this program with csc /t:library or (on mono) mcs -t library

