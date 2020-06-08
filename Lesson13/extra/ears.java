import java.awt.*;
import java.awt.event.*;
import java.awt.Graphics;
import javax.swing.*;

privileged aspect ears
{
    public Point figure.lear;
    public Point figure.rear;
    public Color figure.earcolor = Color.black;

    public void figure.earcolor(Color c) 
	{ earcolor = c; if (autodraw) draw(); }
 
    // calculation aspect
    before(figure f) : call(private void figure.orient()) && target(f) &&
	               withincode(private void figure.calcPoints(..))
    {
	int x = f.chest.x;  int y = f.chest.y;
	int scale = f.scale;
	f.lear = new Point(x-scale/2,y-((3*scale)/2));
	f.rear = new Point(x+scale/2,y-((3*scale)/2));
    } // before advice

    after(figure f) : execution(private void figure.orient()) && target(f)
    {
        f.lear = f.rotate(f.lear);
	f.rear = f.rotate(f.rear);
    } // after
    
    // drawing advice will use another helper function:

    public void drawears(figure f)
	{
	    Color oldcolor = f.color;
	    f.brush.setColor(f.earcolor);
	    int scale = f.scale;
	    f.brush.fillOval(f.lear.x-(scale/4),f.lear.y-(scale/4),
			     scale/2,scale/2);
	    f.brush.fillOval(f.rear.x-(scale/4),f.rear.y-(scale/4),
			     scale/2,scale/2);
	    f.brush.setColor(oldcolor);
	} // drawears

    before(figure f) : call(public void figs.nextframe(..)) && this(f)
	&& withincode(public void figure.draw()) && (if (figure.autodraw))
	{
	    drawears(f);
        }

    private int r=0, g=0, b=0;
    
    after(figure f) : execution(public void figure.draw()) && target(f)
	&& (if (!figure.autodraw))
	{
	    r = (r+10) %256; g=(g+20)%256; b = (b+30)%256;
	    f.earcolor(new Color(r,g,b));
	    drawears(f);
	}
} // ears