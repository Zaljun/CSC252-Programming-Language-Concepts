// This program illustrates the rules of type-casting in C#
using System;

class AA
{  
   double y = 3.14;
   public virtual void f()
   { Console.WriteLine("AA.f, "+y); }   
}

class BB : AA
{
   int x = 1;
   
   public override void f()
   { Console.WriteLine("BB.f, "+x); }
}

class CC : AA
{
   string z = "abc";
   // inherits AA.f
   public void g()
   { Console.WriteLine("CC.g, "+z); }

   // special C# feature:
   public static explicit operator BB(CC n)  // define type cast from CC to BB
   { return new BB(); }
}


public class castings
{
   public static void Main()
   {
      AA p,q,r;
      p = new AA();
      q = new BB();
      r = new CC();

      q.f(); // which method will be called?
//      r.g(); // why not?   -- compiler error
      ((CC)r).g();  // casting down - what does this do
                    // 1. tell compiler to treat r as a CC object
                    // 2. a runtime, perform type check
      ((CC)q).g();  // compiler doesn't know what's wrong: runtime exception.
      CC n = new CC();
//    ((BB)n).f();  // compiler error without explicit operator def.

// But C# (and Java) are not as type safe as their propaganda suggest:

      double x = (double)(object)"abcd"; // this sucker will compile!
      // but at least this is not something that can be done accidentally.
 
      // .Net 4.0 or higher:
      dynamic a = "abc"; 
      dynamic b = 2.3;
      dynamic c = b / a;  // divide a double by a string. what the #@$%!

      // But at least there will be runtime exceptions.
   }//Main
}

//bottom line: C# (and Java) still cannot guarantee type safety at compile time

