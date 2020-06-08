/* stickfig modified to draw eagerly */

import java.awt.*;
import java.awt.event.*;
import java.awt.Graphics;
import javax.swing.*;


public class figure
{   
    /* Attribute variables defining a stick figure: */
    private int scale = 48;  // default scale factor for diameter of head
    private Color color = Color.blue; // default color
    private int mouthfactor = 0;    // default neutral
    private int armfactor = 0;      // default straight arm
    private int angle = 0;          // default angle of rotation
    public  String name;            // not used.
    private Point chest, neck, stomach, lleg, rleg, larm, rarm;
    private Point nose, leye, reye, lmouth, rmouth, lip;

    public static boolean autodraw = false; // controls drawing after each
                                            // method call.
    Graphics brush;
    figs parent;
    int delms = 100; // delay in ms between animation frames
    int sizefactor = 8;  // size change upon grow/shrink

    public figure(int x, int y, figs p) // constructor
    { brush = p.brush;    name = "";  
      parent = p;
      brush.setColor(color);
      calcPoints(x,y);     // Calculate initial coordinates
      if (autodraw) draw();
    }

    /* Private methods for interior computation: */

    private void calcPoints(int x, int y)  // (re)calculate coordinates
    {                                      // based on center (chest) coords
      chest = new Point(x,y);    
      stomach = new Point(x,y+scale);
      neck = new Point(x,y-(scale/2));
      nose = new Point(x,neck.y-(scale/2));
      lleg = new Point(x-scale,y+(2*scale));
      rleg = new Point(x+scale,y+(2*scale));
      leye = new Point(x-(scale/4),neck.y-((3*scale)/4));
      reye = new Point(x+(scale/4),neck.y-((3*scale)/4));
      lmouth = new Point(x-(scale/4),neck.y-(scale/4));
      rmouth = new Point(x+(scale/4),neck.y-(scale/4));
      larm = new Point(x-scale,y+(armfactor*scale/2));
      rarm = new Point(x+scale,y+(armfactor*scale/2));
      lip = new Point(x,neck.y-(scale/4)+(mouthfactor*scale/8));
      orient();  // rotate all points according to angle 
    }

    private Point rotate(Point P)
    { Point NP = new Point(P.x-chest.x,P.y-chest.y);  // origin offset
      int oldx = NP.x;  // save copy of Np.x before changing it
      double r = (double) ((double) angle * Math.PI) / 180;  // radians
      NP.x = (int) (((double) NP.x * Math.cos(r)) +   // multiply by
		    ((double) NP.y * Math.sin(r)));   // rotation matrix
      NP.y = (int) (((double) oldx * (-1.0 * Math.sin(r))) +
		    ((double) NP.y * Math.cos(r)));
      NP.x = NP.x + chest.x;  // origin offset readjustment
      NP.y = NP.y + chest.y;  // origin offset readjustment
      return NP;
    }
  
    // rotate all points by (angle) degrees counterclockwise
  private void orient() 
    { 
      stomach = rotate(stomach);
      neck = rotate(neck);
      nose = rotate(nose);
      lleg = rotate(lleg);
      rleg = rotate(rleg);
      leye = rotate(leye);
      reye = rotate(reye);
      lmouth = rotate(lmouth);
      rmouth = rotate(rmouth);
      larm = rotate(larm);
      rarm = rotate(rarm);
      lip = rotate(lip);
    } 

    /* .................... Public methods that can be called from other classes: ................. */

  public void rotate(int rangle) // counterclockwise rotation
    { angle = (angle + rangle) % 360; 
      calcPoints(chest.x,chest.y); 
      if (autodraw) draw();
    }
             
  public void beHappy()
    { mouthfactor = 1; 
      calcPoints(chest.x,chest.y);
      if (autodraw) draw();
    }
   
  public void beSad()
    { mouthfactor = -1; 
      calcPoints(chest.x,chest.y);
      if (autodraw) draw();
    }
  
  public void armsUp()
    { 
      armfactor = -1;
      calcPoints(chest.x,chest.y); 
      if (autodraw) draw();
    }
 
  public void armsDown()
    { 
      armfactor = 1;
      calcPoints(chest.x,chest.y); 
      if (autodraw) draw();
    }

  public void armsStraight()  
    { armfactor = 0;
      calcPoints(chest.x,chest.y); 
      if (autodraw) draw();
    }

  public void setColor(Color newcolor)
    { color = newcolor; if (autodraw) draw(); }

  public void grow()     // grow by changing scale
    { scale = scale + sizefactor; 
      calcPoints(chest.x,chest.y);
      if (autodraw) draw();
    } 
    
  public void shrink()   // shrink 
    { scale = scale - sizefactor;
      calcPoints(chest.x,chest.y); 
      if (autodraw) draw();
    }   

  public void move(int dx, int dy) // shift by vector dx,dy
    { calcPoints(chest.x+dx,chest.y+dy); if (autodraw) draw(); }
    

    // drawOval does not take center coordinates; hard to maintain
    // relation to other points when figure is rotated:
    private void drawCircle(int cx, int cy, int radius)
    { Graphics g = brush;
      if (radius<0) radius = -1 * radius;
      g.drawOval(cx-radius,cy-radius,2*radius,2*radius);
    }

  public void draw()     // draw to double buffer
  { Graphics g = brush;
    g.setColor(color);
    g.fillOval(leye.x-3,leye.y-3,6,6);   // left eye
    g.fillOval(reye.x-3,reye.y-3,6,6);   // right eye
    g.drawLine(lmouth.x,lmouth.y,lip.x,lip.y); // mouth and lip
    g.drawLine(lip.x,lip.y,rmouth.x,rmouth.y);
    drawCircle(nose.x,nose.y,3);           // nose
    drawCircle(nose.x,nose.y,(scale/2));   // new head
    g.drawLine(neck.x,neck.y,stomach.x,stomach.y);   // body
    g.drawLine(chest.x,chest.y,larm.x,larm.y);       // left arm
    g.drawLine(chest.x,chest.y,rarm.x,rarm.y);       // right arm
    g.drawLine(stomach.x,stomach.y,lleg.x,lleg.y);   // left leg
    g.drawLine(stomach.x,stomach.y,rleg.x,rleg.y);   // right leg
    if (autodraw) parent.nextframe(delms);
  }  //end draw()

    public int xpos() { return chest.x; }
    public int ypos() { return chest.y; }

} // end class figure;