

public class br0: B2
{

    public void x0()
    {
        this.g();
    }
    public void y0()
    {
        this.h2();
    }
}
public class C20: A2
{
    br0 ob20 = new br0();
    public void g()
    {
        ob20.x0();
    }
    public void h2()
    {
        ob20.y0();
    }
    static public void Main()
    {
        C20 c2 = new C20();
        c2.f();
        c2.g();
        c2.h1();
        c2.h2();
        
    }
}