using System;

class A
{
   public int x;
   public int y;
   public A(int a, int b) {x=a; y=b;}
   public void info() { Console.WriteLine("x is "+x+" and y is "+y); }

   // name of overload function is always 'operator' followed by the
   // operator symbol:
   public static A operator+(A n, A m)  // overloads +
   {
      A sum = new A(n.x+m.x, 0);
      return sum;
   }
}//class A

class B : A
{
    public B(int a, int b) {this.x = a; this.y=b;}
    public static B operator+(B n, A m)
    {
        B sum = new B(0,n.y +m.y);
        return sum;
    }
}

public class opover
{
  public static void Main()
  {
      B n = new B(1,10);
      A m = new B(3,30);
      B sum = n + m;     
      sum.info(); 
  }
}



