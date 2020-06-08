import java.awt.*;
import java.awt.event.*;
import java.awt.Graphics;
import javax.swing.*;

public class figs extends JFrame
{
  public Graphics brush;
  public Graphics display;
  private    Image canvas;    // off-screen graphics buffer
  private Color bkcolor = Color.white;

  public void paint(Graphics g) {}  // overrides auto-update

  public static void main(String[] args) // needed for application
  {
    figs session  = new figs();
    session.addWindowListener(new WindowAdapter() {
              public void windowClosing(WindowEvent e) {System.exit(0);} });
    session.init();
  }

   public void init()
    {
       setBounds(0,0,800,600);
       setVisible(true);
       setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
       canvas = createImage(800,600);
       brush = canvas.getGraphics();
       display = this.getGraphics();
       brush.setColor(bkcolor);   // clear 
       brush.fillRect(0,0,800,600);   // with black background
       brush.setColor(Color.green);   // 
       animate();
    } // init

   public static int randint(int min, int max) 
    { return (int) (Math.random() * (max-min+1) + min); }

   private void clearbuffer()
    { Color old;
      old = brush.getColor();
      brush.setColor(bkcolor);   // clear 
      brush.fillRect(0,0,800,600);   // with black background
      brush.setColor(old);   //  restores color
    } // clearbuffer

   public void nextframe(int delay) // delay in ms
    {
       try { Thread.sleep(delay); } catch (InterruptedException IE) {}
       display.drawImage(canvas,0,0,null);  // draws to screen
       clearbuffer();
    } // nextframe with ms delay


   public void animate() 
    {  int i;  // loop counter
       figure fig1, fig2, fig3;
       figure.autodraw = false;
       fig1 = new figure(400,300,this);
       fig2 = new figure(100,150,this);
       fig3 = new figure(50,50,this);
       fig1.beSad();  fig2.beHappy(); 
       fig3.beHappy();  fig3.setColor(Color.red);
       i = 0;
       while (i<40)
	   {   
	       fig1.move(4,1);
	       if (i%2==0) fig1.armsUp();
	         else fig1.armsDown();
	       fig1.shrink();  fig1.rotate(10); 
	       fig2.rotate(350); // 10 degrees clockwise
	       //   fig1.earcolor(Color.red);  // need ears aspect
	       fig3.move(10,10);
	       if (i%3==0) fig3.armsUp();
	       else if (i%3==1) fig3.armsStraight();
	       else fig3.armsDown();

	       i = i+1;

	       fig1.draw();    // not needed with autodraw
	       fig2.draw();
	       fig3.draw();
	       nextframe(100);

           } // while
    } // animate

} // class figs

