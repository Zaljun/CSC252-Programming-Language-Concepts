using System;

public class C : Ia, Ib
{
    A obj1 = new A();
    B obj2 = new B();
    public void f() 
    {
        obj1.f();
    }
    public void g() 
    {
        obj2.g();
    }
    public void h1()
    {
        obj1.h1();
    }
    public void h2()
    {
        obj2.h2();
    }
    
}


public class brg: B2
{

    public void x()
    {
        this.g();
    }
    public void y()
    {
        this.h2();
    }
}
public class C2: A2
{
    brg ob2 = new brg();
    protected override void g()
    {
        ob2.x();
    }
    protected void h2()
    {
        ob2.y();
    }
    static public void Main()
    {
        Console.WriteLine("=== C ===");
        C n = new C();
        n.f();
        n.g();
        n.h1();
        n.h2();
        Console.WriteLine("=== C2 ===");
        C2 c2 = new C2();
        c2.f();
        c2.g();
        c2.h1();
        c2.h2();
        
    }
}

/*Output Sample:
=== C ===
f from class A
g from class B
h1 from class A
h2 from class B
=== C2 ===
A2.f
B2.g
A2.h1
B2.h2
*/