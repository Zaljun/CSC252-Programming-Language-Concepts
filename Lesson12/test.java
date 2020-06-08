import java.util.LinkedList;

class asd{
    public LinkedList asdl = new LinkedList<>();
}
public class test{
    public static void main(String[] args)
    {
        LinkedList a = new LinkedList<>();
        a.add(2.0);
        a.add(0.9);
        asd obj = new asd();
        obj.asdl.add(1.2);
        obj.asdl.add(2.4);
        obj.asdl.add(8.0);
        System.out.println("Linked list : " + obj.asdl);
    }
}