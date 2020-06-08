using System;

class cell
{
   public int val;
   public cell next;
   public cell(int h, cell t) {val=h; next=t;}

   // function to return last value, or at least tries to
   public int lastval()
   {  
      cell i = this;
      while (i.next!=null) i=i.next;
      return (int)i;  // return what?!
   }

}//cell

public class mistake
{
  public static void Main()
  { 
    cell n = new cell(2,new cell(3,new cell(5,new cell(7,null))));
    Console.WriteLine( n.lastval() );
  }//main
}
